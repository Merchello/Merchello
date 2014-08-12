namespace Merchello.Tests.UnitTests.Reports
{
    using System;
    using System.Linq;

    using Lucene.Net.Search;

    using Merchello.Core;
    using Merchello.Core.Reporting;
    using Merchello.Web;

    using NUnit.Framework;

    using Umbraco.Core;

    [TestFixture]
    public class ReportResolutionTests
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            ReportDataAggregatorResolver.Current = new ReportDataAggregatorResolver(PluginManager.Current.ResolveReportDataAggregators());  
        }

        [Test]
        public void Can_Resolve_ReportDataAggrators_Types()
        {
            //// Arrange
            
            //// Act
            var types = ReportDataAggregatorResolver.Current.GetAll();

            //// Assert
            Assert.IsTrue(types.Any());
            Console.Write(types.Count());
        }

        [Test]
        public void Can_Get_An_Instance_Of_The_SalesOverTimeDataAggregator()
        {
            //// Arrange
        }
    }
}