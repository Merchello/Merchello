namespace Merchello.Core.Gateways.Payment
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// The payment gateway method attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PaymentGatewayMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in payment selection.
        /// </param>
        /// <remarks>
        /// TODO rename includeInPaymentSelection ... these are payments!
        /// </remarks>
        public PaymentGatewayMethodAttribute(string title, bool includeInPaymentSelection = true)
            : this(title, string.Empty, includeInPaymentSelection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeCaptureEditorView">
        /// The authorize capture editor view.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in payment selection.
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeCaptureEditorView, bool includeInPaymentSelection = true)
            : this(title, authorizeCaptureEditorView, string.Empty, includeInPaymentSelection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeCaptureEditorView">
        /// The authorize capture editor view.
        /// </param>
        /// <param name="voidPaymentEditorView">
        /// The void payment editor view.
        /// </param>
        /// <param name="includeInShipmentQuotations">
        /// The include in shipment quotations.
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeCaptureEditorView, string voidPaymentEditorView, bool includeInShipmentQuotations = true)
            : this(title, authorizeCaptureEditorView, voidPaymentEditorView, string.Empty, includeInShipmentQuotations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeCaptureEditorView">
        /// The authorize capture editor view.
        /// </param>
        /// <param name="voidPaymentEditorView">
        /// The void payment editor view.
        /// </param>
        /// <param name="refundPaymentEditorView">
        /// The refund payment editor view.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in shipment quotations.
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeCaptureEditorView, string voidPaymentEditorView, string refundPaymentEditorView, bool includeInPaymentSelection = true)
        {
            Mandate.ParameterNotNullOrEmpty(title, "title");

            Title = title;
            AuthorizeCaptureEditorView = authorizeCaptureEditorView;
            this.IncludeInPaymentSelection = includeInPaymentSelection;
            VoidPaymentEditorView = voidPaymentEditorView;
            RefundPaymentEditorView = refundPaymentEditorView;
        }


        /// <summary>
        /// Gets a title to be displayed in the back office if applicable.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the authorize capture editor view.
        /// </summary>
        public string AuthorizeCaptureEditorView { get; private set; }

        /// <summary>
        /// Gets the void payment editor view.
        /// </summary>
        public string VoidPaymentEditorView { get; private set; }

        /// <summary>
        /// Gets the refund payment editor view.
        /// </summary>
        public string RefundPaymentEditorView { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the payment method should be used when quoting shipments.
        /// If false, it is assumed that the payment method is to be used programmatically for some other purpose.
        /// </summary>
        public bool IncludeInPaymentSelection { get; private set; }

    }
}