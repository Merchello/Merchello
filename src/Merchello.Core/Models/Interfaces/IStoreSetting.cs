namespace Merchello.Core.Models
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a Merchello Store Setting
    /// </summary>
    public interface IStoreSetting : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the store setting
        /// </summary>
        /// <remarks>
        /// Should be unique but not enforced
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the store setting
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the store setting
        /// </summary>
        string TypeName { get; set; }
    }
}