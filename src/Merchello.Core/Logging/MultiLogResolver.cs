namespace Merchello.Core.Logging
{
    using Merchello.Core.Acquired.ObjectResolution;

    /// <summary>
    /// The multi logger resolver.
    /// </summary>
    internal sealed class MultiLogResolver : SingleObjectResolverBase<MultiLogResolver, IMultiLogger>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogResolver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MultiLogResolver(IMultiLogger logger)
            : base(logger)
        {            
        }

        /// <summary>
        /// Gets the current logger
        /// </summary>
        public IMultiLogger Logger
        {
            get { return Value; }
        }

        /// <summary>
        /// Method allowing to change the logger during startup
        /// </summary>
        /// <param name="logger">
        /// The <see cref="IMultiLogger"/>
        /// </param>
        internal void SetLogger(IMultiLogger logger)
        {
            Value = logger;
        }
    }
}