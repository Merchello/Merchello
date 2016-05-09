namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Define a checkout workflow marker.
    /// </summary>
    /// <remarks>
    /// This makes single page checkouts slightly easier in the client layer
    /// </remarks>
    public interface ICheckoutWorkflowMarker : IUiModel
    {
        /// <summary>
        /// Gets or sets the previous stage.
        /// </summary>
        CheckoutStage Previous { get; set; }

        /// <summary>
        /// Gets or sets the current stage.
        /// </summary>
        CheckoutStage Current { get; set; }

        /// <summary>
        /// Gets or sets the next stage.
        /// </summary>
        CheckoutStage Next { get; set; } 
    }
}