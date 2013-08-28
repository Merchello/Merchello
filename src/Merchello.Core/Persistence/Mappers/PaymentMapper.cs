using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Payment"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class PaymentMapper : MerchelloBaseMapper
    {
        public PaymentMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<Payment, PaymentDto>(src => src.Id, dto => dto.Id);            
            CacheMap<Payment, PaymentDto>(src => src.InvoiceId, dto => dto.InvoiceId);
            CacheMap<Payment, PaymentDto>(src => src.CustomerKey, dto => dto.CustomerKey);
            CacheMap<Payment, PaymentDto>(src => src.MemberId, dto => dto.MemberId);
            CacheMap<Payment, PaymentDto>(src => src.GatewayAlias, dto => dto.GatewayAlias);
            CacheMap<Payment, PaymentDto>(src => src.PaymentTypeFieldKey, dto => dto.PaymentTypeFieldKey);                        
            CacheMap<Payment, PaymentDto>(src => src.PaymentMethodName, dto => dto.PaymentMethodName);
            CacheMap<Payment, PaymentDto>(src => src.ReferenceNumber, dto => dto.ReferenceNumber);
            CacheMap<Payment, PaymentDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<Payment, PaymentDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<Payment, PaymentDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Payment, PaymentDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
