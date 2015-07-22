namespace Merchello.Bazaar.Controllers
{
    using System;

    using Merchello.Bazaar.Factories;
    using Merchello.Core;
    using Merchello.Web.Mvc;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The base controller for the Merchello Starter Kit.
    /// </summary>
    public abstract class RenderControllerBase : MerchelloRenderMvcController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        /// <remarks>
        /// TODO this is a total hack but it is a quick way to subclass MerchelloRenderMvcController
        /// Refactor for 1.9.0
        /// </remarks>
        private readonly IMerchelloContext _merchelloContext = Core.MerchelloContext.Current;

        /// <summary>
        /// The <see cref="IViewModelFactory"/>.
        /// </summary>
        private Lazy<IViewModelFactory> _viewModelFactory;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControllerBase"/> class.
        /// </summary>
        protected RenderControllerBase()
        {
            this.Initialize();
        }
        
        #endregion
        

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControllerBase"/> class.
        /// </summary>
        protected IMerchelloContext MerchelloContext
        {
            get
            {
                return _merchelloContext;
            }
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
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {          
            _viewModelFactory = new Lazy<IViewModelFactory>(() => new ViewModelFactory(Umbraco, CurrentCustomer, Currency));
        }
    }
}