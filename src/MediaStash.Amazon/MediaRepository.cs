﻿#region License
// Copyright (c) 2017 Fitcode.io (info@fitcode.io)
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fitcode.MediaStash.Lib.Models;
using Fitcode.MediaStash.Lib.Abstractions;
using Amazon.S3;
using Amazon.S3.Model;

namespace Fitcode.MediaStash.Azure
{
    public class MediaRepository : IMediaRepository, IDisposable
    {
        private AmazonS3Client _s3Client = null;

        public MediaRepository(IRepositoryConfiguration config)
        {
            this.Config = config;

            _s3Client = new AmazonS3Client(config.Account.Key, config.Account.Secret, Amazon.RegionEndpoint.USEast1);
        }

        public MediaRepository(IRepositoryConfiguration config, IEnumerable<IProvider> providers) : this(config)
        {
            this.Providers = providers;
        }

        public IRepositoryConfiguration Config { get; private set; }
        public IEnumerable<IProvider> Providers { get; private set; }

        public async Task RunProviderProcess(IMedia media)
        {
            if (media.Metadata == null)
                media.Metadata = new Dictionary<string, string>();

            if (Providers != null && Providers.Count() > 0)
            {
                foreach (var provider in Providers)
                {
                    media.Data = await provider.ProcessAsync(media.Data);
                    media.Metadata.Add(provider.GetType().Name, provider.GetType().FullName);
                }
            }
        }

        public async Task ReverseProvider(IMedia media)
        {
            if (Providers != null && Providers.Count() > 0)
            {
                foreach (var provider in Providers.Reverse()) // We wan't to reverse for the correct processing order
                {
                    if (media.Metadata.ContainsKey(provider.GetType().Name))
                        media.Data = await provider.ReverseAsync(media.Data);
                }
            }
        }

        public async Task<IEnumerable<S3Object>> ListObjectRequest(string storageContainer, string prefix)
        {
            var request = new ListObjectsRequest();
            request.BucketName = storageContainer;
            request.Prefix = prefix;

            ListObjectsResponse response = await _s3Client.ListObjectsAsync(request);

            return response.S3Objects;
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer)
        {
            await StashContainerAsync(mediaContainer, Config.RootContainer);
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer, string storageContainer)
        {
            foreach (var file in mediaContainer.Media)
            {
                using (var stream = new MemoryStream(file.Data))
                {
                    PutObjectRequest request = new PutObjectRequest();
                    request.AutoCloseStream = true;
                    request.BucketName = storageContainer;
                    request.CannedACL = S3CannedACL.PublicRead;
                    request.Key = $@"{mediaContainer.Path}/{file.Name}";
                    request.InputStream = stream;
                    
                    await RunProviderProcess(file);

                    // Append metadata
                    if (file.Metadata.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> entry in file.Metadata)
                            request.Metadata.Add(entry.Key, entry.Value);
                    }

                    PutObjectResponse response = await _s3Client.PutObjectAsync(request);
              
                    file.Uri = $@"https://s3.amazonaws.com/{storageContainer}/{mediaContainer.Path}/{file.Name}";
                }
            }
        }

        public async Task StashMediaAsync(string path, IEnumerable<IMedia> mediaCollection)
        {
            await StashMediaAsync(path, Config.RootContainer, mediaCollection);
        }

        public async Task StashMediaAsync(string path, string storageContainer, IEnumerable<IMedia> mediaCollection)
        {
            await StashContainerAsync(new MediaContainer
            {
                Path = path,
                Media = mediaCollection.Select(s => new GenericMedia(s.Name, s.Data)).AsEnumerable()
            }, storageContainer);
        }

        public async Task<IMediaContainer> GetMediaContainerAsync(string path, bool loadResourcePathOnly = false)
        {
            return await GetMediaContainerAsync(path, Config.RootContainer, loadResourcePathOnly);
        }

        public async Task<IMediaContainer> GetMediaContainerAsync(string path, string storageContainer, bool loadResourcePathOnly)
        {
            var prefix = $@"{path.Replace(@"\", "/")}/";
            var files = await ListObjectRequest(storageContainer, prefix);
            var container = new MediaContainer();
            var media = new List<GenericMedia>();

            foreach (var item in files)
            {
                var amazonFile = await _s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = storageContainer,
                    Key = item.Key
                });

                using (var file = new MemoryStream())
                {
                    await amazonFile.ResponseStream.CopyToAsync(file);
             
                    var mediaFile = new GenericMedia(Path.GetFileName(item.Key), file.ToArray())
                    {
                        Uri = $@"https://s3.amazonaws.com/{storageContainer}/{item.Key}",
                        Metadata = new Dictionary<string, string>()
                    };

                    if (amazonFile.ResponseMetadata.Metadata != null)
                    {
                        foreach (var key in amazonFile.Metadata.Keys)
                            mediaFile.Metadata.Add(key, amazonFile.Metadata[key]);
                    }

                    await ReverseProvider(mediaFile);

                    if (mediaFile.Data != null && mediaFile.Data.Length > 0)
                        media.Add(mediaFile);
                }
            }

            container.Media = media;

            return container;
        }

        public Task<IEnumerable<IMedia>> GetMediaAsync(string path, bool loadResourcePathOnly = false)
        {
            return GetMediaAsync(path, Config.RootContainer, loadResourcePathOnly);
        }

        public async Task<IEnumerable<IMedia>> GetMediaAsync(string path, string storageContainer, bool loadResourcePathOnly)
        {
            return (await GetMediaContainerAsync(path, storageContainer, loadResourcePathOnly))?.Media;
        }

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_disposed) return;
            if (isDisposing)
            {
                _disposed = true;

                if (_s3Client != null)
                {
                    _s3Client.Dispose();
                    _s3Client = null;
                }
            }
        }
    }
}
