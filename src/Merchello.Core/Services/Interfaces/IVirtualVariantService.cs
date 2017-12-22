namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a VirtualVariantService.
    /// </summary>
    public interface IVirtualVariantService : IService
    {
        /// <summary>
        /// Creates a <see cref="IVirtualVariant"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IVirtualVariant"/>.
        /// </returns>
        IVirtualVariant CreateVirtualVariant(string name, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IVirtualVariant"/>
        /// </summary>
        /// <param name="virtualVariant">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IVirtualVariant virtualVariant, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IVirtualVariant"/> from the database.
        /// </summary>
        /// <param name="virtualVariant">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Delete(IVirtualVariant virtualVariant, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IVirtualVariant"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IVirtualVariant"/>.
        /// </returns>
        IVirtualVariant GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IVirtualVariant"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IVirtualVariant}"/>.
        /// </returns>
        IEnumerable<IVirtualVariant> GetByKeys(IEnumerable<Guid> keys);
    }
}