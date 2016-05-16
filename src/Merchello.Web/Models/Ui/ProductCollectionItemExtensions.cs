namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;
    using Merchello.Web.Models.VirtualContent;

    public static class ProductCollectionItemExtensions
    {
        public static ProductCollection Parent(this ProductCollection value)
        {
            return value.ParentKey == null ? 
                       null : 
                       GetByKey(value.ParentKey.Value);
        }

        public static IEnumerable<ProductCollection> Children(this ProductCollection value)
        {
            return GetChildren(value);
        }

        public static IEnumerable<IProductContent> GetProducts(this ProductCollection value)
        {
            return value.GetProducts(1, long.MaxValue);
        }

        public static IEnumerable<IProductContent> GetProducts(this ProductCollection value,
                                                               long page,
                                                               long itemsPerPage,
                                                               string sortBy = "",
                                                               SortDirection sortDirection = SortDirection.Ascending)
        {
            var merchelloHelper = new MerchelloHelper();
            return value.GetProducts(merchelloHelper, page, itemsPerPage, sortBy, sortDirection);
        }

        internal static IEnumerable<IProductContent> GetProducts(this ProductCollection value,
                                                                 MerchelloHelper merchelloHelper,
                                                                 long page,
                                                                 long itemsPerPage,
                                                                 string sortBy = "",
                                                                 SortDirection sortDirection = SortDirection.Ascending)
        {
            return merchelloHelper.TypedProductContentFromCollection(
                value.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        private static IEnumerable<ProductCollection> GetChildren(this ProductCollection value)
        {
            var service = MerchelloContext.Current.Services.EntityCollectionService;
            var children = ((EntityCollectionService)service).GetChildren(value.CollectionKey);
            return children.Select(x => new ProductCollection(x));
        }

        private static ProductCollection GetByKey(Guid key)
        {
            var service = MerchelloContext.Current.Services.EntityCollectionService;
            var collection = service.GetByKey(key);

            if (collection == null) return null;
            if (collection.EntityTfKey != Core.Constants.TypeFieldKeys.Entity.ProductKey) return null;

            return new ProductCollection(collection);
        }
    }
}