namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a class which includes an <see cref="ExtendedDataCollection"/>
    /// </summary>
    public interface IHasExtendedData
    {
        /// <summary>
        /// Gets a collection to store custom/extended data
        /// </summary>
        [DataMember]
        ExtendedDataCollection ExtendedData { get; }
    }
}