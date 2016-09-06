namespace Merchello.Core.Configuration
{
    /// <summary>
    /// Represents a type reference in a configuration file.
    /// </summary>
    public interface ITypeReference
    {
        /// <summary>
        /// Gets the type reference string.
        /// </summary>
        /// <remarks>
        /// Example: SomeNameSpace.SomeClass, SomeAssemblyName
        /// </remarks>
        string Type { get; }
    }
}