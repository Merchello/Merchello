using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Defines builder objects
    /// </summary>
    public interface IBuilderChain<T>
    {
        /// <summary>
        /// Performs the "build" work
        /// </summary>
        /// <returns><see cref="Attempt"/> of T</returns>
        Attempt<T> Build();
    }
}