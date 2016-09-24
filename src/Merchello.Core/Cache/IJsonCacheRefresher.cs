namespace Merchello.Core.Cache
{
    /// <summary>
    /// A cache refresher that supports refreshing or removing cache based on a custom JSON payload
    /// </summary>
    internal interface IJsonCacheRefresher : ICacheRefresher
    {
        /// <summary>
        /// Refreshes, clears, etc... any cache based on the information provided in the JSON
        /// </summary>
        /// <param name="jsonPayload">The JSON payload</param>
        void Refresh(string jsonPayload);
    }
}