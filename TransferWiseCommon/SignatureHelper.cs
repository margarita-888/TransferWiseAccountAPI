using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TransferWiseCommon
{
    public class SignatureHelper
    {
        public static string SignWithPrivateKey(string stringToSign, string algorithm)
        {
            if (string.IsNullOrEmpty(stringToSign))
            {
                Console.WriteLine($"TransferWiseHttpClient::SignWithPrivateKey. String to sign must be provided. Unable to proceed.");
                return null;
            }

            //  The Shell one-liner to sign a string, encode it with Base64 and print to standard output:
            //    $ printf '<string to sign>' | openssl sha256 -sign <path to private key.pem> | base64

            Console.WriteLine($"Signing with private key.");
            try
            {
                AsymmetricCipherKeyPair keyParameters;

                switch (algorithm)
                {
                    case "SHA256WITHRSA":
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/private.pem"))
                        {
                            keyParameters = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;

                    case "SHA1WITHRSA":
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/private1.pem"))
                        {
                            keyParameters = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;

                    default:
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/private.pem"))
                        {
                            keyParameters = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;
                }

                // Init algorithm
                ISigner signer = SignerUtilities.GetSigner(algorithm);

                // Populate key
                signer.Init(true, keyParameters.Private);

                // Get the bytes to be signed from One-Time Token
                var bytes = Encoding.UTF8.GetBytes(stringToSign);

                // Calc the signature
                signer.BlockUpdate(bytes, 0, bytes.Length);

                var signature = signer.GenerateSignature();

                // Base 64 encode the signer so its 8-bit clean
                var signedString = Convert.ToBase64String(signature);

                return signedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::SignWithPrivateKey. Exception {ex.Message}. {ex.StackTrace}");
                return null;
            }
        }

        public static bool VerifySignature(string signature, string stringToSign, string algorithm)
        {
            try
            {
                RsaKeyParameters keyParameters;
                switch (algorithm)
                {
                    case "SHA256WITHRSA":
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/public.pem"))
                        {
                            keyParameters = (RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;

                    case "SHA1WITHRSA":
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/public1.pem"))
                        {
                            keyParameters = (RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;

                    default:
                        using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/Certificates/public.pem"))
                        {
                            keyParameters = (RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                        }
                        break;
                }

                byte[] stringToSignBytes = Encoding.UTF8.GetBytes(stringToSign);
                byte[] signatureBytes = Convert.FromBase64String(signature);

                ISigner signer = SignerUtilities.GetSigner(algorithm);
                signer.Init(false, keyParameters);
                signer.BlockUpdate(stringToSignBytes, 0, stringToSignBytes.Length);
                return signer.VerifySignature(signatureBytes);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Verification failed with the error: " + exc.ToString());
                return false;
            }
        }
    }
}