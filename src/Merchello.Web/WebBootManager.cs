namespace Merchello.Web
{
    using CoreBootManager = Merchello.Core.CoreBootManager;
    using IBootManager = Merchello.Core.IBootManager;

    /// <summary>
    /// The web boot manager.
    /// </summary>
    internal class WebBootManager : CoreBootManager
    {

        private readonly bool _isForTesting;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBootManager"/> class. 
        /// A bootstrapper for the Merchello plugin which initializes all objects including the Web portion of the plugin
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
        /// <returns></returns>
        public override IBootManager Initialize()
        {
            base.Initialize();

            // initialize the AutoMapperMappings
            AutoMapperMappings.CreateMappings();

            return this;
        }

        //protected override void InitializeResolvers()
        //{
        //    base.InitializeResolvers();

        //    ReportDataAggregatorResolver.Current = new ReportDataAggregatorResolver(PluginManager.Current.ResolveReportDataAggregators());
        //}
    }
}
