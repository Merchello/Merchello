using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence
{
    /// <summary>
    /// Used to create the UmbracoDatabase for use in the MerchelloAppContext.DatabaseContext
    /// </summary>
    internal interface IDatabaseFactory : IDisposable
    {
        UmbracoDatabase CreateDatabase();
    }
}
