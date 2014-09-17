using Merchello.Core.Gateways.Payment;

namespace Merchello.Plugin.Payments.PurchaseOrder.Models
{
    public static class PurchaseOrderInfoExtensions
    {
        public static ProcessorArgumentCollection AsProcessorArgumentCollection(this PurchaseOrderFormData purchaseOrder)
        {
            return new ProcessorArgumentCollection()
            {
                { "purchaseOrderNumber", purchaseOrder.PurchaseOrderNumber }
            };
        }

        public static PurchaseOrderFormData AsPurchaseOrderFormData(this ProcessorArgumentCollection args)
        {
            return new PurchaseOrderFormData()
            {
                PurchaseOrderNumber = args.ArgValue("purchaseOrderNumber")
            };
        }

        private static string ArgValue(this ProcessorArgumentCollection args, string key)
        {
            return args.ContainsKey(key) ? args[key] : string.Empty;
        }

    }
}