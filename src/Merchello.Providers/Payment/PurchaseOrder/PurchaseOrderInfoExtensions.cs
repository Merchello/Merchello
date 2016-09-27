namespace Merchello.Providers.Payment.PurchaseOrder
{
    using Core.Gateways.Payment;

    public static class PurchaseOrderInfoExtensions
    {

        public static PurchaseOrderFormData AsPurchaseOrderFormData(this ProcessorArgumentCollection args)
        {
            return new PurchaseOrderFormData
            {
                PurchaseOrderNumber = args.ArgValue(PurchaseOrderConstants.PoStringKey)
            };
        }

        private static string ArgValue(this ProcessorArgumentCollection args, string key)
        {
            return args.ContainsKey(key) ? args[key] : string.Empty;
        }

    }
}