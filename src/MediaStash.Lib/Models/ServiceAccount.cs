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


namespace Fitcode.MediaStash.Lib.Models
{
    public class ServiceAccount
    {
        public string Key { get; private set;}
        public string Secret { get; private set; }
        public string Token { get; private set; }

        public ServiceAccount() { }
        public ServiceAccount(string key, string secret)
        {
            this.Key = key;
            this.Secret = secret;
        }
        public ServiceAccount(string key, string secret, string token) : this(key, secret)
        {
            this.Token = token;
        }

        public ServiceAccount CreateForAmazon(string key, string secret)
        {
            return new ServiceAccount
            {
                Key = key,
                Secret = secret
            };
        }
    }
}