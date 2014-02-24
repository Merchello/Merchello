using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    public class FixRateTaxMethod : GatewayTaxMethodBase, IFixedRateTaxMethod
    {
        public FixRateTaxMethod(Models.IGatewayTaxMethod gatewayTaxMethod) 
            : base(gatewayTaxMethod)
        { }

        public override IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            
            var ctrValues = new object[] { invoice, taxAddress, GatewayTaxMethod };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultInvoiceTaxRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<InvoiceTaxationStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<FixedRateTaxationGatewayProvider>("Failed to instantiate the tax rate quote strategy '" + typeName + "'", attempt.Exception);
                throw attempt.Exception;
            }

            return CalculateTaxForInvoice(attempt.Result);
        }
    }
}