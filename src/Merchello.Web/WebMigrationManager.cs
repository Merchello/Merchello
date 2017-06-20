namespace Merchello.Web
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.Persistence.Migrations.Analytics;

    using Newtonsoft.Json;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The web migration manager.
    /// </summary>
    internal class WebMigrationManager : CoreMigrationManager
    {
        /// <summary>
        /// The post URL
        /// </summary>
        private const string PostUrl = "https://instance.merchello.com/api/migration/Post";

        /// <summary>
        /// The record domain URL
        /// </summary>
        private const string RecordDomainUrl = "https://instance.merchello.com/api/migration/RecordDomain";

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
        /// Posts the migration analytic record.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpResponseMessage> PostAnalyticInfo(MigrationRecord record)
        {
            if (!MerchelloConfiguration.Current.Section.EnableInstallTracking)
                return new HttpResponseMessage(HttpStatusCode.OK);

            // reset the domain analytic
            if (MerchelloContext.HasCurrent)
            {
                var storeSettingService = MerchelloContext.Current.Services.StoreSettingService;

                var setting = storeSettingService.GetByKey(Constants.StoreSetting.HasDomainRecordKey);
                if (setting != null)
                {
                    setting.Value = false.ToString();
                }

                storeSettingService.Save(setting);
            }

            var data = JsonConvert.SerializeObject(record);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseMessage = null;
                try
                {
                    responseMessage = await client.PostAsync(PostUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                }
                catch (Exception ex)
                {
                    if (responseMessage == null)
                    {
                        responseMessage = new HttpResponseMessage();
                    }

                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    responseMessage.ReasonPhrase = string.Format("PostAnalyticInfo failed: {0}", ex);
                }

                return responseMessage;
            }
        }

        /// <summary>
        /// Posts a record of the domain.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpResponseMessage> PostDomainRecord(MigrationDomain record)
        {
            if (!MerchelloConfiguration.Current.Section.EnableInstallTracking)
                return new HttpResponseMessage(HttpStatusCode.OK);

            var data = JsonConvert.SerializeObject(record);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseMessage = null;
                try
                {
                    responseMessage = await client.PostAsync(RecordDomainUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                }
                catch (Exception ex)
                {
                    if (responseMessage == null)
                    {
                        responseMessage = new HttpResponseMessage();
                    }

                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    responseMessage.ReasonPhrase = string.Format("PostDomainRecord failed: {0}", ex);
                }

                return responseMessage;
            }
        }
    }
}