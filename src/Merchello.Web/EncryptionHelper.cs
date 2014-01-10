using Umbraco.Core;

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
            return value.EncryptWithMachineKey();
            
        }

        /// <summary>
        /// Performs simple decryption
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            return value.DecryptWithMachineKey();            
        }
    }
}