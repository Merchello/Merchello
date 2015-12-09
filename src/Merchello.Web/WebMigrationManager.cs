namespace Merchello.Web
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.Persistence.Migrations.Analytics;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The web migration manager.
    /// </summary>
    internal class WebMigrationManager : CoreMigrationManager
    {
        /// <summary>
        /// The post URL.
        /// </summary>
        private const string PostUrl = "http://instance.merchello.com/api/migration/Post";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebMigrationManager"/> class.
        /// </summary>
        public WebMigrationManager()
            : this(ApplicationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebMigrationManager"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application Context.
        /// </param>
        public WebMigrationManager(ApplicationContext applicationContext)
            : base(applicationContext.DatabaseContext.Database, applicationContext.DatabaseContext.SqlSyntax, LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebMigrationManager"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal WebMigrationManager(Database database, ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(database, sqlSyntax, logger)
        {
        }

        /// <summary>
        /// The ensure database.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool EnsureDatabase()
        {
            var record = EnsureDatabaseInstall();
            Logger.Info<WebMigrationManager>("Migration record was null == " + (record == null).ToString());
            if (record != null) PostAnalyticInfo(record);
            return record != null;
        }

        /// <summary>
        /// The post analytic info.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        public async void PostAnalyticInfo(MigrationRecord record)
        {
            if (!MerchelloConfiguration.Current.Section.EnableInstallTracking) return;
            var client = new HttpClient();
            try
            {
                var data = JsonConvert.SerializeObject(record);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await client.PostAsync(PostUrl, new StringContent(data, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                LogHelper.Error<WebMigrationManager>("Migration record post exception", ex);
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                } 
            }
            
        }
    }
}