namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Merchello Store Setting
    /// </summary>
    public interface IStoreSetting : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the store setting
        /// </summary>
        /// <remarks>
        /// Should be unique but not enforced
        /// </remarks>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the store setting
        /// </summary>
        [DataMember]
        string Value { get; set; }

        /// <summary>
        /// Gets or sets type of the store setting
        /// </summary>
        [DataMember]
        string TypeName { get; set; }
    }
}