using System;
using System.Reflection;
using Xunit;
using TransferWiseCommon;

namespace XUnitTestTransferWiseCommon
{
    public class SignatureHelperTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SignWithPrivateKey_NullOrEmptyParameter_ReturnsNull(string stringToSign)
        {
            //arrange
            //act
            var signedString = SignatureHelper.SignWithPrivateKey(stringToSign);

            //assert
            Assert.Null(signedString);
        }

        [Fact]
        public void SignWithPrivateKey_Result_NotEmptyString()
        {
            //arrange
            var stringToSign = "this is a test";

            //act
            var signedString = SignatureHelper.SignWithPrivateKey(stringToSign);

            //assert
            Assert.True(signedString.Length > 0);
        }

        [Theory]
        [InlineData("", "stringToSign")]
        [InlineData("signature", "")]
        [InlineData("signature", "stringToSign")]
        [InlineData(null, "stringToSign")]
        [InlineData("signature", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void VerifySignature_NullOrEmptyParameter_ReturnsFalse(string signature, string stringToSign)
        {
            //arrange

            //act
            var verified = SignatureHelper.VerifySignature(signature, stringToSign);

            //assert
            Assert.False(verified);
        }

        [Fact]
        public void SignedString_WillBeVerified()
        {
            //arrange
            var signature = SignatureHelper.SignWithPrivateKey("stringToSign");

            //act
            var verified = SignatureHelper.VerifySignature(signature, "stringToSign");

            //assert
            Assert.True(verified);
        }
    }
}