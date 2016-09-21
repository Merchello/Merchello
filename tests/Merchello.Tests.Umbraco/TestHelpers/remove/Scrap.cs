namespace Merchello.Tests.Umbraco
{
    using System;

    using Merchello.Tests.Umbraco.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class Scrap : ApplicationContextInstanceBase
    {
        [Test]
        public void BaseDirectory()
        {
            Console.Write(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}