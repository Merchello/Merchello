namespace Merchello.Core.Persistence.Factories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// The anonymous customer factory.
    /// </summary>
    internal class AnonymousCustomerFactory : IEntityFactory<IAnonymousCustomer, AnonymousCustomerDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IAnonymousCustomer"/>.
        /// </returns>
        public IAnonymousCustomer BuildEntity(AnonymousCustomerDto dto)
        {
            return new AnonymousCustomer()
            {
                Key = dto.Key,
                LastActivityDate = dto.LastActivityDate,
                ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate                
            };
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="AnonymousCustomerDto"/>.
        /// </returns>
        public AnonymousCustomerDto BuildDto(IAnonymousCustomer entity)
        {
            return new AnonymousCustomerDto()
            {
                Key = entity.Key,
                LastActivityDate = entity.LastActivityDate,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}
