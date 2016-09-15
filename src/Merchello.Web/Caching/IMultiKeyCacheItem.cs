namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;

    internal interface IMultiKeyCacheItem<T>
    {
        IEnumerable<Guid> Keys { get; set; }

        T Item { get; set; }
    }
}