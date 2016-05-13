namespace Merchello.FastTrack.Models
{
    /// <summary>
    /// Defines a model that allows for defining a successful redirect.
    /// </summary>
    public interface ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the success URL to redirect to the shipping entry stage.
        /// </summary>
        string SuccessRedirectUrl { get; set; }
    }
}