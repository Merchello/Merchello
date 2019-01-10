using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Web.Models.Reports
{
    public sealed class SalesSearchSnapshot
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        public string ProductName { get; set; }
    }
}
