using System;
using System.Reflection;
using Xunit;
using TransferWiseCommon;

namespace XUnitTestTransferWiseCommon
{
    public class SignatureHelperTest
    {
        [Theory]
        [InlineData("", "SHA256WITHRSA")]
        [InlineData(null, "SHA256WITHRSA")]
        [InlineData("stringToSign", "")]
        [InlineData("stringToSign", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void SignWithPrivateKey_EmptyParameter_ReturnsNull(string stringToSign, string algorithm)
        {
            //arrange
            //act
            var signedString = SignatureHelper.SignWithPrivateKey(stringToSign, algorithm);

            //assert
            Assert.Null(signedString);
        }

        [Fact]
        public void SignWithPrivateKey_Result_NotEmptyString()
        {
            //arrange
            var stringToSign = "this is a test";

            //act
            var signedString = SignatureHelper.SignWithPrivateKey(stringToSign, "SHA256WITHRSA");

            //assert
            Assert.True(signedString.Length > 0);
        }

        [Theory]
        [InlineData("", "stringToSign", "SHA1WITHRSA")]
        [InlineData("signature", "", "SHA1WITHRSA")]
        [InlineData("signature", "stringToSign", "")]
        [InlineData(null, "stringToSign", "SHA1WITHRSA")]
        [InlineData("signature", null, "SHA1WITHRSA")]
        [InlineData("signature", "stringToSign", null)]
        [InlineData("", "", "SHA1WITHRSA")]
        [InlineData("signature", "", "")]
        [InlineData("", "stringToSign", "")]
        [InlineData("", "", "")]
        [InlineData(null, null, "SHA1WITHRSA")]
        [InlineData("signature", null, null)]
        [InlineData(null, "stringToSign", null)]
        [InlineData(null, null, null)]
        public void VerifySignature_EmptyParameter_ReturnsFalse(string signature, string stringToSign, string algorithm)
        {
            //arrange

            //act
            var verified = SignatureHelper.VerifySignature(signature, stringToSign, algorithm);

            //assert
            Assert.False(verified);
        }

        [Fact]
        public void StringSignedWith_SHA1WITHRSA_WillBeVerified()
        {
            //arrange
            var signature = SignatureHelper.SignWithPrivateKey("stringToSign", "SHA1WITHRSA");

            //act
            var verified = SignatureHelper.VerifySignature(signature, "stringToSign", "SHA1WITHRSA");

            //assert
            Assert.True(verified);
        }

        [Fact]
        public void StringSignedWith_SHA256WITHRSA_WillFail()
        {
            //arrange
            var signature = SignatureHelper.SignWithPrivateKey("stringToSign", "SHA256WITHRSA");

            //act
            var verified = SignatureHelper.VerifySignature(signature, "stringToSign", "SHA256WITHRSA");

            //assert
            Assert.False(verified);
        }
    }
}