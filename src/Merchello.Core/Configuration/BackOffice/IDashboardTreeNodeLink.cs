namespace Merchello.Core.Configuration.BackOffice
{
    /// <summary>
    /// Represent a back office dashboard tree node link.
    /// </summary>
    public interface IDashboardTreeNodeLink
    {
        /// <summary>
        /// Gets the icon.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets a value indicating whether node should be visible in the back office.
        /// </summary>
        bool Visible { get; }
    }
}