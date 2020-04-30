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
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        /// <remarks>
        /// TODO rename includeInPaymentSelection ... these are payments!
        /// </remarks>
        public PaymentGatewayMethodAttribute(string title, bool includeInPaymentSelection = true, bool requiresCustomer = false)
            : this(title, string.Empty, includeInPaymentSelection, requiresCustomer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeEditorView">
        /// The authorize editor view.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in payment selection.
        /// </param>
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeEditorView, bool includeInPaymentSelection = true, bool requiresCustomer = false)
            : this(title, authorizeEditorView, string.Empty, string.Empty, includeInPaymentSelection, requiresCustomer)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeEditorView">
        /// The authorize editor view.
        /// </param>
        /// <param name="authorizeCaptureEditorView">
        /// The authorize capture editor view.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in payment selection.
        /// </param>
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeEditorView, string authorizeCaptureEditorView, bool includeInPaymentSelection = true, bool requiresCustomer = false)
            : this(title, authorizeEditorView, authorizeCaptureEditorView, string.Empty, includeInPaymentSelection, requiresCustomer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeEditorView">
        /// The authorize Editor View.
        /// </param>
        /// <param name="authorizeCaptureEditorView">
        /// The authorize capture editor view.
        /// </param>
        /// <param name="voidPaymentEditorView">
        /// The void payment editor view.
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in payment selection.
        /// </param>
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeEditorView, string authorizeCaptureEditorView, string voidPaymentEditorView, bool includeInPaymentSelection = true, bool requiresCustomer = false)
            : this(title, authorizeEditorView, authorizeCaptureEditorView, voidPaymentEditorView, string.Empty, includeInPaymentSelection, requiresCustomer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeEditorView">
        /// The authorize Editor View.
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
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeEditorView, string authorizeCaptureEditorView, string voidPaymentEditorView, string refundPaymentEditorView, bool includeInPaymentSelection = true, bool requiresCustomer = false)
            : this(title, authorizeEditorView, authorizeCaptureEditorView, voidPaymentEditorView, refundPaymentEditorView, string.Empty, includeInPaymentSelection, requiresCustomer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="authorizeEditorView">
        /// The authorize Editor View.
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
        /// <param name="capturePaymentEditorView">
        /// The capture payment editor view
        /// </param>
        /// <param name="includeInPaymentSelection">
        /// The include in shipment quotations.
        /// </param>
        /// <param name="requiresCustomer">
        /// A value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </param>
        public PaymentGatewayMethodAttribute(string title, string authorizeEditorView, string authorizeCaptureEditorView, string voidPaymentEditorView, string refundPaymentEditorView, string capturePaymentEditorView, bool includeInPaymentSelection = true, bool requiresCustomer = false)
        {
            Ensure.ParameterNotNullOrEmpty(title, "title");

            Title = title;
            AuthorizeEditorView = authorizeEditorView;
            AuthorizeCaptureEditorView = authorizeCaptureEditorView;
            IncludeInPaymentSelection = includeInPaymentSelection;
            VoidPaymentEditorView = voidPaymentEditorView;
            RefundPaymentEditorView = refundPaymentEditorView;
            CapturePaymentEditorView = capturePaymentEditorView;
            RequiresCustomer = requiresCustomer;
        }


        /// <summary>
        /// Gets a title to be displayed in the back office if applicable.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the authorize capture editor view.
        /// </summary>
        public string AuthorizeEditorView { get; private set; }

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
        /// Gets the capture payment editor view.
        /// </summary>
        public string CapturePaymentEditorView { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the payment method should be used when quoting shipments.
        /// If false, it is assumed that the payment method is to be used programmatically for some other purpose.
        /// </summary>
        public bool IncludeInPaymentSelection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this payment method must be used with a logged in customer (vaulted transactions)
        /// </summary>
        public bool RequiresCustomer { get; private set; }

    }
}