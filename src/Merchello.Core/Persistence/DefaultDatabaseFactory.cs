﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence
{
    /// <summary>
    /// This is a modified version of Umbraco's DefaultDatabaseFactory 
    /// </summary>
    /// <remarks>
    /// We need to read the connectionstring name from the Merchello configuration section and also
    /// assert that we have our own database object in the event that the Merchello database is different from
    /// Umbraco
    /// </remarks>
    internal class DefaultDatabaseFactory : DisposableObject, IDatabaseFactory
    {
        private readonly string _connectionStringName;
        public string ConnectionString { get; private set; }
        public string ProviderName { get; private set; }

        //very important to have ThreadStatic:
        [ThreadStatic]
        private static volatile UmbracoDatabase _nonHttpInstance;

        private static readonly object Locker = new object();

        public DefaultDatabaseFactory() :
            this(PluginConfiguration.Section.DefaultConnectionStringName)
        {
            
        }

        /// <summary>
		/// Constructor accepting custom connection string
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string in web.config</param>
		public DefaultDatabaseFactory(string connectionStringName)
		{
			Mandate.ParameterNotNullOrEmpty(connectionStringName, "connectionStringName");
			_connectionStringName = connectionStringName;
		}

        /// <summary>
        /// Constructor accepting custom connectino string and provider name
        /// </summary>
        /// <param name="connectionString">Connection String to use with Database</param>
        /// <param name="providerName">Database Provider for the Connection String</param>
        public DefaultDatabaseFactory(string connectionString, string providerName)
        {
            Mandate.ParameterNotNullOrEmpty(connectionString, "connectionString");
            Mandate.ParameterNotNullOrEmpty(providerName, "providerName");
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public UmbracoDatabase CreateDatabase()
        {
            //no http context, create the singleton global object
            if (HttpContext.Current == null)
            {
                if (_nonHttpInstance == null)
                {
                    lock (Locker)
                    {
                        //double check
                        if (_nonHttpInstance == null)
                        {
                            _nonHttpInstance = string.IsNullOrEmpty(ConnectionString) == false &&
                                               string.IsNullOrEmpty(ProviderName) == false
                                ? new UmbracoDatabase(ConnectionString, ProviderName)
                                : new UmbracoDatabase(_connectionStringName);
                        }
                    }
                }
                return _nonHttpInstance;
            }

            //we have an http context, so only create one per request
			if (HttpContext.Current.Items.Contains(typeof(DefaultDatabaseFactory)) == false)
			{
			    HttpContext.Current.Items.Add(typeof (DefaultDatabaseFactory),
			                                  string.IsNullOrEmpty(ConnectionString) == false && string.IsNullOrEmpty(ProviderName) == false
			                                      ? new UmbracoDatabase(ConnectionString, ProviderName)
			                                      : new UmbracoDatabase(_connectionStringName));
			}
			return (UmbracoDatabase)HttpContext.Current.Items[typeof(DefaultDatabaseFactory)];
        }


        protected override void DisposeResources()
        {
            if (HttpContext.Current == null)
            {
                _nonHttpInstance.Dispose();
            }
            else
            {
                if (HttpContext.Current.Items.Contains(typeof(DefaultDatabaseFactory)))
                {
                    ((UmbracoDatabase)HttpContext.Current.Items[typeof(DefaultDatabaseFactory)]).Dispose();
                }
            }
        }

    
    }
}
