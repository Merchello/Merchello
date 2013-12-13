using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Cache;

namespace Merchello.Web.Models
{
    public class ShipRateTable : IShipRateTable
    {
        private readonly List<IShipRateTier> _shipRateTiers;
        private readonly Guid _shipMethodKey;

        public ShipRateTable(Guid shipMethodKey)
            : this(shipMethodKey, new List<IShipRateTier>())
        { }

        internal ShipRateTable(Guid shipMethodKey, IEnumerable<IShipRateTier> rows)
        {
            var shipRateTiers = rows as IShipRateTier[] ?? rows.ToArray();

            Mandate.ParameterCondition(shipMethodKey != Guid.Empty, "shipMethodKey");
            Mandate.ParameterNotNull(shipRateTiers, "rows");
            

            _shipMethodKey = shipMethodKey;
            _shipRateTiers = new List<IShipRateTier>();
            _shipRateTiers.AddRange(shipRateTiers);

        }

        public static ShipRateTable GetShipRateTable(Guid shipMethodKey)
        {
            var context = MerchelloContext.Current;

            return (ShipRateTable)context.Cache
                .RequestCache.GetCacheItem(CacheKeys.ShipRateTableCacheKey(shipMethodKey), 
                () => GetShipRateTable(MerchelloContext.Current, shipMethodKey));
        }

        internal static ShipRateTable GetShipRateTable(IMerchelloContext merchelloContext, Guid shipMethodKey)
        {
            var rows = merchelloContext.Services.ShippingService.GetShipRateTiersByShipMethodKey(shipMethodKey);
            return new ShipRateTable(shipMethodKey, rows);
        }

        public Guid ShipMethodKey {
            get { return _shipMethodKey; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipRateTier"></param>
        /// <remarks>
        /// Requires a call to Save() to persist
        /// </remarks>
        public void AddRow(IShipRateTier shipRateTier)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipRateTier"></param>
        /// <remarks>
        /// Requires a call to save to persist
        /// </remarks>
        public void DeleteRow(IShipRateTier shipRateTier)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        internal static void Save(IMerchelloContext merchelloContext, IShipRateTable rateTable)
        {
            merchelloContext.Cache.RequestCache.ClearCacheItem(CacheKeys.ShipRateTableCacheKey(rateTable.ShipMethodKey));
            merchelloContext.Services.ShippingService.Save(rateTable.Rows);
        }


        public decimal GetRate(decimal rangeValue)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IShipRateTier> Rows
        {
            get { return _shipRateTiers; }

        }
    }
}