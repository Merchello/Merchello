using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a class which includes an <see cref="ExtendedDataCollection"/>
    /// </summary>
    public interface IHasExtendedData
    {
        /// <summary>
        /// A collection to store custom/extended data
        /// </summary>
        [DataMember]
        ExtendedDataCollection ExtendedData { get; }
    }
}