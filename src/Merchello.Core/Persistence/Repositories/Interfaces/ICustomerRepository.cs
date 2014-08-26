namespace Merchello.Core.Persistence.Repositories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Marker Interface for the customer repository
    /// </summary>
    internal interface ICustomerRepository : IPagedRepository<ICustomer, CustomerDto>
    {
    }
}
