namespace Merchello.Web.Models.Payments
{
    using System;
    using ContentEditing;

    /// <summary>
    /// The payment method display.
    /// </summary>
    public class PaymentMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the payment code.
        /// </summary>
        public string PaymentCode { get; set; }

        /// <summary>
        /// Gets or sets the authorize capture editor view.
        /// </summary>
        public DialogEditorViewDisplay AuthorizePaymentEditorView { get; set; }

        /// <summary>
        /// Gets or sets the authorize capture editor view.
        /// </summary>
        public DialogEditorViewDisplay AuthorizeCapturePaymentEditorView { get; set; }

        /// <summary>
        /// Gets or sets the void payment dialog editor view.
        /// </summary>
        public DialogEditorViewDisplay VoidPaymentEditorView { get; set; }

        /// <summary>
        /// Gets or sets the refund payment dialog editor view.
        /// </summary>
        public DialogEditorViewDisplay RefundPaymentEditorView { get; set; }

        /// <summary>
        /// Gets or sets the capture payment editor view.
        /// </summary>
        public DialogEditorViewDisplay CapturePaymentEditorView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include in payment selection.
        /// </summary>
        public bool IncludeInPaymentSelection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  or not this payment method can only be used by known customers.
        /// </summary>
        public bool RequiresCustomer { get; set; }
    }
}