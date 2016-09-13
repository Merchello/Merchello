namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    internal interface IProductFilterTreeQuery
    {
         
    }



    internal interface AppliedFiltersContext
    {
        /// <summary>
        /// Gets or sets the collection keys to be applied .
        /// </summary>
        IEnumerable<Guid> CollectionKeys { get; set; }
    }

}