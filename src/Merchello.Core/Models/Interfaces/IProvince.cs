namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a state, region or province reference.
    /// </summary>
    public interface IProvince
    {
        /// <summary>
        /// Gets the name of the province
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// Gets the two letter province code
        /// </summary>
        [DataMember]
        string Code { get; }
    }
}