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
        ICheckoutWorkflowMarker WorkflowMarker { get; set; }  
    }
}