﻿using System;
using System.Threading;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class MerchelloContext : IMerchelloContext
    {
        internal MerchelloContext(IServiceContext serviceContext, IGatewayContext gatewayContext)
            : this(serviceContext, gatewayContext, ApplicationContext.Current.ApplicationCache)
        {}

        internal MerchelloContext(IServiceContext serviceContext, IGatewayContext gatewayContext, CacheHelper cache)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            Mandate.ParameterNotNull(gatewayContext, "gatewayContext");
            Mandate.ParameterNotNull(cache, "cache");
            
            _services = serviceContext;
            _gateways = gatewayContext;
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
                    var currentVersion = MerchelloVersion.Current.ToString();


                    if (currentVersion != configVersion)
                    {
                        LogHelper.Info<ApplicationContext>("CurrentVersion different from configurationStatus: '" + currentVersion + "','" + configVersion + "'");
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
                    return MerchelloConfiguration.ConfigurationStatus;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        private IServiceContext _services;
        private IGatewayContext _gateways;

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
