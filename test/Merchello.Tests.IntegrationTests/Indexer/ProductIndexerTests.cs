using System;
using System.Diagnostics;
using System.IO;
using Examine;
using Lucene.Net.Analysis;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Examine.DataServices;
using Merchello.Tests.IntegrationTests.Services;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.Indexer
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