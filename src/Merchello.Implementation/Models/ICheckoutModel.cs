namespace Merchello.Implementation.Models
{
    /// <summary>
    /// Defines a checkout model.
    /// </summary>
    public interface ICheckoutModel
    {
        /// <summary>
        /// Gets or sets the workflow marker.
        /// </summary>
        ICheckoutWorkflowMarker WorkflowMarker { get; set; }  
    }
}