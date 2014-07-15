namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using Core.Models;

    /// <summary>
    /// Defines he CustomerDataService interface.
    /// </summary>
    public interface ICustomerDataService : IIndexDataService
    {
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="ICustomer"/>.
        /// </returns>
        IEnumerable<ICustomer> GetAll(); 
    }
}