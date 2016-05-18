namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Represents a checkout workflow marker.
    /// </summary>
    public class CheckoutWorkflowMarker : ICheckoutWorkflowMarker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutWorkflowMarker"/> class.
        /// </summary>
        public CheckoutWorkflowMarker()
        {
            this.Previous = CheckoutStage.None;
            this.Current = CheckoutStage.BillingAddress;
            this.Next = CheckoutStage.ShippingAddress;
        }

        /// <summary>
        /// Gets or sets the previous stage.
        /// </summary>
        public CheckoutStage Previous { get; set; }

        /// <summary>
        /// Gets or sets the current stage.
        /// </summary>
        public CheckoutStage Current { get; set; }

        /// <summary>
        /// Gets or sets the next stage.
        /// </summary>
        public CheckoutStage Next { get; set; }
    }
}