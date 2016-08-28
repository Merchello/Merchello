namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines the ModifiedData.
    /// </summary>
    public interface IDataModifierLog
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        object OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the modified value.
        /// </summary>
        object ModifiedValue { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        ExtendedDataCollection ExtendedData { get; set; }
    }
}