namespace Merchello.Plugin.Shipping.FedEx.Models
{
    public class FedExProcessorSettings
    {        
        public bool UseSandbox { get; set; }

        public FedExProcessorSettings()
        {
            // set the defaults
            FedExPickupTypeCode = "01";
            FedExCustomerClassification = "04";
            FedExPackagingTypeCode = "02";
            FedExAdditionalHandlingCharge = "0.00";
        }

        public string FedExKey { get; set; }
        public string FedExPassword { get; set; }

        public string FedExPickupTypeCode { get; set; }
        public string FedExCustomerClassification  { get; set; }
        public string FedExPackagingTypeCode { get; set; }
        public string FedExAdditionalHandlingCharge { get; set; }

        public string ApiVersion
        {
            get { return FedExShippingProcessor.ApiVersion; }
        }
    }
}
