namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents a custom type field, usually added via configuration.
    /// </summary>
    public interface ICustomTypeField
    {
        /// <summary>
        /// Gets the <see cref="ITypeField"/> by it's alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        ITypeField Custom(string alias);
    }
}