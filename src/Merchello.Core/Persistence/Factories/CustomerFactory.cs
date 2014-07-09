namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The customer factory.
    /// </summary>
    internal class CustomerFactory : IEntityFactory<ICustomer, CustomerDto>
    {
        /// <summary>
        /// Builds the entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public ICustomer BuildEntity(CustomerDto dto)
        {
            var customer = new Customer(dto.LoginName)
                {
                    Key = dto.Key,
                    EntityKey = dto.EntityKey,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            customer.ResetDirtyProperties();

            return customer;
        }

        /// <summary>
        /// Build the dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDto"/>.
        /// </returns>
        public CustomerDto BuildDto(ICustomer entity)
        {
            var dto = new CustomerDto()
                {
                    Key = entity.Key,
                    LoginName = entity.LoginName,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    EntityKey = entity.EntityKey,
                    ExtendedData = entity.ExtendedData.SerializeToXml(),
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };

            return dto;
        }       
        
    }
}
