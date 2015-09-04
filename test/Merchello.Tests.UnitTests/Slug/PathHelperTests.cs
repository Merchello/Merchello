namespace Merchello.Tests.UnitTests.Slug
{
    using Merchello.Core;
    using Merchello.Web.Models.VirtualContent;

    using NUnit.Framework;

    [TestFixture]
    public class PathHelperTests
    {
        [Test]
        public void Can_Format_Slug1()
        {
            //// Arrange
            const string Expected = "some-random-name";
            const string Name = "some random name";

            //// Act
            var slug = PathHelper.ConvertToSlug(Name);

            //// Assert
            Assert.AreEqual(Expected, slug);

        }
    }
}