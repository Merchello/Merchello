namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The entity collection extensions.
    /// </summary>
    public static class EntityCollectionExtensions
    {
        /// <summary>
        /// Gets the <see cref="EntityType"/> managed by the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityType"/>.
        /// </returns>
        public static EntityType EntityType(this IEntityCollection collection)
        {
            return EnumTypeFieldConverter.EntityType.GetTypeField(collection.EntityTfKey);
        }

        /// <summary>
        /// Resolves the provider for the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public static EntityCollectionProviderBase ResolveProvider(this IEntityCollection collection)
        {
            if (!EntityCollectionProviderResolver.HasCurrent) return null;
 
            var attempt = EntityCollectionProviderResolver.Current.GetProviderForCollection(collection.Key);

            if (attempt.Success) return attempt.Result;
            
            LogHelper.Error(typeof(EntityCollectionExtensions), "Resolver failed to resolve collection provider", attempt.Exception);
            return null;
        }

        /// <summary>
        /// The resolve provider.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <typeparam name="T">
        /// The type of provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public static T ResolveProvider<T>(this IEntityCollection collection)
            where T : class
        {
            var provider = collection.ResolveProvider();
            
            if (provider == null) return null;
            
            if (provider is T) return provider as T;
            
            LogHelper.Debug(typeof(EntityCollectionExtensions), "Provider was resolved but was not of expected type.");

            return null;
        }

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="T">
        /// The type of the <see cref="IEntity"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists<T>(this IEntityCollection collection, T entity) where T : IEntity
        {
            var resolved = collection.ResolveValidatedProvider(entity);

            return resolved != null && resolved.Exists(entity);
        }

        /// <summary>
        /// The child collections.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEnityCollection}"/>.
        /// </returns>
        public static IEnumerable<IEntityCollection> ChildCollections(this IEntityCollection collection)
        {
            return !MerchelloContext.HasCurrent ? 
                Enumerable.Empty<IEntityCollection>() : 
                MerchelloContext.Current.Services.EntityCollectionService.GetChildren(collection.Key);
        }

        ///// <summary>
        ///// The save as child of.
        ///// </summary>
        ///// <param name="collection">
        ///// The collection.
        ///// </param>
        ///// <param name="parent">
        ///// The parent.
        ///// </param>
        //internal static void SetParent(this IEntityCollection collection, IEntityCollection parent)
        //{
        //    if (!MerchelloContext.HasCurrent) return;

        //    collection.ParentKey = parent.Key;

        //    MerchelloContext.Current.Services.EntityCollectionService.Save(collection);
        //}

        ///// <summary>
        ///// Sets the parent to root
        ///// </summary>
        ///// <param name="collection">
        ///// The collection.
        ///// </param>
        //internal static void SetParent(this IEntityCollection collection)
        //{
        //    if (collection.ParentKey == null) return;
        //    if (!MerchelloContext.HasCurrent) return;
        //    collection.ParentKey = null;
        //    MerchelloContext.Current.Services.EntityCollectionService.Save(collection);
        //}

        /// <summary>
        /// The resolve validated provider.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="T">
        /// The type of the <see cref="IEntity"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        internal static EntityCollectionProviderBase<T> ResolveValidatedProvider<T>(this IEntityCollection collection, T entity) where T : IEntity
        {
            var resolved = collection.ResolveProvider();

            return resolved is EntityCollectionProviderBase<T> ? resolved as EntityCollectionProviderBase<T> : null;
        }

        /// <summary>
        /// Gets the actual System.Type of the entities in the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        internal static Type TypeOfEntities(this IEntityCollection collection)
        {
            switch (collection.EntityType())
            {
                case Core.EntityType.Customer:
                    return typeof(ICustomer);
                case Core.EntityType.EntityCollection:
                    return typeof(IEntityCollection);
                case Core.EntityType.GatewayProvider:
                    return typeof(IGatewayProviderSettings);
                case Core.EntityType.Invoice:
                    return typeof(IInvoice);
                case Core.EntityType.ItemCache:
                    return typeof(IItemCache);
                case Core.EntityType.Order:
                    return typeof(IOrder);
                case Core.EntityType.Payment:
                    return typeof(IPayment);
                case Core.EntityType.Product:
                    return typeof(IProduct);
                case Core.EntityType.Shipment:
                    return typeof(IShipment);
                case Core.EntityType.Warehouse:
                    return typeof(IWarehouse);
                case Core.EntityType.WarehouseCatalog:
                    return typeof(IWarehouseCatalog);
                default:
                    return typeof(object);                    
            }
        }

        #region Examine


        #endregion
    }
}