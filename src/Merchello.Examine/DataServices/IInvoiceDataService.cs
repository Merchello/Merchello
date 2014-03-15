using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Examine.DataServices
{
    /// <summary>
    /// Defines an Invoice Data SErvice
    /// </summary>
    public interface IInvoiceDataService : IIndexDataService
    {
        IEnumerable<IInvoice> GetAll(); 
    }
}