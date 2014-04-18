using System;
using System.Threading;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class MerchelloContext : IMerchelloContext
    {
        internal MerchelloContext(IServiceContext serviceContext)
            : this(serviceContext, ApplicationContext.Current.ApplicationCache)
        {}

        internal MerchelloContext(IServiceContext serviceContext, CacheHelper cache)
            : this(serviceContext, cache, false)
        { }

        internal MerchelloContext(IServiceContext serviceContext, CacheHelper cache, bool isUnitTest)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            Mandate.ParameterNotNull(cache, "cache");
            
            _services = serviceContext;
            Cache = cache;

            BuildMerchelloContext(isUnitTest);
        }

        /// <summary>
        /// Builds the MerchelloContext internals
        /// </summary>
        /// <param name="isUnitTest">True/false indicating whether or not is being called by certain unit tests</param>
        private void BuildMerchelloContext(bool isUnitTest)
        {
            if (isUnitTest) return;

            var gatewayResolver = new Lazy<GatewayProviderResolver>(() => new GatewayProviderResolver(_services.GatewayProviderService, Cache.RuntimeCache));

            _gateways = new GatewayContext(
                new ShippingContext(_services.GatewayProviderService, _services.StoreSettingService, gatewayResolver.Value),
                new TaxationContext(_services.GatewayProviderService, gatewayResolver.Value),
                new PaymentContext(_services.GatewayProviderService, gatewayResolver.Value)
                );
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
        public CacheHelper Cache { get; private set; }

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
                    var configVersion = ConfigurationVersion;
                    var currentVersion = MerchelloVersion.Current.ToString(3);


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

        private IServiceContext _services;
        private IGatewayContext _gateways;
        //private ISalesManager _salesManager;


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
        /// Gets the current GatewayContext
        /// </summary>
        public IGatewayContext Gateways
        {
            get
            {
                if(_gateways == null)
                    throw new InvalidOperationException("The GatewayContext has not been set on the MerchelloContext");
                return _gateways;
            }
            internal set { _gateways = value; }
        }
       
        ///// <summary>
        ///// Gets the sales manager
        ///// </summary>
        //public ISalesManager SalesManager
        //{
        //    get
        //    {
        //        if(_salesManager == null)
        //            throw new InvalidOperationException("The SalesManager has not been set of the MerchelloContext");
        //        return _salesManager;
        //    }
        //    internal set { _salesManager = value; }
        //}

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
