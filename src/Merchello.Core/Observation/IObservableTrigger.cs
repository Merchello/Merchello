using System;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Marker interface for observable triggers
    /// </summary>
    public interface IObservableTrigger<T> : IObservable<T>
    { }
}