using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Shipping
{
    [TestFixture]
    public class ShipRateTableTests
    {

        private ShippingFixedRateTable _shippingFixedRateTable;

        [SetUp]
        public void Init()
        {
            _shippingFixedRateTable = new ShippingFixedRateTable(Guid.NewGuid(), new List<IShipRateTier>());
            _shippingFixedRateTable.IsTest = true;
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
            _shippingFixedRateTable.AddRow(10, 20, 5);

            //// Assert
            Assert.IsTrue(_shippingFixedRateTable.Rows.Any());
            Console.WriteLine("Low: {0} to High: {1}", _shippingFixedRateTable.Rows.First().RangeLow, _shippingFixedRateTable.Rows.First().RangeHigh);
            Assert.AreEqual(0, _shippingFixedRateTable.Rows.First().RangeLow);
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
            _shippingFixedRateTable.AddRow(1, 5, 0);
            _shippingFixedRateTable.AddRow(6, 4, 1);
            _shippingFixedRateTable.AddRow(3, 4, 1);


            foreach (var row in _shippingFixedRateTable.Rows)
            {
                Console.WriteLine("Low: {0} to High: {1}", row.RangeLow, row.RangeHigh);    
            }

            //// Assert
            Assert.AreEqual(3, _shippingFixedRateTable.Rows.Count());
            
            Assert.AreEqual(0, _shippingFixedRateTable.Rows.First().RangeLow);
            Assert.AreEqual(3, _shippingFixedRateTable.Rows.First().RangeHigh);

            Assert.NotNull(_shippingFixedRateTable.Rows.First(x => x.RangeLow == 3));
            Assert.NotNull(_shippingFixedRateTable.Rows.First(x => x.RangeHigh == 4));

            Assert.AreEqual(4, _shippingFixedRateTable.Rows.Last().RangeLow);
            Assert.AreEqual(6, _shippingFixedRateTable.Rows.Last().RangeHigh);
        }

        /// <summary>
        /// Test verifies taht a rate tier that spans one or more existing rate tiers is not inserted
        /// </summary>
        [Test]
        public void Can_Verify_That_A_Rate_Tier_That_Spans_Multiple_Existing_Tiers_Is_Not_Inserted()
        {
            //// Arrange
            _shippingFixedRateTable.AddRow(0,5, 1);
            _shippingFixedRateTable.AddRow(5, 10, 1);
            _shippingFixedRateTable.AddRow(10, 20, 1);
            _shippingFixedRateTable.AddRow(20, 25, 1);
            _shippingFixedRateTable.AddRow(25, 999, 1);
            Assert.AreEqual(5, _shippingFixedRateTable.Rows.Count());

            //// Act
            _shippingFixedRateTable.AddRow(4, 11, 1);

            //// Assert
            Assert.AreEqual(5, _shippingFixedRateTable.Rows.Count());
            Assert.IsFalse(_shippingFixedRateTable.Rows.Any(x => x.RangeLow == 4));
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //[Test]
        //public void Can_Verify_That_A_Rate_Tier_Can_Be_Deleted()
        //{
        //    //// Arrange
        //    _shippingFixedRateTable.AddRow(0, 5, 1);
        //    _shippingFixedRateTable.AddRow(5, 10, 1);
        //    _shippingFixedRateTable.AddRow(10, 20, 1);
        //    _shippingFixedRateTable.AddRow(20, 25, 1);

        //    //// Act
        //    _shippingFixedRateTable.DeleteRow(_shippingFixedRateTable.Rows.First(x => x.RangeLow == 5));
    
        //    //// Assert
        //    Assert.AreEqual(3, _shippingFixedRateTable.Rows.Count());
        //}

    }
}