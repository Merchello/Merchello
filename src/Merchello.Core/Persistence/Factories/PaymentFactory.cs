using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class PaymentFactory : IEntityFactory<IPayment, PaymentDto>
    {
        public IPayment BuildEntity(PaymentDto dto)
        {
            var payment = new Payment(GetCustomer(dto.CustomerDto), dto.PaymentTypeFieldKey, dto.Amount)
            {
                Id = dto.Id,
                MemberId = dto.MemberId,
                GatewayAlias = dto.GatewayAlias,
                PaymentMethodName = dto.PaymentMethodName,
                ReferenceNumber = dto.ReferenceNumber,
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
                MemberId = entity.MemberId,
                GatewayAlias = entity.GatewayAlias,
                PaymentTypeFieldKey = entity.PaymentTypeFieldKey,
                PaymentMethodName = entity.PaymentMethodName,
                ReferenceNumber = entity.ReferenceNumber,
                Amount = entity.Amount,
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }

        private static ICustomer GetCustomer(CustomerDto dto)
        {
            var factory = new CustomerFactory();
            return factory.BuildEntity(dto);
        }

    }
}
