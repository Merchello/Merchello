using Newtonsoft.Json;

namespace Merchello.Plugin.Taxation.Taxjar.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Gateways.Taxation;
    using Core.Models;
    using Core.Services;
    using Models;
    using Services; 
    using Umbraco.Core.Logging;

    public class TaxJarTaxationGatewayMethod : TaxationGatewayMethodBase, ITaxJarTaxationGatewayMethod
    {
        /// <summary>
        /// The  provider settings.
        /// </summary>
        private readonly TaxJarProviderSettings _settings;

                /// <summary>
        /// The tax service.
        /// </summary>
        private readonly TaxJarTaxService _taxjarService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxJarTaxationGatewayMethod"/> class.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data collection from the provider.
        /// </param>
        public TaxJarTaxationGatewayMethod(ITaxMethod taxMethod, ExtendedDataCollection extendedData) 
            : base(taxMethod)
        {
            _settings = extendedData.GetTaxJarProviderSettings();

            _taxjarService = new TaxJarTaxService(_settings);
        }

        /// <summary>
        /// Calculates tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public override ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            decimal amount = 0m;
            foreach (var item in invoice.Items)
            {
                // can I use?: https://github.com/Merchello/Merchello/blob/5706b8c9466f7417c41fdd29de7930b3e8c4dd2d/src/Merchello.Core/Models/ExtendedDataExtensions.cs#L287-L295
                if (item.ExtendedData.GetTaxableValue())
                    amount = amount + item.TotalPrice;
            }

            TaxRequest taxRequest = new TaxRequest();
            taxRequest.Amount = amount;
            taxRequest.Shipping = invoice.TotalShipping();
            taxRequest.ToCity = taxAddress.Locality;
            taxRequest.ToCountry = taxAddress.CountryCode;
            taxRequest.ToState = taxAddress.Region;
            taxRequest.ToZip = taxAddress.PostalCode;

            Models.TaxResult taxResult = _taxjarService.GetTax(taxRequest);

            if (taxResult.Success)
            {
                var extendedData = new ExtendedDataCollection();

                extendedData.SetValue(Core.Constants.ExtendedDataKeys.TaxTransactionResults, JsonConvert.SerializeObject(taxResult));

                return new TaxCalculationResult(TaxMethod.Name, taxResult.Rate, taxResult.TotalTax, extendedData);
            }

            throw new Exception("TaxJar.com error");
        }
    }
}
