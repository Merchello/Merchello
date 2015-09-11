namespace Merchello.Core.Chains.CopyEntity
{
    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// The CopyEntityChain interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="Entity"/>
    /// </typeparam>
    public interface ICopyEntityChain<T>
        where T : class, IEntity
    {
        /// <summary>
        /// The copy.
        /// </summary>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<T> Copy();
    }
}