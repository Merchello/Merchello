namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a DigitalMediaService.
    /// </summary>
    public interface IDigitalMediaService : IService
    {
        ///// <summary>
        ///// Creates a <see cref="IDigitalMedia"/> without saving it to the database.
        ///// </summary>
        ///// <param name="name">
        ///// The name.
        ///// </param>
        ///// <returns>
        ///// The <see cref="IDigitalMedia"/>.
        ///// </returns>
        //IDigitalMedia CreateDigitalMedia(string name);

        /// <summary>
        /// Creates a <see cref="IDigitalMedia"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        IDigitalMedia CreateDigitalMediaForProductVariant(Guid productVariantKey, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IDigitalMedia"/>
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IDigitalMedia digitalMedia, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Delete(IDigitalMedia digitalMedia, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Delete(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IDigitalMedia"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        IDigitalMedia GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IDigitalMedia"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        IEnumerable<IDigitalMedia> GetByKeys(IEnumerable<Guid> keys);
    }
}