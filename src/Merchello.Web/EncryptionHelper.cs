namespace Merchello.Web
{
    using Umbraco.Core;

    /// <summary>
    /// The encryption helper.
    /// </summary>
    internal class EncryptionHelper
    {
        /// <summary>
        /// Performs simple encryption
        /// </summary>
        /// <param name="value">The value to be encrypted</param>
        /// <returns>The encrypted <see cref="string"/></returns>
        public static string Encrypt(string value)
        {
            return value.EncryptWithMachineKey();
        }

        /// <summary>
        /// Performs simple decryption
        /// </summary>
        /// <param name="value">The value to be decrypted</param>
        /// <returns>The decrypted <see cref="string"/></returns>
        public static string Decrypt(string value)
        {
            return value.DecryptWithMachineKey();            
        }
    }
}