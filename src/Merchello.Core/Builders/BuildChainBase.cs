namespace Merchello.Core.Builders
{
    using Chains;
    using Umbraco.Core;

    /// <summary>
    /// Represents the build chain base class
    /// </summary>
    /// <typeparam name="T"><see cref="Attempt"/> of T</typeparam>
    public abstract class BuildChainBase<T> : ConfigurationChainBase<T>, IBuilderChain<T>
    {       
        /// <summary>
        /// Performs the "build" work
        /// </summary>
        /// <returns><see cref="Attempt"/> of T</returns>
        public abstract Attempt<T> Build();
    }
}