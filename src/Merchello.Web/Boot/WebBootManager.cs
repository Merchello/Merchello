namespace Merchello.Web.Boot
{
    using System;

    using Merchello.Core.Boot;
    using Merchello.Core.Logging;
    using IBootManager = Merchello.Core.Boot.IBootManager;

    /// <summary>
    /// The web boot manager.
    /// </summary>
    internal class WebBootManager : CoreBootManager
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
            base.Initialize();

            return this;
        }

        /// <summary>
        /// Initializer resolvers.
        /// </summary>
        protected override void InitializeResolvers()
        {
            base.InitializeResolvers();
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
