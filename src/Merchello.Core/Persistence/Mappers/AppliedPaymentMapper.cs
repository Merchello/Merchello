using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class AppliedPaymentMapper : MerchelloBaseMapper
    {
        public AppliedPaymentMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Id, dto => dto.Id);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.PaymentId, dto => dto.PaymentId);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.AppliedPaymentTfKey, dto => dto.AppliedPaymentTfKey);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Description, dto => dto.Description);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<AppliedPayment, AppliedPaymentDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            
        }
    }
}
