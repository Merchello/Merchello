using System.Globalization;
using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Examine.Providers;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Web.Search
{
    /// <summary>
    /// Used to wire up events for Examine
    /// </summary>
    public class ExamineEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<ExamineEvents>("Initializing Merchello ProductIndex binding events");

            // Merchello registered providers
            var registeredProviders =
                ExamineManager.Instance.IndexProviderCollection.OfType<BaseMerchelloIndexer>()
                    .Count(x => x.EnableDefaultEventHandler);

            if(registeredProviders == 0)
                return;

            ProductService.Created += ProductServiceCreated;
            ProductService.Saved += ProductServiceSaved;
            ProductService.Deleted += ProductServiceDeleted;

            ProductVariantService.Created += ProductVariantServiceCreated;
            ProductVariantService.Saved += ProductVariantServiceSaved;
            ProductVariantService.Deleted += ProductVariantServiceDeleted;

        }

        /// <summary>
        /// Adds a product with all of its variants to the index if the product has an identity
        /// </summary>
        static void ProductServiceCreated(IProductService sender, Core.Events.NewEventArgs<IProduct> e)
        {
            if(e.Entity.HasIdentity) IndexProduct(e.Entity);
        }

        /// <summary>
        /// Adds or updates a product with all of its variants to the index
        /// </summary>        
        static void ProductServiceSaved(IProductService sender, SaveEventArgs<IProduct> e)
        {
            e.SavedEntities.ForEach(IndexProduct);
        }

        /// <summary>
        /// Removes a product with all of its variants from the index
        /// </summary>   
        static void ProductServiceDeleted(IProductService sender, DeleteEventArgs<IProduct> e)
        {
            e.DeletedEntities.ForEach(DeleteProductFromIndex);
        }

        /// <summary>
        /// Adds or updates a product variant to the index if the product variant has an identity
        /// </summary>
        static void ProductVariantServiceCreated(IProductVariantService sender, Core.Events.NewEventArgs<IProductVariant> e)
        {
            if(e.Entity.HasIdentity) IndexProductVariant(e.Entity);
        }

        /// <summary>
        /// Adds or updates a product variant to the index
        /// </summary>
        static void ProductVariantServiceSaved(IProductVariantService sender, SaveEventArgs<IProductVariant> e)
        {
            e.SavedEntities.ForEach(IndexProductVariant);
        }

        /// <summary>
        /// Removes a product variant from the index
        /// </summary>
        static void ProductVariantServiceDeleted(IProductVariantService sender, DeleteEventArgs<IProductVariant> e)
        {
            e.DeletedEntities.ForEach(DeleteProductVariantFromIndex);
        }


        private static void IndexProduct(IProduct product)
        {
            product.ProductVariants.ForEach(IndexProductVariant);
            IndexProductVariant(((Product)product).MasterVariant, product.ProductOptions);
        }

        private static void IndexProductVariant(IProductVariant productVariant)
        {
            IndexProductVariant(productVariant, null);
        }

        private static void IndexProductVariant(IProductVariant productVariant, ProductOptionCollection productOptions)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].ReIndexNode(productVariant.SerializeToXml(productOptions).Root, IndexTypes.ProductVariant);
        }

        private static void DeleteProductFromIndex(IProduct product)
        {
            product.ProductVariants.ForEach(DeleteProductVariantFromIndex);
            DeleteProductVariantFromIndex(((Product)product).MasterVariant);
        }

        private static void DeleteProductVariantFromIndex(IProductVariant productVariant)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].DeleteFromIndex(((ProductVariant)productVariant).ExamineId.ToString(CultureInfo.InvariantCulture));
        }
    }
}