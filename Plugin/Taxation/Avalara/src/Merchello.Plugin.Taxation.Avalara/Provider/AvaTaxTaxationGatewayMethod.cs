using Newtonsoft.Json;

namespace Merchello.Plugin.Taxation.Avalara.Provider
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

    /// <summary>
    /// Represents the Avalara taxation gateway method
    /// </summary>
    public class AvaTaxTaxationGatewayMethod : TaxationGatewayMethodBase, IAvaTaxTaxationGatewayMethod
    {
        /// <summary>
        /// The  provider settings.
        /// </summary>
        private readonly AvaTaxProviderSettings _settings;

        /// <summary>
        /// The ava tax service.
        /// </summary>
        private readonly AvaTaxService _avaTaxService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaTaxTaxationGatewayMethod"/> class.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data collection from the provider.
        /// </param>
        public AvaTaxTaxationGatewayMethod(ITaxMethod taxMethod, ExtendedDataCollection extendedData) 
            : base(taxMethod)
        {            
            _settings = extendedData.GetAvaTaxProviderSettings();

            _avaTaxService = new AvaTaxService(_settings);
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
            return CalculateTaxForInvoice(invoice, taxAddress, true);
        }

        /// <summary>
        /// The calculate tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <param name="quoteOnly">
        /// A value indicating whether or not this is a tax quote or a formal tax submission
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress, bool quoteOnly)
        {
            var defaultStoreAddress = _settings.DefaultStoreAddress;

            string prefix = invoice.InvoiceNumberPrefix;

            if (quoteOnly)
            {                
                invoice.InvoiceNumberPrefix = string.Format("Quote-{0}", Guid.NewGuid());
            }

            var request = invoice.AsTaxRequest(defaultStoreAddress, quoteOnly);
            if (quoteOnly) invoice.InvoiceNumberPrefix = prefix;

            request.CompanyCode = _settings.CompanyCode;

            var avaTaxResult = _avaTaxService.GetTax(request);

            if (avaTaxResult.ResultCode == SeverityLevel.Success)
            {
                var extendedData = new ExtendedDataCollection();

                extendedData.SetValue(Core.Constants.ExtendedDataKeys.TaxTransactionResults, JsonConvert.SerializeObject(avaTaxResult));

                return new TaxCalculationResult(TaxMethod.Name, -1, avaTaxResult.TotalTax, extendedData);
            }

            IEnumerable<ApiResponseMessage> messages;
            try
            {
                messages = avaTaxResult.Messages;
            }
            catch (Exception)
            {                    
                messages = Enumerable.Empty<ApiResponseMessage>();
            }
        
            var exception = new AvaTaxApiException(string.Format("AvaTax returned result code: {0}.  {1}", avaTaxResult.ResultCode, string.Join(Environment.NewLine, messages.Select(x => x.Details))));

            LogHelper.Error<AvaTaxTaxationGatewayMethod>("AvaTax API returned an exception", exception);

            throw exception;
        }
    }
}