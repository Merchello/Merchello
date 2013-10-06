using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class PaymentFactory : IEntityFactory<IPayment, PaymentDto>
    {
        public IPayment BuildEntity(PaymentDto dto)
        {
            var payment = new Payment(
               new CustomerFactory().BuildEntity(dto.CustomerDto), dto.PaymentTfKey, dto.Amount)
            {
                Id = dto.Id,
                ProviderKey = dto.ProviderKey,
                PaymentMethodName = dto.PaymentMethodName,
                ReferenceNumber = dto.ReferenceNumber,
                Authorized = dto.Authorized,
                Collected = dto.Collected,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            payment.ResetDirtyProperties();

            return payment;
        }

        public PaymentDto BuildDto(IPayment entity)
        {
            var dto = new PaymentDto()
            {
                Id = entity.Id,                
                CustomerKey = entity.CustomerKey,
                ProviderKey = entity.ProviderKey,
                PaymentTfKey = entity.PaymentTypeFieldKey,
                PaymentMethodName = entity.PaymentMethodName,
                ReferenceNumber = entity.ReferenceNumber,
                Amount = entity.Amount,
                Authorized = entity.Authorized,
                Collected = entity.Collected,
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }

    }
}
