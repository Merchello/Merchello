namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using Core.Models;

    /// <summary>
    /// Defines an Invoice Data Service
    /// </summary>
    public interface IInvoiceDataService : IIndexDataService
    {
        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IInvoice"/>.
        /// </returns>
        IEnumerable<IInvoice> GetAll(); 
    }
}