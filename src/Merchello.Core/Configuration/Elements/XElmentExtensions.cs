namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Xml.Linq;

    using Merchello.Core.Acquired;
    using Merchello.Core.Configuration.BackOffice;

    /// <summary>
    /// XElement extension methods for configuration.
    /// </summary>
    /// REVIEW - this is common to BackOfficeTreeElement and FilterElement
    internal static class XElmentExtensions
    {
        /// <summary>
        /// Builds a <see cref="DashboardTreeNodeKeyLink"/> from a <see cref="XElement"/> representation.
        /// </summary>
        /// <param name="xp">
        /// The <see cref="XElement"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DashboardTreeNodeKeyLink"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if the key cannot be converted to a GUID
        /// </exception>
        public static IDashboardTreeNodeKeyLink AsDashboardTreeNodeKeyLink(this XElement xp)
        {
            var key = xp.Attribute("key").Value.TryConvertTo<Guid>();
            if (!key.Success) throw key.Exception;

            var visible = xp.Attribute("visible").Value.TryConvertTo<bool>();

            return new DashboardTreeNodeKeyLink
            {
                Key = key.Result,
                Title = xp.Attribute("ref").Value,
                Icon = xp.Attribute("icon").Value,
                Visible = !visible.Success || visible.Result
            };
        }
    }
}