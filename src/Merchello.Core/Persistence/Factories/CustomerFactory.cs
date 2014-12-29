namespace Merchello.Core.Persistence.Factories
{
    using System.Collections.Generic;

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
            return BuildEntity(dto, null);
        }

        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <param name="addresses">
        /// The addresses.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public ICustomer BuildEntity(CustomerDto dto, IEnumerable<ICustomerAddress> addresses)
        {
            var customer = new Customer(dto.LoginName)
            {
                Key = dto.Key,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                TaxExempt = dto.TaxExempt,
                ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                ExamineId = dto.CustomerIndexDto.Id,
                Notes = dto.Notes,
                Addresses = addresses ?? new List<ICustomerAddress>(),
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
                    TaxExempt = entity.TaxExempt,
                    LastActivityDate = entity.LastActivityDate,
                    ExtendedData = entity.ExtendedData.SerializeToXml(),
                    CustomerIndexDto = new CustomerIndexDto()
                    {
                      Id = ((Customer)entity).ExamineId,
                      CustomerKey = entity.Key,
                      UpdateDate = entity.UpdateDate,
                      CreateDate = entity.CreateDate
                    },
                    Notes = entity.Notes,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };

            return dto;
        }               
    }
}
