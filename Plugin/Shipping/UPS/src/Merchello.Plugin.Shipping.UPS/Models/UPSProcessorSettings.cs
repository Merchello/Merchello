using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Shipping.UPS.Models
{
    public class UPSProcessorSettings
    {        
        public bool UseSandbox { get; set; }

        public UPSProcessorSettings()
        {
            // set the defaults
            UpsPickupTypeCode = "01";
            UpsCustomerClassification = "04";
            UpsPackagingTypeCode = "02";
            UpsAdditionalHandlingCharge = "0.00";
        }

        public string UpsAccessKey { get; set; }
        public string UpsUserName { get; set; }
        public string UpsPassword { get; set; }
        public string UpsPickupTypeCode  { get; set; }
        public string UpsCustomerClassification  { get; set; }
        public string UpsPackagingTypeCode { get; set; }
        public string UpsAdditionalHandlingCharge { get; set; }

        public string ApiVersion
        {
            get { return UpsShippingProcessor.ApiVersion; }
        }
    }
}
