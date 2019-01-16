namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.SqlSyntax;

    using UnitOfWork;

    /// <summary>
    /// A line item repository base class
    /// </summary>
    /// <typeparam name="T">The type T of the <see cref="ILineItem"/></typeparam>
    internal abstract class LineItemRepositoryBase<T> : MerchelloPetaPocoRepositoryBase<T>, ILineItemRepositoryBase<T> where T : class, ILineItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemRepositoryBase{T}"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        protected LineItemRepositoryBase(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Gets a collection of all <see cref="ILineItem"/> for a container
        /// </summary>
        /// <param name="containerKey">The key of the container.  ex. Invoice.Key, ItemCache.Key</param>
        /// <returns>
        /// A collection of <see cref="ILineItem"/> as Type T
        /// </returns>
        public virtual IEnumerable<T> GetByContainerKey(Guid containerKey)
        {
            var query = Querying.Query<T>.Builder.Where(x => x.ContainerKey == containerKey);
            return PerformGetByQuery(query);
        }

        /// <summary>
        /// Saves a collection of <see cref="ILineItem"/> associated with a container
        /// </summary>
        /// <param name="items">The collection of <see cref="ILineItem"/></param>
        /// <param name="containerKey">The "Container" or parent collection key</param>
        public virtual void SaveLineItem(LineItemCollection items, Guid containerKey)
        {          
            var existing = GetByContainerKey(containerKey);

            // assert there are no existing items not in the new set of items.  If there are ... delete them
            var toDelete = existing.Where(x => items.All(item => item.Key != x.Key)).ToArray();
            if (toDelete.Any())
            {
                foreach (var d in toDelete)
                {
                    Delete(d);
                }
            }

            foreach (var item in items)
            {
                // In the mapping between different line item types the container key is 
                // invalidated so we need to set it to the current container.
                if (!item.ContainerKey.Equals(containerKey)) item.ContainerKey = containerKey;

                SaveLineItem(item as T);
            }
        }

        /// <summary>
        /// Saves a <see cref="ILineItem"/>
        /// </summary>
        /// <param name="item">The <see cref="ILineItem"/> to be saved</param>
        public virtual void SaveLineItem(T item)
        {
            if (!item.HasIdentity)
            {
                PersistNewItem(item);
            }
            else
            {
                PersistUpdatedItem(item);
            }            
        }
    }
}