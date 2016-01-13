namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// Represents the store model.
    /// </summary>
    public partial class StoreModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public StoreModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets the tag line.
        /// </summary>
        public string TagLine 
        { 
            get
            {
                return this.Content.GetPropertyValue<string>("tagLine");
            } 
        }
    }
}