namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    internal sealed class AppliedPaymentMapper : MerchelloBaseMapper
    {
        public AppliedPaymentMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Key, dto => dto.Key);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.PaymentKey, dto => dto.PaymentKey);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.InvoiceKey, dto => dto.InvoiceKey);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.AppliedPaymentTfKey, dto => dto.AppliedPaymentTfKey);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Description, dto => dto.Description);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            
        }
    }
}
