using System;
using Merchello.Web;
using NUnit.Framework;
using Umbraco.Tests.PublishedCache;

namespace Merchello.Tests.UnitTests.Utilities
{
    [TestFixture]
    public class EncryptionHelperTests
    {
        [Test]
        public void Can_Encrypt_And_Decrypt_A_Value()
        {
            //// Arrange
            const string value = "Rusty was here";

            //// Act
            var encrypted = EncryptionHelper.Encrypt(value);
            Console.Write(encrypted);
            var decrypted = EncryptionHelper.Decrypt(encrypted);
            Console.Write(decrypted);

            //// Assert
            Assert.AreNotEqual(value, encrypted);
            Assert.AreEqual(value, decrypted);
        }
    }
}