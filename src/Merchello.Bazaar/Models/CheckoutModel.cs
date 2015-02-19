namespace Merchello.Bazaar.Models
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout model.
    /// </summary>
    public class CheckoutModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CheckoutModel(IPublishedContent content)
            : base(content)
        {
        }
    }
}