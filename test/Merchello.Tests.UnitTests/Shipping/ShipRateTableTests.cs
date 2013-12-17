using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Shipping;
using Merchello.Web.Shipping.Gateway.FlatRate;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Shipping
{
    [TestFixture]
    public class ShipRateTableTests
    {

        private IShipRateTable _shipRateTable;

        [SetUp]
        public void Init()
        {
            _shipRateTable = new ShipRateTable(Guid.NewGuid(), new List<IShipRateTier>());
        }

        /// <summary>
        /// Test verifies that a ShipRateTier can be added to the rate table.
        /// </summary>
        [Test]
        public void Can_Verify_That_A_ShipRateTier_Can_Be_Added_To_ShipRateTable()
        {
            //// Arrange
            // handled via setup

            //// Act
            _shipRateTable.AddRow(10, 20, 5);

            //// Assert
            Assert.IsTrue(_shipRateTable.Rows.Any());
            Console.WriteLine("Low: {0} to High: {1}", _shipRateTable.Rows.First().RangeLow, _shipRateTable.Rows.First().RangeHigh);
            Assert.AreEqual(0, _shipRateTable.Rows.First().RangeLow);
        }

        /// <summary>
        /// Test verifies that several shipratetiers can be added to the table and the class 
        /// </summary>
        [Test]
        public void Can_Verify_That_Several_RateTiers_Can_Be_Added_And_Ranges_Are_Preserved()
        {
            //// Arrange
            // handled by setup

            //// Act
            _shipRateTable.AddRow(1, 5, 0);
            _shipRateTable.AddRow(6, 4, 1);
            _shipRateTable.AddRow(3, 4, 1);


            foreach (var row in _shipRateTable.Rows)
            {
                Console.WriteLine("Low: {0} to High: {1}", row.RangeLow, row.RangeHigh);    
            }

            //// Assert
            Assert.AreEqual(3, _shipRateTable.Rows.Count());
            
            Assert.AreEqual(0, _shipRateTable.Rows.First().RangeLow);
            Assert.AreEqual(3, _shipRateTable.Rows.First().RangeHigh);

            Assert.NotNull(_shipRateTable.Rows.First(x => x.RangeLow == 3));
            Assert.NotNull(_shipRateTable.Rows.First(x => x.RangeHigh == 4));

            Assert.AreEqual(4, _shipRateTable.Rows.Last().RangeLow);
            Assert.AreEqual(6, _shipRateTable.Rows.Last().RangeHigh);
        }

        /// <summary>
        /// Test verifies taht a rate tier that spans one or more existing rate tiers is not inserted
        /// </summary>
        [Test]
        public void Can_Verify_That_A_Rate_Tier_That_Spans_Multiple_Existing_Tiers_Is_Not_Inserted()
        {
            //// Arrange
            _shipRateTable.AddRow(0,5, 1);
            _shipRateTable.AddRow(5, 10, 1);
            _shipRateTable.AddRow(10, 20, 1);
            _shipRateTable.AddRow(20, 25, 1);
            _shipRateTable.AddRow(25, 999, 1);
            Assert.AreEqual(5, _shipRateTable.Rows.Count());

            //// Act
            _shipRateTable.AddRow(4, 11, 1);

            //// Assert
            Assert.AreEqual(5, _shipRateTable.Rows.Count());
            Assert.IsFalse(_shipRateTable.Rows.Any(x => x.RangeLow == 4));
        }

    }
}