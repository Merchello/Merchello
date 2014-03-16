using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Examine.DataServices
{
    /// <summary>
    /// Defines the OrderDataService
    /// </summary>
    public interface IOrderDataService : IIndexDataService
    {
        IEnumerable<IOrder> GetAll(); 
    }
}