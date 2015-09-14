namespace Merchello.Bazaar.Models.ViewModels
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web;

    /// <summary>
    /// The master model.
    /// </summary>
    public abstract class MasterModel : PublishedContentWrapped, IMasterModel
    {
        #region Fields

        /// <summary>
        /// The root store page.
        /// </summary>
        private Lazy<IPublishedContent> _storePage;

        /// <summary>
        /// The Theme.
        /// </summary>
        private string _theme;

        /// <summary>
        /// The store title.
        /// </summary>
        private string _storeTitle;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The base <see cref="IPublishedContent"/>.
        /// </param>
        protected MasterModel(IPublishedContent content)
            : base(content)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme 
        {
            get
            {
                return this._theme ?? _storePage.Value.GetPropertyValue<string>("themePicker");
            }

            protected set
            {
                this._theme = value;
            }
        }


        /// <summary>
        /// Gets or sets the store title.
        /// </summary>
        public string StoreTitle
        {
            get
            {
                return this._storeTitle ?? _storePage.Value.GetPropertyValue<string>("storeTitle");
            }

            set
            {
                this._storeTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the current customer.
        /// </summary>
        public ICustomerBase CurrentCustomer { get; set; }

        /// <summary>
        /// Gets a value indicating whether show account.
        /// </summary>
        public bool ShowAccount 
        {
            get
            {
                return _storePage.Value.GetPropertyValue<bool>("customerAccounts");
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the wish list.
        /// </summary>
        public bool ShowWishList
        {
            get
            {
                return _storePage.Value.GetPropertyValue<bool>("enableWishList");
            }
        }

        /// <summary>
        /// Gets the customer member type name.
        /// </summary>
        public string CustomerMemberTypeName
        {
            get
            {
                return _storePage.Value.GetPropertyValue<string>("customerMemberType");
            }
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public ICurrency Currency { get; internal set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            _storePage = new Lazy<IPublishedContent>(BazaarContentHelper.GetStoreRoot);
        }
    }
}