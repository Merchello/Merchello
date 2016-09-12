namespace Merchello.Umbraco.Adapters
{
    using Merchello.Core;
    using Merchello.Core.Acquired;

    /// <summary>
    /// Extension method to map <see cref="Attempt{T}"/> to Umbraco's <see>
    ///         <cref>global::Umbraco.Core.Attempt{T}</cref>
    ///       </see>.
    /// </summary>
    internal static class UmbracoAttemptExtensions
    {
        /// <summary>
        /// Maps Merchello's <see cref="Attempt{T}"/> to Umbraco's attempt.
        /// </summary>
        /// <param name="merchAttempt">
        /// The Merchello <see cref="Attempt{T}"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type of the result
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public static global::Umbraco.Core.Attempt<T> ForUmb<T>(this Attempt<T> merchAttempt)
        {
            // TODO needs to be tested!!!
            return merchAttempt.Success
                       ? global::Umbraco.Core.Attempt<T>.Succeed(merchAttempt.Result)
                       : global::Umbraco.Core.Attempt<T>.Fail(merchAttempt.Result, merchAttempt.Exception);
        }
    }
}