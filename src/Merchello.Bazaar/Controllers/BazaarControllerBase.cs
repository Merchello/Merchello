namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Bazaar.Factories;
    using Merchello.Core.Models;
    using Merchello.Web.Mvc;
    using Merchello.Web.Pluggable;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar controller base.
    /// </summary>
    public abstract class BazaarControllerBase : MerchelloSurfaceController, IRenderMvcController
    {
        /// <summary>
        /// The <see cref="IViewModelFactory"/>.
        /// </summary>
        private Lazy<IViewModelFactory> _viewModelFactory;

        /// <summary>
        /// The current currency setting.
        /// </summary>
        private ICurrency _currency;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarControllerBase"/> class.
        /// </summary>
        protected BazaarControllerBase()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the view model factory.
        /// </summary>
        protected IViewModelFactory ViewModelFactory
        {
            get
            {
                return _viewModelFactory.Value;
            }
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public abstract ActionResult Index(RenderModel model);

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            _currency = BazaarContentHelper.GetStoreCurrency();
            _viewModelFactory = new Lazy<IViewModelFactory>(() => new ViewModelFactory(Umbraco, CurrentCustomer, _currency));
        }
    }
}