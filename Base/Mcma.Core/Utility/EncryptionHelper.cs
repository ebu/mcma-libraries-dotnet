using System;
using System.Security.Cryptography;
using System.Text;
using Mcma.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcma.Utility
{
    /// <summary>
    /// Simple support for basic RSA encryption
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// Exports RSA parameters as base64-encoded json
        /// </summary>
        /// <param name="rsa">The RSA algorithm</param>
        /// <param name="includePrivate">Flag indicating whether the export should include private parameters</param>
        /// <returns>The RSA parameters as base64-encoded json</returns>
        public static string ExportJson(this RSA rsa, bool includePrivate)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(rsa.ExportParameters(includePrivate).ToMcmaJson().ToString(Formatting.None)));

        /// <summary>
        /// Imports RSA parameters from base64-encoded json
        /// </summary>
        /// <param name="rsa">The RSA algorithm</param>
        /// <param name="json">Base64-encoded json containing the RSA parameters to import</param>
        public static void ImportJson(this RSA rsa, string json)
            => rsa.ImportParameters(JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(json))).ToMcmaObject<RSAParameters>());

        /// <summary>
        /// Generates new public and private RSA keys and exports them as base64-encoded json
        /// </summary>
        /// <returns></returns>
        public static (string, string) GenerateNewKeys()
        {
            using var rsaCryptoServiceProvider = RSA.Create();
            return (rsaCryptoServiceProvider.ExportJson(true), rsaCryptoServiceProvider.ExportJson(false));
        }

        /// <summary>
        /// Encrypts a string using RSA
        /// </summary>
        /// <param name="toEncrypt">The string to encrypt</param>
        /// <param name="publicKeyJson">The public RSA key as base64-encoded json</param>
        /// <returns>The encrypted string</returns>
        public static string Encrypt(string toEncrypt, string publicKeyJson)
        {
            using var rsa = RSA.Create();
            
            rsa.ImportJson(publicKeyJson);

            var encryptedBytes = rsa.EncryptValue(Encoding.UTF8.GetBytes(toEncrypt));

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts a string using RSA
        /// </summary>
        /// <param name="toDecrypt">The string to decrypt</param>
        /// <param name="privateKeyJson">The private RSA key as base64-encoded json</param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt, string privateKeyJson)
        {
            using var rsa = RSA.Create();
            
            rsa.ImportJson(privateKeyJson);

            var decryptedBytes = rsa.DecryptValue(Convert.FromBase64String(toDecrypt));

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
