namespace Merchello.Web
{
    using Merchello.Web.Mvc;
    using Merchello.Web.Reporting;
    using Merchello.Web.Ui;

    using Umbraco.Core;

    using CoreBootManager = Merchello.Core.CoreBootManager;
    using IBootManager = Merchello.Core.IBootManager;

    /// <summary>
    /// The web boot manager.
    /// </summary>
    internal class WebBootManager : CoreBootManager
    {
        /// <summary>
        /// Designates if this boot manager is being used by a Test
        /// </summary>
        private readonly bool _isForTesting;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBootManager"/> class. 
        /// A boot strap class for the Merchello plugin which initializes all objects including the Web portion of the plugin
        /// </summary>
        public WebBootManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBootManager"/> class. 
        /// Constructor for unit tests, ensures some resolvers are not initialized
        /// </summary>
        /// <param name="isForTesting">
        /// The is For Testing.
        /// </param>
        internal WebBootManager(bool isForTesting = false)
        {
            _isForTesting = isForTesting;
        }


        /// <summary>
        /// Initialize objects before anything during the boot cycle happens
        /// </summary>
        /// <returns>The <see cref="IBootManager"/></returns>
        public override IBootManager Initialize()
        {
            base.Initialize();

            // initialize the AutoMapperMappings
            AutoMapperMappings.CreateMappings();

            return this;
        }

        /// <summary>
        /// Initializer resolvers.
        /// </summary>
        protected override void InitializeResolvers()
        {
            base.InitializeResolvers();

            if (!PaymentMethodUiControllerResolver.HasCurrent)
            PaymentMethodUiControllerResolver.Current = new PaymentMethodUiControllerResolver(PluginManager.Current.ResolveCheckoutOperationControllers());

            if (!ReportApiControllerResolver.HasCurrent)
            ReportApiControllerResolver.Current = new ReportApiControllerResolver(PluginManager.Current.ResolveReportApiControllers());
        }
    }
}
