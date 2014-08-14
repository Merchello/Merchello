using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together warehouse data for testing
    /// </summary>
    public class MockWarehouseDataMaker : MockDataMakerBase
    {
        public static IWarehouse WarehouseForInserting()
        {        
            var addresses = FakeAddresses().ToArray();
            var index = NoWhammyStop.Next(addresses.Count());
            return addresses[index].MakeWarehouse();       
        }

        public static IEnumerable<IWarehouse> WarehouseCollectionForInserting(int count)
        {
            for (var i = 0; i < count; i++) yield return WarehouseForInserting();
        }
    }
}