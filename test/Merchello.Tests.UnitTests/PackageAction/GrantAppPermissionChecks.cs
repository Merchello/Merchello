namespace Merchello.Tests.UnitTests.PackageAction
{
    using NUnit.Framework;

    [TestFixture]
    public class GrantAppPermissionTests
    {
        [Test]
        public void Can_Parse_Package_Name_Correctly()
        {
            const string PackageName = "Merchello.Bazaar";
            var rootPackage = PackageName.IndexOf('.') > 0
                                  ? PackageName.Substring(0, PackageName.IndexOf('.'))
                                  : PackageName;

            Assert.AreEqual("Merchello", rootPackage);
        }
    }
}