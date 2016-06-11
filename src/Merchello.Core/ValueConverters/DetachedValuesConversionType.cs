namespace Merchello.Core.ValueConverters
{
    using Umbraco.Core.Models;

    /// <summary>
    /// Specifies the how the Detached Values should be converted.
    /// </summary>
    public enum DetachedValuesConversionType
    {
        /// <summary>
        /// Indicates conversion of value for saving to the database
        /// </summary>
        Db,


        /// <summary>
        /// Indicates conversion of value for use in a back office editor
        /// </summary>
        Editor,

        /// <summary>
        /// Indicates conversion of a value as if it was retrieved from the should be used for <see cref="IPublishedContent"/>
        /// </summary>
        Content
    }
}