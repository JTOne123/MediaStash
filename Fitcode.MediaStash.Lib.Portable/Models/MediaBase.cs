﻿#region License
// Copyright (c) 2017 Fitcode.io
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

using Fitcode.MediaStash.Lib.Contracts;
using PCLStorage;
using System;
using System.IO;

namespace Fitcode.MediaStash.Lib.Models
{
    /// <summary>
    /// Basic outline for media files.
    /// </summary>
    /// <typeparam name="T">Media type template constrained to Stream.</typeparam>
    public abstract class MediaBase<T> : IMedia where T : IFile
    {
        public string Name { get; set; }

        public T Media { get; set; }

        public virtual byte[] Data
        {
            get
            {
                return Media?.ToByteArray()?.Result;
            }
        }

        public MediaBase(string name, T media)
        {
            this.Name = name;
            this.Media = media;
        }
    }
}