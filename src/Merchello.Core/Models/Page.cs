using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> : IPage<T>
    {
        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public long TotalItems { get; set; }

        public long ItemsPerPage { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
