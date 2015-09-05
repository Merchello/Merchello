namespace Merchello.Web.Routing
{
    using Merchello.Web.Models.DetachedContent;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Web.Routing;

    /// <summary>
    /// Responsible for finding ProductContent by it's slug.
    /// </summary>
    public class ContentFinderProductBySlug : IContentFinder
    {
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
            
            return false;                            
        }
    }
}