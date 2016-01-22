namespace Merchello.Bazaar.Models
{
    using System;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// The payment method UI info.
    /// </summary>
    public partial class PaymentMethodUiInfo
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        public Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the url action parameters used to embed the Payment form
        /// </summary>
        public UrlActionParams UrlActionParams { get; set; }
    }
}