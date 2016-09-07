namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Identifies a type field or type field that can have custom type fields added through configuration.
    /// </summary>
    public interface ICanHaveCustomTypeFields
    {
        /// <summary>
        /// Returns a custom <see cref="ITypeField"/> from the merchelloExtensibility configuration
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