using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class PaymentMethodFactory : IEntityFactory<IPaymentMethod, PaymentMethodDto>
    {
        public IPaymentMethod BuildEntity(PaymentMethodDto dto)
        {
            var paymentMethod = new PaymentMethod(dto.ProviderKey)
                {
                    Key = dto.Key,
                    Name = dto.Name,
                    Description = dto.Description,
                    PaymentCode = dto.PaymentCode,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            paymentMethod.ResetDirtyProperties();

            return paymentMethod;
        }

        public PaymentMethodDto BuildDto(IPaymentMethod entity)
        {
            return new PaymentMethodDto()
                {
                    Key = entity.Key,
                    ProviderKey = entity.ProviderKey,
                    Name = entity.Name,
                    Description = entity.Description,
                    PaymentCode = entity.PaymentCode,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}