using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines an ObservationChannelResolver
    /// </summary>
    internal interface IObservationChannelResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IObservationChannel"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IObservationChannel"/></returns>
        IEnumerable<IObservationChannel> GetChannelByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IObservationChannel"/>s
        /// </summary>
        IEnumerable<IObservationChannel> GetAllChannels();

        /// <summary>
        /// Gets a <see cref="IObservationChannel"/> from the resolver
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IObservationChannel"/></returns>
        IObservationChannel TryGetChannel(Guid key);
    }
}