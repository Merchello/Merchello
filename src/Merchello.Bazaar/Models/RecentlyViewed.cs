using Merchello.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Merchello.Bazaar.Models
{

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Merchello.Web;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Pluggable;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// Stores keys for the recently viewed collection.
    /// </summary>
    public class RecentlyViewed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyViewed"/> class.
        /// </summary>
        public RecentlyViewed()
        {
            this.Keys = new Guid[] { };
        }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        public IEnumerable<Guid> Keys { get; set; }
    }

    /// <summary>
    /// One time extension methods for the recently viewed collection.
    /// </summary>
    internal static class RecentlyViewedExtensions
    {
        /// <summary>
        /// Safely adds a recently viewed key to the CustomerContext.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="product">
        /// The product.
        /// </param>
        internal static void AddRecentKey(this ICustomerContext context, IProductContent product)
        {
            var recent = context.DeserializeRecentlyViewed();
            recent.AddKey(product.Key);
            recent.Store(context);
        }

        /// <summary>
        /// Serializes the keys to a CSV list for cookie storage.
        /// </summary>
        /// <param name="recent">
        /// The recent.
        /// </param>
        /// <param name="context">
        /// The <see cref="ICustomerContext"/>.
        /// </param>
        internal static void Store(this RecentlyViewed recent, ICustomerContext context)
        {
            context.SetValue("rviewed", string.Join(",", recent.Keys));
        }

        /// <summary>
        /// Gets the list of recently viewed items.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="siteAlias">
        /// The site alias.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static IEnumerable<ProductBoxModel> GetRecentlyViewedProducts(this ICustomerContext context, string siteAlias = "Bazaar")
        {
            var keys = context.DeserializeRecentlyViewed().Keys;

            // Get the Merchello helper
            var merchelloHelper = new MerchelloHelper();

            // Get the products as IProductContent
            var listOfIProductContent = keys.Select(
                                            x =>
                                            merchelloHelper.TypedProductContent(x))
                                                .Reverse();

            return BazaarContentHelper.GetProductBoxModels(listOfIProductContent);
        }

        /// <summary>
        /// Safely adds a key to the recently viewed collection list.
        /// </summary>
        /// <param name="recent">
        /// The recent.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="siteAlias">
        /// The site alias.
        /// </param>
        private static void AddKey(this RecentlyViewed recent, Guid key, string siteAlias = "ChairOffice")
        {
            const int count = 6;

            var keys = recent.Keys.ToList();

            if (recent.Keys.Count() >= count)
            {
                keys.ToList().RemoveAt(0);
            }

            if (!keys.Contains(key)) keys.Add(key);

            recent.Keys = keys;
        }

        /// <summary>
        /// deserialize recently viewed.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="RecentlyViewed"/>.
        /// </returns>
        private static RecentlyViewed DeserializeRecentlyViewed(this ICustomerContext context)
        {
            var value = context.GetValue("rviewed");
            return value.IsNullOrWhiteSpace() ? new RecentlyViewed() :
                new RecentlyViewed()
                {
                    Keys = value.Split(',').Select(x => new Guid(x))
                };
        }
    }
}
