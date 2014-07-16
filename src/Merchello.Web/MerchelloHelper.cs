namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Examine.SearchCriteria;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    public class MerchelloHelper
    {
        
        #region Product

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay Product(string key)
        {
            return ProductQuery.GetByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay Product(Guid key)
        {
            return ProductQuery.GetByKey(key);
        }

        /// <summary>
        /// Returns a collection of all <see cref="ProductDisplay"/>
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="ProductDisplay"/> found in the index.
        /// </returns>
        public IEnumerable<ProductDisplay> AllProducts()
        {
            return ProductQuery.GetAllProducts();
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public ProductVariantDisplay ProductVariant(string key)
        {
            return ProductQuery.GetVariantDisplayByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public ProductVariantDisplay ProductVariant(Guid key)
        {
            return ProductVariant(key.ToString());
        }


        /// <summary>
        /// Get a product variant from a product by it's collection of attributes
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The option choices (attributeKeys)</param>
        /// <returns>The <see cref="ProductVariantDisplay"/></returns>
        public ProductVariantDisplay GetProductVariantWithAttributes(Guid productKey, Guid[] attributeKeys)
        {
            var product = Product(productKey);
            return product.ProductVariants.FirstOrDefault(x => x.Attributes.Count() == attributeKeys.Count() && attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Gets a list of valid variants based on partial attribute selection
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The selected option choices</param>
        /// <returns>A collection of <see cref="ProductVariantDisplay"/></returns>
        /// <remarks>
        /// Intended to assist in product variant selection 
        /// </remarks>
        public IEnumerable<ProductVariantDisplay> GetValidProductVariants(Guid productKey, Guid[] attributeKeys)
        {
            var product = Product(productKey);
            if (product == null) throw new InvalidOperationException("Product is null");
            if (!attributeKeys.Any()) return product.ProductVariants;

            var variants = product.ProductVariants.Where(x => attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));

            return variants;
        }

        /// <summary>
        /// Searches the Merchello Product index.  NOTE:  This returns a ProductDisplay and is not a Content search.  Use the the UmbracoHelper.Search for content searches.
        /// </summary>
        /// <param name="term">The search term</param>
        /// <returns>The collection of <see cref="ProductDisplay"/></returns>
        public IEnumerable<ProductDisplay> SearchProducts(string term)
        {
            return ProductQuery.Search(term);
        }

        /// <summary>
        /// Searches the Merchello Product index.  NOTE:  This returns a ProductDisplay and is not a Content search.  Use the the UmbracoHelper.Search for content searches.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// <returns>The collection of <see cref="ProductDisplay"/></returns>
        /// </returns>
        public IEnumerable<ProductDisplay> SearchProducts(ISearchCriteria criteria)
        {
            return ProductQuery.Search(criteria);
        }

        #endregion

        #region Invoice

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> from the Merchello Invoice index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        public InvoiceDisplay Invoice(Guid key)
        {
            return InvoiceQuery.GetByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> from the Merchello Invoice index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        public InvoiceDisplay Invoice(string key)
        {
            return InvoiceQuery.GetByKey(key);
        }

        /// <summary>
        /// Gets a collection of all invoices
        /// </summary>
        /// <returns>
        /// <returns>The collection of all <see cref="InvoiceDisplay"/></returns>
        /// </returns>
        public IEnumerable<InvoiceDisplay> AllInvoices()
        {
            return InvoiceQuery.GetAllInvoices();
        }

        /// <summary>
        /// Searches the Merchello Invoice index. 
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// The collection of <see cref="InvoiceDisplay"/>.
        /// </returns>
        public IEnumerable<InvoiceDisplay> SearchInvoices(string term)
        {
            return InvoiceQuery.Search(term);
        }

        /// <summary>
        /// Searches the Merchello Invoice index. 
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The collection of all <see cref="InvoiceDisplay"/> matching the criteria.
        /// </returns>
        public IEnumerable<InvoiceDisplay> SearchInvoices(ISearchCriteria criteria)
        {
            return InvoiceQuery.Search(criteria);
        }

        #endregion

        #region Customers

        /// <summary>
        /// The customer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        public CustomerDisplay Customer(string key)
        {
            return CustomerQuery.GetByKey(key);
        }

        /// <summary>
        /// The customer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        public CustomerDisplay Customer(Guid key)
        {
            return Customer(key.ToString());
        }

        /// <summary>
        /// The all customers.
        /// </summary>
        /// <returns>
        /// The collection of all customers.
        /// </returns>
        public IEnumerable<CustomerDisplay> AllCustomers()
        {
            return CustomerQuery.GetAllCustomers();
        }

        #endregion
    }

}