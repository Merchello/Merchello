namespace Merchello.Providers.Payment.BankTransfer
{
    using Core.Gateways.Payment;

    /// <summary>
    /// Extension methods for the Processor Argument Collection.
    /// </summary>
    public static class ProcessorArgumentCollectionExtensions
    {
        /// <summary>
        /// Maps the <see cref="ProcessorArgumentCollection"/> to <see cref="PurchaseOrderFormData"/>.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="PurchaseOrderFormData"/>.
        /// </returns>
        public static BankTransferFormData AsPurchaseOrderFormData(this ProcessorArgumentCollection args)
        {
            return new BankTransferFormData
            {
                PurchaseOrderNumber = args.ArgValue(Constants.PurchaseOrder.PoStringKey)
            };
        }

        /// <summary>
        /// Gets the purchase order number from the <see cref="ProcessorArgumentCollection"/>.
        /// </summary>
        /// <param name="args">
        /// The <see cref="ProcessorArgumentCollection"/>.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ArgValue(this ProcessorArgumentCollection args, string key)
        {
            return args.ContainsKey(key) ? args[key] : string.Empty;
        }
    }
}