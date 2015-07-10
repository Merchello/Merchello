namespace Merchello.Core.Chains
{
    using Umbraco.Core;

    /// <summary>
    /// Defines a DataModifierChain.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to Modify
    /// </typeparam>
    public interface IDataModifierChain<T>
    {
        /// <summary>
        /// Attempts to modify the data.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<T> Modify(T value);
    }
}