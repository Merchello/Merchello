namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;

    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Persistence;

    public interface IVirtualContentCache<TContent, TEntity>
        where TContent : IPublishedContent
        where TEntity : IEntity

    {
        TContent GetByKey(Guid key);

        IEnumerable<TContent> GetByKeys(IEnumerable<Guid> keys);


        void ClearVirtualCache(SaveEventArgs<TEntity> e);

        void ClearVirtualCache(DeleteEventArgs<TEntity> e);

        string GetPagedQueryCacheKey(
            string methodName,
            long page,
            long itemsPerPage,
            string sortBy,
            SortDirection sortDirection,
            IDictionary<string, string> args = null);
    }
}