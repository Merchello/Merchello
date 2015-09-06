namespace Merchello.Web.Routing
{
    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Web.Routing;

    /// <summary>
    /// Responsible for finding ProductContent by it's slug.
    /// </summary>
    public class ContentFinderProductBySlug : IContentFinder
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private static readonly MerchelloHelper Merchello = new MerchelloHelper(MerchelloContext.Current.Services);

        /// <summary>
        /// The factory.
        /// </summary>
        private static readonly ProductContentFactory Factory = new ProductContentFactory();

        /// <summary>
        /// Tries to find a <see cref="IProductContent"/> by it's unique slug.
        /// </summary>
        /// <param name="contentRequest">
        /// The content request.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the product was found by slug.
        /// </returns>        
        public bool TryFindContent(PublishedContentRequest contentRequest)
        {
            if (contentRequest.Uri.AbsolutePath == "/") return false;

            var slug = contentRequest.Uri.AbsolutePath.EnsureNotStartsOrEndsWith('/');

            var display = Merchello.Query.Product.GetBySlug(slug);

            if (display == null) return false;

            contentRequest.PublishedContent = Factory.BuildContent(display, contentRequest.Culture.Name);

            return true;
        }
    }
}