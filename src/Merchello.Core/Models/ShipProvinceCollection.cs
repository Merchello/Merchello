using System;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    public class ShipProvinceCollection : NotifiyCollectionBase<string, IShipProvince>
    {
        protected override string GetKeyForItem(IShipProvince item)
        {
            return item.Code;
        }

        public override int IndexOfKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}