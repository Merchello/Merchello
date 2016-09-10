namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Configuration.BackOffice;

    /// <summary>
    /// Represents a configuration section for Merchello back office configurations.
    /// </summary>
    public interface IBackOfficeSection : IMerchelloSection
    {
        /// <summary>
        /// Gets the a collection of back office tree nodes.
        /// </summary>
        /// <remarks>
        /// This allows for configuring how and what is displayed in the main navigational tree in the CMS back office.
        /// </remarks>
        IEnumerable<IDashboardTreeNode> Trees { get; }

        /// <summary>
        /// Gets a value indicating whether the implementer should be able to specify what HTML element should be used when selecting option choices.
        /// <para>
        /// This requires the implementer to write custom razor code in their templates and it is generally not included in any of the Merchello starter kits.
        /// </para>
        /// </summary>
        bool EnableProductOptionUiElementSelection { get; }

        /// <summary>
        /// Gets the collection of configured ProductOption UI elements that can be assigned in the back office.
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> ProductOptionUi { get; }
    }
}