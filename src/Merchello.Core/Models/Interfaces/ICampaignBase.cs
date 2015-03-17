namespace Merchello.Core.Models
{
    /// <summary>
    /// Define a Marketing CampaignSettings Base Class.
    /// </summary>
    public interface ICampaignBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        bool Active { get; set; }
    }
}