namespace Merchello.Core.Cache
{
    using System;

    /// <summary>
    /// Specifies how a repository cache policy should cache entities.
    /// </summary>
    internal class RepositoryCachePolicyOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCachePolicyOptions"/> class.
        /// </summary>
        /// <param name="performCount">
        /// The perform count.
        /// </param>
        public RepositoryCachePolicyOptions(Func<int> performCount)
        {
            this.PerformCount = performCount;
            this.GetAllCacheValidateCount = true;
            this.GetAllCacheAllowZeroCount = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCachePolicyOptions"/> class.
        /// </summary>
        public RepositoryCachePolicyOptions()
        {
            this.PerformCount = null;
            this.GetAllCacheValidateCount = false;
            this.GetAllCacheAllowZeroCount = false;
        }

        /// <summary>
        /// Gets the Callback required to get count for GetAllCacheValidateCount
        /// </summary>
        public Func<int> PerformCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to validate the total item count when all items are returned from cache, the default is true but this
        /// means that a db lookup will occur - though that lookup will probably be significantly less expensive than the normal 
        /// GetAll method. 
        /// </summary>
        /// <remarks>
        /// setting this to return false will improve performance of GetAll cache with no parameters but should only be used
        /// for specific circumstances
        /// </remarks>
        public bool GetAllCacheValidateCount { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the GetAll method will cache that there are zero results so that the db is not hit when there are no results found
        /// </summary>
        public bool GetAllCacheAllowZeroCount { get; set; }
    }
}