namespace Merchello.Core.Builders
{
    using Umbraco.Core;

    /// <summary>
    /// Defines builder objects
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to build
    /// </typeparam>
    public interface IBuilderChain<T>
    {
        /// <summary>
        /// Performs the "build" work
        /// </summary>
        /// <returns><see cref="Attempt"/> of T</returns>
        Attempt<T> Build();
    }
}