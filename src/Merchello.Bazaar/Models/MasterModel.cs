namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.Linq;

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
        private StoreModel _storePage;

        /// <summary>
        /// The basket page.
        /// </summary>
        private IPublishedContent _basketPage;

        /// <summary>
        /// The product groups or categories.
        /// </summary>
        private IEnumerable<ProductGroupModel> _productGroups; 

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
        }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme 
        {
            get
            {
                return this._theme ?? this.StorePage.GetPropertyValue<string>("themePicker");
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
                return this._storeTitle ?? this.StorePage.GetPropertyValue<string>("storeTitle");
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
        /// Gets or sets the store page.
        /// </summary>
        public StoreModel StorePage
        {
            get
            {
                var store = this.Content.AncestorOrSelf("BazaarStore");
                if (store != null) 
                {
                    this._storePage = new StoreModel(store)
                                        {
                                            CurrentCustomer = this.CurrentCustomer
                                        };
                }

                return _storePage;
            } 

            protected set
            {
                _storePage = value;
            }
        }

        /// <summary>
        /// Gets or sets the basket page.
        /// </summary>
        public IPublishedContent BasketPage
        {
            get
            {
                return this._basketPage ?? this.StorePage.Children.FirstOrDefault(x => x.DocumentTypeAlias == "BazaarBasket");
            }

            protected set
            {
                this._basketPage = value;
            }
        }

        /// <summary>
        /// Gets the product groups.
        /// </summary>
        public IEnumerable<ProductGroupModel> ProductGroups
        {
            get
            {
                return _productGroups
                       ?? this.StorePage.Children.Where(x => x.DocumentTypeAlias == "BazaarProductGroup")
                              .Select(x => new ProductGroupModel(x)
                                               {
                                                   CurrentCustomer = this.CurrentCustomer,
                                                   Currency = this.Currency
                                               });
            }
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public ICurrency Currency { get; internal set; }
    }
}