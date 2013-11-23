using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class WarehouseServiceTests : ServiceIntegrationTestBase
    {
        private IWarehouseService _warehouseService;

        [SetUp]
        public void Init()
        {
            _warehouseService = PreTestDataWorker.WarehouseService;
        }

        // MOVED TO INTERNAL
        ///// <summary>
        ///// Test to verify the service can create a warehouse
        ///// </summary>
        //[Test]
        //public void Can_Create_A_Warehouse()
        //{
        //    //// Arrange
        //    IWarehouse warehouse;

        //    //// Act
        //    warehouse = _warehouseService.CreateWarehouse("Expected");

        //    //// Assert
        //    Assert.NotNull(warehouse);
        //}
    }
}
