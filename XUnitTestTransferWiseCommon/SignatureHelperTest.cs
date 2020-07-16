using System;
using System.Reflection;
using Xunit;
using TransferWiseCommon;

namespace XUnitTestTransferWiseCommon
{
    public class SignatureHelperTest
    {
        [Theory]
        [InlineData(null, "", "SHA256WITHRSA")]
        [InlineData(null, null, "SHA256WITHRSA")]
        [InlineData(null, "stringToSign", "")]
        [InlineData(null, "stringToSign", null)]
        [InlineData(null, "", "")]
        [InlineData(null, null, null)]
        public void SignWithPrivateKey_EmptyParameter_ReturnsNull(object signedString, string stringToSign, string algorithm)
        {
            //arrange deploy
            //act
            signedString = SignatureHelper.SignWithPrivateKey(stringToSign, algorithm);

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
        [InlineData(false, "", "stringToSign", "SHA1WITHRSA")]
        [InlineData(false, "signature", "", "SHA1WITHRSA")]
        [InlineData(false, "signature", "stringToSign", "")]
        [InlineData(false, null, "stringToSign", "SHA1WITHRSA")]
        [InlineData(false, "signature", null, "SHA1WITHRSA")]
        [InlineData(false, "signature", "stringToSign", null)]
        [InlineData(false, "", "", "SHA1WITHRSA")]
        [InlineData(false, "signature", "", "")]
        [InlineData(false, "", "stringToSign", "")]
        [InlineData(false, "", "", "")]
        [InlineData(false, null, null, "SHA1WITHRSA")]
        [InlineData(false, "signature", null, null)]
        [InlineData(false, null, "stringToSign", null)]
        [InlineData(false, null, null, null)]
        public void VerifySignature_EmptyParameter_ReturnsFalse(bool verified, string signature, string stringToSign, string algorithm)
        {
            //arrange

            //act
            verified = SignatureHelper.VerifySignature(signature, stringToSign, algorithm);

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