using System;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Mocks
{
    public sealed class ShipmentStatusMock : NotifiedStatusBase, IShipmentStatus
    {
        public ShipmentStatusMock()
        {
            Key = Constants.ShipmentStatus.Quoted;
            Alias = "quoted";
            Name = "Quoted";
            Active = true;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }
    }
}