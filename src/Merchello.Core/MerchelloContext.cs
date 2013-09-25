using System;
using System.Threading;
using Merchello.Core.Configuration;
using Merchello.Core.Gateway;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class MerchelloContext : IDisposable
    {
        internal MerchelloContext(IServiceContext serviceContext)
            : this(serviceContext, ApplicationContext.Current.ApplicationCache)
        {}

        internal MerchelloContext(IServiceContext serviceContext, CacheHelper cache)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            Mandate.ParameterNotNull(cache, "cache");

            
            _services = serviceContext;
            Cache = cache;

        }

        /// <summary>
        /// Creates a basic basic context
        /// </summary>
        /// <param name="cache"></param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal MerchelloContext(CacheHelper cache)
        {
            Cache = cache;
        }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static MerchelloContext Current { get; internal set; }

        /// <summary>
        /// Returns the application wide cache accessor
        /// </summary>
        /// <remarks>
        /// This is generally a short cut to the ApplicationContext.Current.ApplicationCache
        /// </remarks>
        internal CacheHelper Cache { get; private set; }

        

        // IsReady is set to true by the boot manager once it has successfully booted
        // note - the original umbraco module checks on content.Instance in umbraco.dll
        //   now, the boot task that setup the content store ensures that it is ready
        bool _isReady = false;
        readonly ManualResetEventSlim _isReadyEvent = new System.Threading.ManualResetEventSlim(false);
        private IServiceContext _services;
        private IGatewayContext _gatewayContext;

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
                    return MerchelloConfiguration.Current.Section.Version;
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
                    throw new InvalidOperationException("The ServiceContext has not been set on the MerchelloContext");
                return _services;
            }
            internal set { _services = value; }
        }

        /// <summary>
        /// Gets the current GatewayProviderRegistry
        /// </summary>
        public IGatewayContext GatewayContext
        {
            get
            {
                if(_gatewayContext == null)
                    throw new InvalidOperationException("GatewayProvider have not been set on the MerchelloContext");
                return _gatewayContext;
            }
            internal set { _gatewayContext = value; }
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
                _disposed = true;
            }
        }
    }
}
