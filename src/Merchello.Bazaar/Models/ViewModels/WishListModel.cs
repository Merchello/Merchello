namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The wish list model.
    /// </summary>
    public partial class WishListModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WishListModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public WishListModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the wish list table.
        /// </summary>
        public WishListTableModel WishListTable { get; set; }
    }
}