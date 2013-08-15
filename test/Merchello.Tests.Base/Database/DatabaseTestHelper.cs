using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Tests.Base.Database
{
    public class DatabaseTestHelper
    {
        /// <summary>
        /// Clears an initialized database
        /// </summary>
        public static void ClearDatabase()
        {
            var config = (MerchelloSection)ConfigurationManager.GetSection("merchello");

            var connection = new SqlCeConnection(config.DefaultConnectionStringName);
            

        }
    }
}
