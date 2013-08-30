using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models
{
    public interface IPage<T>
    {
        long CurrentPage { get; set; }
        long TotalPages { get; set; }
        long TotalItems { get; set; }
        long ItemsPerPage { get; set; }
        IEnumerable<T> Items { get; set; }
    }
}
