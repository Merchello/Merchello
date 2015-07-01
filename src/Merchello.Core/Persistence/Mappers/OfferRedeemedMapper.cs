namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The offer redeemed mapper.
    /// </summary>
    internal sealed class OfferRedeemedMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedeemedMapper"/> class.
        /// </summary>
        public OfferRedeemedMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Maps <see cref="OfferRedeemed"/> to <see cref="OfferRedeemedDto"/>
        /// </summary>
        internal override void BuildMap()
        {
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.Key, dto => dto.Key);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.OfferSettingsKey, dto => dto.OfferSettingsKey);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.OfferCode, dto => dto.OfferCode);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.OfferProviderKey, dto => dto.OfferProviderKey);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.CustomerKey, dto => dto.CustomerKey);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.InvoiceKey, dto => dto.InvoiceKey);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.RedeemedDate, dto => dto.RedeemedDate);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OfferRedeemed, OfferRedeemedDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}