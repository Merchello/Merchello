using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Examine.DataServices
{
    public interface IProductDataService : IIndexDataService
    {
        IEnumerable<IProduct> GetAll();
    }
}