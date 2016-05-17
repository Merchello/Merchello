namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Defines a checkout model.
    /// </summary>
    public interface ICheckoutModel : IUiModel
    {
        /// <summary>
        /// Gets or sets the workflow marker.
        /// </summary>
        /// <remarks>
        /// This is used to assist in tracking the checkout
        /// </remarks>
        ICheckoutWorkflowMarker WorkflowMarker { get; set; }  
    }
}