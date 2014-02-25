using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    public class FixRateMethod : TaxationGatewayMethodBase, IFixedRateTaxMethod
    {
        public FixRateMethod(ITaxMethod taxMethod) 
            : base(taxMethod)
        { }

        public override ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            
            var ctrValues = new object[] { invoice, taxAddress, TaxMethod };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultInvoiceTaxRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<TaxCalculationStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<FixedRateTaxationGatewayProvider>("Failed to instantiate the tax calculation strategy '" + typeName + "'", attempt.Exception);
                throw attempt.Exception;
            }

            return CalculateTaxForInvoice(attempt.Result);
        }
    }
}