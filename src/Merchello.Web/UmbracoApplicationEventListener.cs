using log4net;
using Merchello.Core;
using System;
using System.Reflection;
using Umbraco.Core;

namespace Merchello.Web
{
    /// <summary>
    /// Listens for the Umbraco Application "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventListener : ApplicationEventHandler
    {
        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );

        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            // Initialize Merchello
            Log.Info("Attempting to initialize Merchello");
            try
            {
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");                
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }
        }

        //protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        //{
        //    base.ApplicationStarted(umbracoApplication, applicationContext);

        //    // TODO why doesn't this fire
        //    var unitOfWorkProvider = new PetaPocoUnitOfWorkProvider();
        //    Log.Info("Checking Merchello DB Schema");

        //    try
        //    {
        //       // var success = DatabaseSchemaHelper.VerifyDatabaseSchema(unitOfWorkProvider.GetUnitOfWork().Database);
        //    }
        //    catch (Exception ex)
        //    {                
        //        Log.Error("Merchello Database Schema Verification Failed", ex);
        //    }
            
        //}
        
    }
}
