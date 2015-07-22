namespace Merchello.Core.Models
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a DataModifierLog.
    /// </summary>
    public class DataModifierLog : IDataModifierLog
    {
        /// <summary>
        /// Gets or sets the property name of the property modified.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        public object OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the modified value.
        /// </summary>
        public object ModifiedValue { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }
    }
}