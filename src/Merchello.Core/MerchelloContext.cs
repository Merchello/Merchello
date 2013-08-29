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
    public class MerchelloContext : IDisposable
    {

        internal MerchelloContext(IServiceContext serviceContext, CacheHelper cache)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            //Mandate.ParameterNotNull(cache, "cache");

            _services = serviceContext;
            PluginCache = cache;

        }

        /// <summary>
        /// Creates a basic plugin context
        /// </summary>
        /// <param name="cache"></param>
        internal MerchelloContext(CacheHelper cache)
        {
            PluginCache = cache;
        }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static MerchelloContext Current { get; internal set; }

        /// <summary>
        /// Returns the application wide cache accessor
        /// </summary>
        /// <remarks>
        /// Any caching that is done in the package (Merchello wide) should be done through this property
        /// </remarks>
        internal CacheHelper PluginCache { get; private set; }

        

        // IsReady is set to true by the boot manager once it has successfully booted
        // note - the original umbraco module checks on content.Instance in umbraco.dll
        //   now, the boot task that setup the content store ensures that it is ready
        bool _isReady = false;
        readonly System.Threading.ManualResetEventSlim _isReadyEvent = new System.Threading.ManualResetEventSlim(false);
        private IServiceContext _services;

        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            internal set
            {
                AssertIsNotReady();
                _isReady = value;
                _isReadyEvent.Set();
            }
        }

        public bool WaitForReady(int timeout)
        {
            return _isReadyEvent.WaitHandle.WaitOne(timeout);
        }


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
                    return MerchelloConfiguration.Section.Version;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        private void AssertIsNotReady()
        {
            if (IsReady)
                throw new Exception("MerchelloPluginContext has already been initialized.");
        }

        /// <summary>
        /// Gets the current ServiceContext
        /// </summary>
        /// <remarks>
        /// Internal set is generally only used for unit tests
        /// </remarks>
        public IServiceContext Services
        {
            get
            {
                if (_services == null)
                    throw new InvalidOperationException("The ServiceContext has not been set on the MerchelloPluginContext");
                return _services;
            }
            internal set { _services = value; }
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
