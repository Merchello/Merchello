namespace Merchello.Bazaar.Models.ViewModels
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
        private BasketModel _basketPage;

        /// <summary>
        /// The registration page.
        /// </summary>
        private RegistrationModel _registrationPage;

        /// <summary>
        /// The account page.
        /// </summary>
        private AccountModel _accountPage;

        /// <summary>
        /// The wish list page.
        /// </summary>
        private WishListModel _wishListPage;

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

                return this._storePage;
            } 

            protected set
            {
                this._storePage = value;
            }
        }

        /// <summary>
        /// Gets or sets the basket page.
        /// </summary>
        public BasketModel BasketPage
        {
            get
            {
                return this._basketPage ?? new BasketModel(this.StorePage.Children.FirstOrDefault(x => x.DocumentTypeAlias == "BazaarBasket"))
                                               {
                                                   Currency = this.Currency,
                                                   CurrentCustomer = this.CurrentCustomer
                                               };
            }

            protected set
            {
                this._basketPage = value;
            }
        }

        /// <summary>
        /// Gets the registration page.
        /// </summary>
        public RegistrationModel RegistrationPage
        {
            get
            {
                return this._registrationPage ?? new RegistrationModel(this.StorePage.Descendant("BazaarRegistration"))
                                                {
                                                    CurrentCustomer = this.CurrentCustomer,
                                                    Currency = this.Currency
                                                };
            }
        }

        /// <summary>
        /// Gets the account page.
        /// </summary>
        public AccountModel AccountPage
        {
            get
            {
                return _accountPage ?? new AccountModel(StorePage.Descendant("BazaarAccount"))
                                           {
                                             Currency  = Currency,
                                             CurrentCustomer = CurrentCustomer
                                           };
            }
        }

        /// <summary>
        /// Gets the wish list page.
        /// </summary>
        public WishListModel WishListPage
        {
            get
            {
                return _wishListPage
                       ?? new WishListModel(StorePage.Descendant("BazaarWishList"))
                              {
                                  CurrentCustomer = CurrentCustomer,
                                  Currency = Currency
                              };
            }
        }

        /// <summary>
        /// Gets the product groups.
        /// </summary>
        public IEnumerable<ProductGroupModel> ProductGroups
        {
            get
            {
                return this._productGroups
                       ?? this.StorePage.Children.Where(x => x.DocumentTypeAlias == "BazaarProductGroup" && x.IsVisible())
                              .Select(x => new ProductGroupModel(x)
                                               {
                                                   CurrentCustomer = this.CurrentCustomer,
                                                   Currency = this.Currency
                                               });
            }
        }

        /// <summary>
        /// Gets a value indicating whether show account.
        /// </summary>
        public bool ShowAccount 
        {
            get
            {
                return StorePage.GetPropertyValue<bool>("customerAccounts");
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the wish list.
        /// </summary>
        public bool ShowWishList
        {
            get
            {
                return StorePage.GetPropertyValue<bool>("enableWishList");
            }
            
        }

        /// <summary>
        /// Gets the customer member type name.
        /// </summary>
        public string CustomerMemberTypeName
        {
            get
            {
                return StorePage.GetPropertyValue<string>("customerMemberType");
            }
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public ICurrency Currency { get; internal set; }
    }
}