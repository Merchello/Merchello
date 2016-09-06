namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    ///  Represents a configuration section for configurations for associating custom "type fields" with Merchello. 
    /// </summary>
    public interface ITypeFieldsSection
    {
        /// <summary>
        /// Gets a dictionary of type fields.
        /// </summary>
        /// <remarks>
        /// The dictionary key is the group of type fields (e.g.  Product, LineItem, etc.)
        /// </remarks>
        IDictionary<string, ITypeField> TypeFields { get; }
    }
}