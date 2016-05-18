namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Defines a model that needs to indicate whether or not the intended process may require JavaScript.
    /// </summary>
    public interface IRequireJs
    {
        /// <summary>
        /// Gets or sets a value indicating whether JavaScript is required.
        /// </summary>
        bool RequireJs { get; set; }
    }
}