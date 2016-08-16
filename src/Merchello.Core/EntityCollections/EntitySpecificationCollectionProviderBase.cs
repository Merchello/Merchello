namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    public abstract class EntitySpecificationCollectionProviderBase<T> : CachedQueryableEntityCollectionProviderBase<T>, IEntitySpecificationCollectionProvider
        where T : class, IEntity
    {
        protected EntitySpecificationCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        public abstract IEnumerable<IEntityCollection> AttributeCollections { get; }
    }
}