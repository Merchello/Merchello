using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using CacheKeys = Merchello.Core.Cache.CacheKeys;

namespace Merchello.Core.Gateways.Shipping.RateTable
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
            IsTest = false;
            var shipRateTiers = rows as IShipRateTier[] ?? rows.ToArray();

            Mandate.ParameterCondition(shipMethodKey != Guid.Empty, "shipMethodKey");
            Mandate.ParameterNotNull(shipRateTiers, "rows");
            

            _shipMethodKey = shipMethodKey;
            _shipRateTiers = new List<IShipRateTier>();
            _shipRateTiers.AddRange(shipRateTiers);

        }

        internal static ShipRateTable GetShipRateTable(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCacheProvider, Guid shipMethodKey)
        {
            return (ShipRateTable) runtimeCacheProvider.GetCacheItem(
                CacheKeys.GatewayShipMethodCacheKey(shipMethodKey),
                delegate
                    {
                        var rows = gatewayProviderService.GetShipRateTiersByShipMethodKey(shipMethodKey);
                        return new ShipRateTable(shipMethodKey, rows);
                    });
        }

        /// <summary>
        /// The 'unique' ShipMethodKey of the ship method associated with the <see cref="IShipRateTable"/>
        /// </summary>
        public Guid ShipMethodKey {
            get
            {
                return _shipMethodKey;
            }
        }

        /// <summary>
        /// Adds a rate tier row to the ship rate table
        /// </summary>
        /// <param name="rangeLow">The lowest qualifying value defining the range</param>
        /// <param name="rangeHigh">The highest qualifying value defining the range</param>
        /// <param name="rate">The rate or cost assoicated with the range</param>
        /// <remarks>
        /// Requires a call to Save() to persist
        /// </remarks>
        public void AddRow(decimal rangeLow, decimal rangeHigh, decimal rate)
        {
            AddRow(new ShipRateTier(_shipMethodKey)
                    {
                        RangeLow = rangeLow,
                        RangeHigh = rangeHigh,
                        Rate = rate
                    });
        }

        /// <summary>
        /// Adds a rate tier row to the ship rate table
        /// </summary>
        /// <param name="shipRateTier"></param>
        /// <remarks>
        /// Requires a call to Save() to persist
        /// </remarks>
        internal void AddRow(IShipRateTier shipRateTier)
        {
            if (!ValidateRateTier(ref shipRateTier)) return;
                        
            // TODO : Refactor this validation
            if (!Rows.Any())
            {
                shipRateTier.RangeLow = 0;
                _shipRateTiers.Add(shipRateTier);
            }
            else
            {
                // confirm there is not already a matching tier
                if(Rows.FirstOrDefault(x => x.RangeLow == shipRateTier.RangeLow && x.RangeHigh == shipRateTier.RangeHigh) != null) return;
                
                // find the insertion point
                var index = Rows.IndexOf(Rows.Where(y => y.RangeHigh >= shipRateTier.RangeLow).OrderBy(z => z.RangeLow).FirstOrDefault());
                if (index < 0)
                {
                    shipRateTier.RangeLow = Rows.Last().RangeHigh;
                    AddRow(shipRateTier);
                    return;
                }

                // not found or at the end of the table
                if (index < 0 || index == Rows.IndexOf(Rows.Last()))
                {
                    shipRateTier.RangeLow = Rows.Last().RangeHigh;
                    if (shipRateTier.RangeHigh <= shipRateTier.RangeLow) return;
                    _shipRateTiers.Add(shipRateTier);
                }
                else // insert in the middle of the table
                {
                    // verify that inserting this tier will not create a span encapsulating another tier
                    if (shipRateTier.RangeHigh >= _shipRateTiers[index + 1].RangeHigh) return;
                    if (shipRateTier.RangeLow <= _shipRateTiers[index].RangeLow) return;

                    // match the range low to range high in the following tier
                    _shipRateTiers[index + 1].RangeLow = shipRateTier.RangeHigh;

                    // verify that the high value at the current index is equal to the low value of the tier to be insert
                    _shipRateTiers[index].RangeHigh = shipRateTier.RangeLow;

                    _shipRateTiers.Insert(index + 1, shipRateTier);

                }
            }
 
        }

        /// <summary>
        /// Asserts the ranges in the rate tier are low to high, non zero and not equal.
        /// </summary>
        /// <param name="shipRateTier"></param>
        /// <returns></returns>
        private static bool ValidateRateTier(ref IShipRateTier shipRateTier)
        {
            if (shipRateTier.RangeLow < 0 || shipRateTier.RangeHigh < 0) return false;
            if (shipRateTier.RangeLow == shipRateTier.RangeHigh) return false;
            if (shipRateTier.RangeHigh > shipRateTier.RangeLow) return true;

            var temp = shipRateTier.RangeLow;
            shipRateTier.RangeLow = shipRateTier.RangeHigh;
            shipRateTier.RangeHigh = temp;
            return true;
        }

        /// <summary>
        /// Deletes a row
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void DeleteRow(IShipRateTier shipRateTier)
        {
            if (MerchelloContext.Current == null) throw new InvalidOperationException("MerchelloContext.Current is null");

            var row = Rows.FirstOrDefault(x => x.Key == shipRateTier.Key);
            if (!Rows.Any() || row == null) return;
            if (Rows.IndexOf(Rows.Last()) != Rows.IndexOf(row))
            { 
                _shipRateTiers[Rows.IndexOf(row) + 1].RangeLow = row.RangeLow;
            }
            _shipRateTiers.Remove(row);

            if(!IsTest) DeleteRow(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, this, shipRateTier);
        }

        /// <summary>
        /// Persists the rate table to the database and refreshes the runtime cache
        /// </summary>
        public void Save()
        {
            if(MerchelloContext.Current == null) throw new InvalidOperationException("MerchelloContext.Current is null");
            Save(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, this);
        }

        internal static void Save(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider cache, IShipRateTable rateTable)
        {
           
            // clear the current cached item
            // TODO : This should use the distributed cache referesher
            cache.ClearCacheItem(CacheKeys.GatewayShipMethodCacheKey(rateTable.ShipMethodKey));

            // persist and enter into cache
           gatewayProviderService.Save(rateTable.Rows);
           cache.GetCacheItem(CacheKeys.GatewayShipMethodCacheKey(rateTable.ShipMethodKey), () => rateTable);   
        }

        internal static void DeleteRow(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider cache, IShipRateTable rateTable, IShipRateTier shipRateTier)
        {
            // clear the current cached item
            // TODO : This should use the distributed cache referesher
            cache.ClearCacheItem(CacheKeys.GatewayShipMethodCacheKey(rateTable.ShipMethodKey));

            gatewayProviderService.Delete(shipRateTier);
            cache.GetCacheItem(CacheKeys.GatewayShipMethodCacheKey(rateTable.ShipMethodKey), () => rateTable);   
        }

        /// <summary>
        /// Gets the decimal rate associated with the range
        /// </summary>
        /// <param name="rangeValue">The value within a range used to determine which rate to return</param>
        /// <returns>A decimal rate or zero (0) if not found</returns>
        public decimal FindRate(decimal rangeValue)
        {
            var rateTier = Rows.FirstOrDefault(x => x.RangeLow < rangeValue && x.RangeHigh >= rangeValue);
            return rateTier == null ? 0M : rateTier.Rate;
        }

        /// <summary>
        /// The rows of the rate table
        /// </summary>
        public IEnumerable<IShipRateTier> Rows
        {
            get { return _shipRateTiers.OrderBy(x => x.RangeLow); }

        }

        internal bool IsTest { get; set; }
    }
}