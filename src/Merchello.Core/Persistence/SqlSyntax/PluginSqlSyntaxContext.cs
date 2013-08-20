using System;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.SqlSyntax
{
    /// <summary>
    /// Singleton to handle the configuration of the SqlSyntaxProvider used in Merchello.
    /// </summary>
    /// <remarks>
    /// This is required if for some reason the Merchello database was hosted in a database using
    /// a different Sql Syntax than that of Umbraco.  Ex.  Umbraco in MySql and Merchello in SqlServer
    /// </remarks>
    public static class PluginSqlSyntaxContext
    {
        private static ISqlSyntaxProvider _sqlSyntaxProvider;

        public static ISqlSyntaxProvider SqlSyntaxProvider
        {
            get
            {
                // if null, try to get the SyntaxProvider from Umbraco
                if (_sqlSyntaxProvider == null) _sqlSyntaxProvider = SqlSyntaxContext.SqlSyntaxProvider ?? null;
  
                if (_sqlSyntaxProvider == null)
                {
                    throw new ArgumentNullException("SqlSyntaxProvider",
                                                    "You must set the singleton 'Merchello.Core.Persistence.SqlSyntax.PluginSqlSyntaxContext' to use an sql syntax provider");
                }

                return _sqlSyntaxProvider;
            }
            set { _sqlSyntaxProvider = value; }
        }
    }
}
