using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class AppliedPaymentFactory : IEntityFactory<IAppliedPayment, AppliedPaymentDto>
    {
        public IAppliedPayment BuildEntity(AppliedPaymentDto dto)
        {


            var transaction = new AppliedPayment(
                dto.PaymentKey, 
                dto.InvoiceKey, 
                dto.AppliedPaymentTfKey)
            {
                Key = dto.Key,
                Description = dto.Description,
                Amount = dto.Amount,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            transaction.ResetDirtyProperties();

            return transaction;
        }

        public AppliedPaymentDto BuildDto(IAppliedPayment entity)
        {
            var dto = new AppliedPaymentDto()
            {
                Key = entity.Key,
                PaymentKey = entity.PaymentKey,
                InvoiceKey = entity.InvoiceKey,
                AppliedPaymentTfKey = entity.AppliedPaymentTfKey,
                Description = entity.Description,
                Amount = entity.Amount,
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
