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

        [Test]
        public void Can_Encrypt_And_Decrypt_A_Guid()
        {
            //// Arrange
            var guid = new Guid("2D88E063-43C7-4DE3-83BC-0FE879853ACD");

            //// Act
            var encrypted = EncryptionHelper.Encrypt(guid.ToString());
            Guid decrypted;

            var success = Guid.TryParse(EncryptionHelper.Decrypt(encrypted), out decrypted);

            //// Assert
            Assert.IsTrue(success);
            Assert.AreEqual(guid, decrypted);
        }
    }
}