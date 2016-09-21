namespace Merchello.Tests.Umbraco
{
    using System;

    using Merchello.Tests.Umbraco.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class Scrap : UmbracoInstanceBase
    {
        [Test]
        public void One()
        {
            Console.WriteLine("Got here");
        }

        protected override bool EnableCache
        {
            get
            {
                return true;
            }
        }

    }
}