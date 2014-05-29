using System;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Marker interface for Monitor observers
    /// </summary>
    public interface IMonitor
    {


        /// <summary>
        /// The type being observed {T}
        /// </summary>
        Type ObservesType { get; }
    }
}