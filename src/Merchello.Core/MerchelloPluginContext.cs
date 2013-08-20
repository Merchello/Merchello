using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Merchello.Core.Configuration;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class MerchelloPluginContext : IDisposable
    {

        internal MerchelloPluginContext(ServiceContext serviceContext, CacheHelper cache)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");

            _services = serviceContext;
            PluginCache = cache;

        }


        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static MerchelloPluginContext Current { get; internal set; }

        /// <summary>
        /// Returns the application wide cache accessor
        /// </summary>
        /// <remarks>
        /// Any caching that is done in the package (Merchello wide) should be done through this property
        /// </remarks>
        public CacheHelper PluginCache { get; private set; }

        private ServiceContext _services;

        /// <summary>
        /// Compares the binary version to that listed in the Merchello configuration to determine if the 
        /// package was upgraded
        /// </summary>
        public bool IsConfigured
        {
            get
            {
                try
                {
                    string configVersion = ConfigurationVersion;
                    string currentVersion = MerchelloVersion.Current.ToString(3);


                    if (currentVersion != configVersion)
                    {
                        LogHelper.Info<ApplicationContext>("CurrentVersion different from configStatus: '" + currentVersion + "','" + configVersion + "'");
                    }

                    return (configVersion == currentVersion);
                }
                catch
                {                    
                    return false;
                }
            }
        }


        private static string ConfigurationVersion
        {
            get
            {
                try
                {
                    return PluginConfiguration.Section.Version;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        private volatile bool _disposed;
        private readonly ReaderWriterLockSlim _disposalLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// This will dispose and reset all resources used to run the Merchello
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            using (new WriteLock(_disposalLocker))
            { 

                // clear the cache
                if (PluginCache != null)
                {
                    PluginCache.ClearAllCache();
                }

                // reset the instance objects
                this.PluginCache = null;

                _disposed = true;
            }
        }
    }
}
