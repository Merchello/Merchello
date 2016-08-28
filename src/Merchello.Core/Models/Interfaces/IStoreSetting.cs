using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Store Setting
    /// </summary>
    public interface IStoreSetting : IEntity
    {
        /// <summary>
        /// The name of the store setting
        /// </summary>
        /// <remarks>
        /// Should be unique but not enforced
        /// </remarks>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The value of the store setting
        /// </summary>
        [DataMember]
        string Value { get; set; }

        /// <summary>
        /// The type of the store setting
        /// </summary>
        [DataMember]
        string TypeName { get; set; }
    }
}