using System;
using System.Collections.Generic;
using System.Configuration;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    internal interface ILineItemRepository : IRepositoryQueryable<Guid, ILineItem>
    {
        IEnumerable<ILineItem> GetByContainerKey(Guid containerKey);

        void SaveLineItem(IEnumerable<ILineItem> items, Guid containerKey);
        void SaveLineItem(ILineItem items);

    }
}