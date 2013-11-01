using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    internal interface ILineItemRepository : IRepositoryQueryable<int, ILineItem>
    {
        IEnumerable<ILineItem> GetByContainerId(int containerId);

        void SaveLineItem(IEnumerable<ILineItem> items);
        void SaveLineItem(ILineItem items);

    }
}