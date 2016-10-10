namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a province.
    /// </summary>
    public interface IProvince
    {
        /// <summary>
        /// Gets the name of the province
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// Gets the two letter ISO province code
        /// </summary>
        [DataMember]
        string Code { get; }
    }
}