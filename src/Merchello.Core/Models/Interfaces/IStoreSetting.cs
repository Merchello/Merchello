namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a store setting.
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
        /// Gets or sets type of the store setting
        /// </summary>
        string TypeName { get; set; }

        /// <summary>
        /// Gets a value indicating whether the setting is global to all stores.
        /// </summary>
        /// <remarks>
        /// If true, the same record will be used in all stores.
        /// </remarks>
        bool IsGlobal { get; }
    }
}