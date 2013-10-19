using System.Diagnostics;
using Examine;
using Merchello.Tests.IntegrationTests.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Examine.Indexer
{
    [TestFixture]
    public class ProductIndexerTests : ServiceIntegrationTestBase
    {
        
        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void Can_Index_Products()
        {
            //// Arrange
            var timer = new Stopwatch();
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();
            
            timer.Stop();

           

        }
    }
}