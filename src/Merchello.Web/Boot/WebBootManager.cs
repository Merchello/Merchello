namespace Merchello.Web.Boot
{
    using System;

    using Merchello.Core.Boot;
    using Merchello.Core.Logging;
    //using Merchello.Core;
    //using Merchello.Core.Logging;
    //using Merchello.Core.Marketing.Offer;
    //using Merchello.Web.Pluggable;
    //using Merchello.Web.Reporting;
    //using Merchello.Web.Ui;

    //using Umbraco.Core;
    //using Umbraco.Core.Logging;
    //using Umbraco.Core.Persistence.SqlSyntax;
    using IBootManager = Merchello.Core.Boot.IBootManager;

    /// <summary>
    /// The web boot manager.
    /// </summary>
    internal class WebBootManager : Core.Boot.CoreBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebBootManager"/> class. 
        /// </summary>
        public WebBootManager()
            : base(new CoreBootSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBootManager"/> class. 
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IWebBootSettings"/>.
        /// </param>
        internal WebBootManager(IWebBootSettings settings)
            : base(settings)
        {
        }


        /// <summary>
        /// Initialize objects before anything during the boot cycle happens
        /// </summary>
        /// <returns>The <see cref="IBootManager"/></returns>
        public override IBootManager Initialize()
        {
            this.EnsureDatabase();

            base.Initialize();

            // initialize the AutoMapperMappings
            //AutoMapperMappings.CreateMappings();

            return this;
        }

        /// <summary>
        /// The ensure database.
        /// </summary>
        protected void EnsureDatabase()
        {
            //Logger.Info<WebBootManager>("Verifying Merchello Database is present.");
            //var database = GetDatabase();
            //var manager = new WebMigrationManager(database, SqlSyntax, Logger);
            //if (!manager.EnsureDatabase())
            //{
            //    Logger.Info<WebBootManager>("Merchello database tables installed");
            //}
        }

        /// <summary>
        /// Initializer resolvers.
        /// </summary>
        protected override void InitializeResolvers()
        {
            base.InitializeResolvers();

            //if (!PaymentMethodUiControllerResolver.HasCurrent)
            //PaymentMethodUiControllerResolver.Current = new PaymentMethodUiControllerResolver(PluginManager.Current.ResolvePaymentMethodUiControllers());

            //if (!ReportApiControllerResolver.HasCurrent)
            //ReportApiControllerResolver.Current = new ReportApiControllerResolver(PluginManager.Current.ResolveReportApiControllers());

            //if (!OfferProviderResolver.HasCurrent)
            //    OfferProviderResolver.Current = new OfferProviderResolver(PluginManager.Current.ResolveOfferProviders(), MerchelloContext.Current.Services.OfferSettingsService);

            //if(!OfferComponentResolver.HasCurrent)
            //    OfferComponentResolver.Current = new OfferComponentResolver(PluginManager.Current.ResolveOfferComponents(), OfferProviderResolver.Current);
        }

        /// <summary>
        /// Overrides the base GetMultiLogger.
        /// </summary>
        /// <returns>
        /// The <see cref="IMultiLogger"/>.
        /// </returns>
        protected override IMultiLogger GetMultiLogger(ILogger logger)
        {
            return base.GetMultiLogger(logger);
            //try
            //{
            //    var remoteLogger = PluggableObjectHelper.GetInstance<RemoteLoggerBase>("RemoteLogger");
            //    return new MultiLogger(Logger, remoteLogger);
            //}
            //catch (Exception ex)
            //{
            //    Logger.WarnWithException<WebBootManager>("Failed to instantiate remote logger. Returning default logger", ex, () => new object[] { });
            //    return new MultiLogger();
            //}
        }
    }
}
