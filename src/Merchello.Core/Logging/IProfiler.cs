namespace Merchello.Core.Logging
{
    using System;

    /// <summary>
    /// Defines an object for use in the application to profile operations
    /// </summary>
    public interface IProfiler
    {
        /// <summary>
        /// Render the UI to display the profiler 
        /// </summary>
        /// <returns>
        /// The content to be rendered.
        /// </returns>
        /// <remarks>
        /// Generally used for HTML displays
        /// </remarks>
        string Render();

        /// <summary>
        /// Profile an operation
        /// </summary>
        /// <param name="name">
        /// The step name
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/> step
        /// </returns>
        /// <remarks>
        /// Use the 'using(' syntax
        /// </remarks>
        IDisposable Step(string name);

        /// <summary>
        /// Start the profiler
        /// </summary>
        void Start();

        /// <summary>
        /// Start the profiler
        /// </summary>
        /// <param name="discardResults">
        /// A value indicating whether or not to dispose the results.
        /// </param>
        /// <remarks>
        /// set discardResults to false when you want to abandon all profiling, this is useful for 
        /// when someone is not authenticated or you want to clear the results based on some other mechanism.
        /// </remarks>
        void Stop(bool discardResults = false);
    }
}