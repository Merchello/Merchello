using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class BasketFactory : IEntityFactory<IBasket, BasketDto>
    {
        public IBasket BuildEntity(BasketDto dto)
        {
            var basket = new Basket(dto.BasketTypeFieldKey)
            {
                Id = dto.Id,
                ConsumerKey = dto.ConsumerKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            basket.ResetDirtyProperties();

            return basket;
        }

        public BasketDto BuildDto(IBasket entity)
        {
            var dto = new BasketDto()
            {
                Id = entity.Id,
                ConsumerKey = entity.ConsumerKey,
                BasketTypeFieldKey = entity.BasketTypeFieldKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
