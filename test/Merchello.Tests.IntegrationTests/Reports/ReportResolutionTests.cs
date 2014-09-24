using Merchello.Tests.Base.TestHelpers;

namespace Merchello.Tests.IntegrationTests.Reports
{
    using System.Linq;

    using Merchello.Tests.IntegrationTests.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.Reporting;

    using Moq;

    using NUnit.Framework;

    using Umbraco.Core;
    using Umbraco.Web;

    [TestFixture]
    public class ReportResolutionTests : MerchelloAllInTestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Can resolve reports api controller 
        /// </summary>
        [Test]
        public void Can_Resolve_Reports_Types_With_Umbraco_Plugin_Manager()
        {
            //// Arrange
            
            //// Act
            var types = PluginManager.Current.ResolveReportApiControllers();

            //// Assert
            Assert.IsTrue(types.Any());
        }


        [Test]
        public void Can_Show_ReportApiControllerResolver_Is_Setup_In_The_WebBootManager()
        {
            //// Assert
            Assert.IsTrue(ReportApiControllerResolver.HasCurrent);
        }

        [Test]
        public void Can_Resolve_Types_With_The_Reports_Resolver()
        {
            //// Arrange
            
            //// Act
            var types = ReportApiControllerResolver.Current.ResolvedTypes;

            //// Assert
            Assert.IsTrue(types.Any());
        }

        ///// <summary>
        ///// Can resolve api controller 
        ///// This requires an UmbracoContext
        ///// </summary>
        ////[Test]
        ////public void Can_Resolve_ReportApiControllers_Using_Resolver()
        ////{
        ////    //// Arrange
            
        ////    //// Act
        ////    var controllers = ReportApiControllerResolver.Current.GetAll();

        ////    //// Assert
        ////    Assert.IsTrue(controllers.Any());
        ////}
    }
}