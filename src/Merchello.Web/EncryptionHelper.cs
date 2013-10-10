using System;
using System.Security.Cryptography;
using System.Text;

namespace Merchello.Web
{
    internal class EncryptionHelper
    {
        /// <summary>
        /// Performs simple encryption
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser));
        }

        /// <summary>
        /// Performs simple decryption
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            var encrypedBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(encrypedBytes, null, DataProtectionScope.CurrentUser));
        }
    }
}