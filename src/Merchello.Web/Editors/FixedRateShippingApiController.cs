namespace Merchello.Web.Editors
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways.Shipping.FixedRate;
    using Models.ContentEditing;    
    using Umbraco.Web.Mvc;
    using WebApi;

    /// <summary>
    /// The fixed rate shipping api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class FixedRateShippingApiController : MerchelloApiController
    {
        #region Fields


        /// <summary>
        /// The fixed rate shipping gateway provider.
        /// </summary>
        private readonly FixedRateShippingGatewayProvider _fixedRateShippingGatewayProvider;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateShippingApiController"/> class.
        /// </summary>
        public FixedRateShippingApiController()
            : this(MerchelloContext.Current)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateShippingApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public FixedRateShippingApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _fixedRateShippingGatewayProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Gateways.Shipping.GetProviderByKey(Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);
        }

        /// <summary>
        /// The get ship fixed rate table.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// The <see cref="ShipFixedRateTableDisplay"/>.
        /// </returns>
        /// <exception cref="HttpResponseException">
        /// Throws a not found exception if fixed rate method was not found
        /// </exception>
        [HttpPost, HttpGet]
        public ShipFixedRateTableDisplay GetShipFixedRateTable(ShipMethodDisplay method)
        {
            var fixedMethod = (IFixedRateShippingGatewayMethod)_fixedRateShippingGatewayProvider.GetShippingGatewayMethod(method.Key, method.ShipCountryKey);

            if (fixedMethod == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return fixedMethod.RateTable.ToShipFixedRateTableDisplay();
        }

        /// <summary>
        /// The put ship fixed rate table.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// The <see cref="ShipFixedRateTableDisplay"/>.
        /// </returns>
        public ShipFixedRateTableDisplay PutShipFixedRateTable(RateTableShipMethodDisplay method)
        {
            var fixedMethod = (IFixedRateShippingGatewayMethod)_fixedRateShippingGatewayProvider.GetShippingGatewayMethod(method.Key, method.ShipCountryKey);

            fixedMethod = method.ToFixedRateShipMethod(fixedMethod);

            _fixedRateShippingGatewayProvider.SaveShippingGatewayMethod(fixedMethod);

            return fixedMethod.RateTable.ToShipFixedRateTableDisplay();
        }
    }
}