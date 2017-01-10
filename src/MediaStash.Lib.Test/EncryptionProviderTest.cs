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

using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using Fitcode.MediaStash.Lib.Providers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaStash.Lib.Test
{
    [TestFixture]
    public class EncryptionProviderTest
    {
        private static IEncryptionConfiguration _encryptionConfiguration;
        private static IEncryptionProvider _encryptionProvider;

        [SetUp]
        public void Init()
        {
            _encryptionConfiguration = new EncryptionConfiguration
            {
                Password = "test",
                EncryptionExtension = ".sec"
            };
            _encryptionProvider = new EncryptionProvider(_encryptionConfiguration);
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        public static void TestImageEncryption()
        {
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia("anime16.jpg", new FileStream(@"C:\Users\felip_kw0ekdh\Desktop\anime16.jpg", FileMode.Open).ToByteArray())
                }
            };

            _encryptionProvider.Encrypt(container);

            using (var writer = new FileStream($@"C:\Users\felip_kw0ekdh\Desktop\{container.Media.FirstOrDefault().Name}", FileMode.OpenOrCreate))
            {
                var data = container.Media.FirstOrDefault().Data;

                writer.Write(data, 0, data.Length);
            }

            Assert.IsTrue(File.Exists($@"C:\Users\felip_kw0ekdh\Desktop\{container.Media.FirstOrDefault().Name}"));
        }

        [Test]
        public static void TestImageDecryption()
        {
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia("anime16.jpg.sec", new FileStream(@"C:\Users\felip_kw0ekdh\Desktop\anime16.jpg.sec", FileMode.Open).ToByteArray())
                }
            };

            _encryptionProvider.Decrypt(container);

            using (var writer = new FileStream($@"C:\Users\felip_kw0ekdh\Desktop\decrypted-{container.Media.FirstOrDefault().Name}", FileMode.OpenOrCreate))
            {
                var data = container.Media.FirstOrDefault().Data;

                writer.Write(data, 0, data.Length);
            }

            Assert.IsTrue(File.Exists($@"C:\Users\felip_kw0ekdh\Desktop\decrypted-{container.Media.FirstOrDefault().Name}"));
        }
    }
}
