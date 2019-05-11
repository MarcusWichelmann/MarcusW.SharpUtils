using System;
using MarcusW.SharpUtils.Core.Cryptography;
using MarcusW.SharpUtils.Core.Extensions;
using Xunit;

namespace MarcusW.SharpUtils.Core.Tests
{
    public class HashingTests
    {
        [Theory]
        [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData("010203", "039058c6f2c0cb492c533b0a4d14ef77cc0f78abccced5287d84a1a2011cfb81")]
        [InlineData("ffffff", "5ae7e6a42304dc6e4176210b83c43024f99a0bce9a870c3b6d2c95fc8ebfb74c")]
        public void Sha256HashIsCorrect(string input, string expectedHash)
        {
            string hash = Hashing.Sha256(input.ParseAsHex()).ToHexString();
            Assert.Equal(expectedHash, hash);
        }

        [Theory]
        [InlineData("", "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData("010203", "86229dc6d2ffbeac7380744154aa700291c064352a0dbdc77b9ed3f2c8e1dac4dc325867d39ddff1d2629b7a393d47f6")]
        [InlineData("ffffff", "f39722ceb67e9f43f2ad8302317d9711e1202186b1fca81d2e803331bf72de1dedc9eda61cdc3de7cb95509846a712b3")]
        public void Sha384HashIsCorrect(string input, string expectedHash)
        {
            string hash = Hashing.Sha384(input.ParseAsHex()).ToHexString();
            Assert.Equal(expectedHash, hash);
        }

        [Theory]
        [InlineData("", "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData("010203", "27864cc5219a951a7a6e52b8c8dddf6981d098da1658d96258c870b2c88dfbcb51841aea172a28bafa6a79731165584677066045c959ed0f9929688d04defc29")]
        [InlineData("ffffff", "0a238ed9ee16bc4fe4a25f1145452b9bd31d7c0605d55da55bef715adae51944c7f8c2ca5ef85cc373e6304b7534168e09732d6b3a20c74f26a4d4a8e4f53d63")]
        public void Sha512HashIsCorrect(string input, string expectedHash)
        {
            string hash = Hashing.Sha512(input.ParseAsHex()).ToHexString();
            Assert.Equal(expectedHash, hash);
        }
    }
}
