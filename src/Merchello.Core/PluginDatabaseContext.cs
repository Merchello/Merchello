using System;
using System.Configuration;
using Merchello.Core.Configuration;
using Merchello.Core.Persistence.Migrations.Initial;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core
{
    /// <summary>
    /// The Merchello Plugin Database context
    /// </summary>
    public class PluginDatabaseContext
    {
        
        
        private bool _configured;
        private string _connectionString;
        private string _providerName;
        private IDatabaseUnitOfWorkProvider _uowProvider;
        private PluginDatabaseSchemaResult _result;

        internal PluginDatabaseContext()
        {
            //_uowProvider = new PetaPocoUnitOfWorkProvider();
        }

        /// <summary>
        /// Gets the <see cref="Database"/> object for doing CRUD operations
        /// against custom tables that resides in the Merchello database.
        /// </summary>
        /// <remarks>
        /// This should not be used for CRUD operations or queries against the
        /// standard Merchello tables! Use the Public services for that.
        /// </remarks>
        public UmbracoDatabase Database
        {
            get { return null; }
        }

        /// <summary>
        /// Boolean indicating whether the database has been configured
        /// </summary>
        public bool IsDatabaseConfigured
        {
            get { return _configured; }
        }

        /// <summary>
        /// Gets the configured umbraco db connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Returns the name of the dataprovider from the connectionstring setting in config
        /// </summary>
        internal string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_providerName) == false)
                    return _providerName;

                _providerName = "System.Data.SqlClient";
                if (ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName] != null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ProviderName))
                        _providerName = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ProviderName;
                }
                else
                {
                    throw new InvalidOperationException("Can't find a connection string with the name '" + PluginConfiguration.Section.DefaultConnectionStringName + "'");
                }
                return _providerName;
            }
        }

        /// <summary>
        /// Returns the Type of DatabaseProvider used
        /// </summary>
        public DatabaseProviders DatabaseProvider
        {
            get
            {
                string dbtype = Database.Connection == null ? ProviderName : Database.Connection.GetType().Name;


                if (dbtype.StartsWith("SqlCe") || dbtype.Contains("SqlServerCe")) return DatabaseProviders.SqlServerCE;

                return DatabaseProviders.SqlServer;
            }
        }



        /// <summary>
        /// Internal method to initialize the database configuration.
        /// </summary>
        /// <remarks>
        /// If an Umbraco connectionstring exists the database can be configured on app startup,
        /// but if its a new install the entry doesn't exist and the db cannot be configured.
        /// So for new installs the Initialize() method should be called after the connectionstring
        /// has been added to the web.config.
        /// </remarks>
        internal void Initialize()
        {
            var databaseSettings = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName];
            if (databaseSettings != null && string.IsNullOrWhiteSpace(databaseSettings.ConnectionString) == false && string.IsNullOrWhiteSpace(databaseSettings.ProviderName) == false)
            {
                var providerName = "System.Data.SqlClient";
                string connString = null;
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ProviderName))
                {
                    providerName = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ProviderName;
                    connString = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ConnectionString;
                }
                Initialize(providerName, connString);
         
            }
            
            else
            {
                _configured = false;
            }
        }

        internal void Initialize(string providerName)
        {
            _providerName = providerName;

            try
            {
                SqlSyntaxContext.SqlSyntaxProvider = providerName.Equals("System.Data.SqlClient")
                    ? new SqlServerSyntaxProvider()
                    : new SqlCeSyntaxProvider() as ISqlSyntaxProvider;

                _configured = true;
            }
            catch (Exception e)
            {
                _configured = false;

                LogHelper.Info<PluginDatabaseContext>("Initialization of the PluginDatabaseContext failed with following error: " + e.Message);
                LogHelper.Info<PluginDatabaseContext>(e.StackTrace);
            }
        }

        internal void Initialize(string providerName, string connectionString)
        {
            _connectionString = connectionString;
            Initialize(providerName);
        }


        internal PluginDatabaseSchemaResult ValidateDatabaseSchema()
        {
            if (_configured == false || (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(ProviderName)))
                return new PluginDatabaseSchemaResult();    

            if (_result == null)
            {
                var database = new UmbracoDatabase(_connectionString, ProviderName);
                var dbSchema = new Merchello.Core.Persistence.Migrations.Initial.DatabaseSchemaCreation(database);
                _result = dbSchema.ValidateSchema();
            }
            return _result;
        }



        internal Result CreateDatabaseSchemaAndDataOrUpgrade()
        {
            if (_configured == false || (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(ProviderName)))
            {
                return new Result
                {
                    Message =
                        "Database configuration is invalid. Please check that the entered database exists and that the provided username and password has write access to the database.",
                    Success = false,
                    Percentage = "10"
                };
            }

            try
            {
                LogHelper.Info<PluginDatabaseContext>("Database configuration status: Started");

                var message = string.Empty;

                var database = new UmbracoDatabase(_connectionString, ProviderName);
                var supportsCaseInsensitiveQueries = SqlSyntaxContext.SqlSyntaxProvider.SupportsCaseInsensitiveQueries(database);

                // TODO: At the moment we will only support SqlServer and SqlCE
                //if (supportsCaseInsensitiveQueries == false)
                //{
                //    message = "<p>&nbsp;</p><p>The database you're trying to use does not support case insensitive queries. <br />We currently do not support these types of databases.</p>" +
                //              "<p>You can fix this by changing the following setting in your my.ini file in your MySQL installation directory:</p>" +
                //              "<pre>lower_case_table_names=1</pre><br />" +
                //              "<p>Note: Make sure to check with your hosting provider if they support case insensitive queries as well.</p>" +
                //              "<p>For more technical information on case sensitivity in MySQL, have a look at " +
                //              "<a href='http://dev.mysql.com/doc/refman/5.0/en/identifier-case-sensitivity.html'>the documentation on the subject</a></p>";

                //    return new Result { Message = message, Success = false, Percentage = "15" };
                //}
                //else if (supportsCaseInsensitiveQueries == null)
                //{
                //    message = "<p>&nbsp;</p><p>Warning! Could not check if your database type supports case insensitive queries. <br />We currently do not support these databases that do not support case insensitive queries.</p>" +
                //              "<p>You can check this by looking for the following setting in your my.ini file in your MySQL installation directory:</p>" +
                //              "<pre>lower_case_table_names=1</pre><br />" +
                //              "<p>Note: Make sure to check with your hosting provider if they support case insensitive queries as well.</p>" +
                //              "<p>For more technical information on case sensitivity in MySQL, have a look at " +
                //              "<a href='http://dev.mysql.com/doc/refman/5.0/en/identifier-case-sensitivity.html'>the documentation on the subject</a></p>";
                //}
                //else
                //{
                    
                //    if (PluginSqlSyntaxContext.SqlSyntaxProvider.GetType() == typeof(MySqlSyntaxProvider))
                //    {
                //        message = "<p>&nbsp;</p><p>Congratulations, the database step ran successfully!</p>" +
                //                  "<p>Note: You're using MySQL and the database instance you're connecting to seems to support case insensitive queries.</p>" +
                //                  "<p>However, your hosting provider may not support this option. Umbraco does not currently support MySQL installs that do not support case insensitive queries</p>" +
                //                  "<p>Make sure to check with your hosting provider if they support case insensitive queries as well.</p>" +
                //                  "<p>They can check this by looking for the following setting in the my.ini file in their MySQL installation directory:</p>" +
                //                  "<pre>lower_case_table_names=1</pre><br />" +
                //                  "<p>For more technical information on case sensitivity in MySQL, have a look at " +
                //                  "<a href='http://dev.mysql.com/doc/refman/5.0/en/identifier-case-sensitivity.html'>the documentation on the subject</a></p>";
                //    }
                //}

                var schemaResult = ValidateDatabaseSchema();
                var installedVersion = schemaResult.DetermineInstalledVersion();


                //If Configuration section version is empty and the determined version is "empty" its a new install - otherwise upgrade the existing
                if (string.IsNullOrEmpty(PluginConfiguration.Section.Version) && installedVersion.Equals(new Version(0, 0, 0)))
                {
                    database.CreateDatabaseSchema();
                    message = message + "<p>Installation completed!</p>";
                }
                // TODO : Upgrades
                //else
                //{
                //    var configuredVersion = string.IsNullOrEmpty(GlobalSettings.ConfigurationStatus)
                //                                ? installedVersion
                //                                : new Version(GlobalSettings.ConfigurationStatus);
                //    var targetVersion = UmbracoVersion.Current;
                //    var runner = new MigrationRunner(configuredVersion, targetVersion, GlobalSettings.UmbracoMigrationName);
                //    var upgraded = runner.Execute(database, true);
                //    message = message + "<p>Upgrade completed!</p>";
                //}

                //now that everything is done, we need to determine the version of SQL server that is executing

                LogHelper.Info<PluginDatabaseContext>("Database configuration status: " + message);

                return new Result { Message = message, Success = true, Percentage = "100" };
            }
            catch (Exception ex)
            {
                LogHelper.Info<PluginDatabaseContext>("Database configuration failed with the following error and stack trace: " + ex.Message + "\n" + ex.StackTrace);

                if (_result != null)
                {
                    LogHelper.Info<PluginDatabaseContext>("The database schema validation produced the following summary: \n" + _result.GetSummary());
                }

                return new Result
                {
                    Message =
                        "The database configuration failed with the following message: " + ex.Message +
                        "\n Please check log file for additional information (can be found in '/App_Data/Logs/UmbracoTraceLog.txt')",
                    Success = false,
                    Percentage = "90"
                };
            }
        }

        internal class Result
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public string Percentage { get; set; }
        }


    }
}
