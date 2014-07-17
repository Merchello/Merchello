namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// The ProductDataService interface.
    /// </summary>
    public interface IProductDataService : IIndexDataService
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IProduct"/>.
        /// </returns>
        IEnumerable<IProduct> GetAll();
    }
}