namespace Merchello.Plugin.Payments.Epay.Services
{
    using Merchello.Core;
    using Merchello.Plugin.Payments.Epay.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for EpayApiServices.
    /// </summary>
    internal abstract class EpayApiServiceBase
    {

        /// <summary>
        /// The <see cref="EpayProcessorSettings"/>.
        /// </summary>
        private readonly EpayProcessorSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpayApiServiceBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected EpayApiServiceBase(IMerchelloContext merchelloContext, EpayProcessorSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            MerchelloContext = merchelloContext;
            _settings = settings;
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }
    }
}