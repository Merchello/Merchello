using System;
using System.Security;
using Umbraco.Core.Logging;

namespace Merchello.Examine.DataServices
{
    public class MerchelloLogService : ILogService
    {
        public string ProviderName { get; set; }

        [SecuritySafeCritical]

        public void AddErrorLog(int nodeId, string msg, Exception ex)
        {
            LogHelper.Error<MerchelloLogService>(string.Format("Provider={0}, NodeId={1}, {2}", ProviderName, nodeId, msg), ex);
        }

        [SecuritySafeCritical]
        public void AddInfoLog(int nodeId, string msg)
        {
            LogHelper.Info<MerchelloLogService>("{0}, Provider={1}, NodeId={2}", () => msg, () => ProviderName, () => nodeId);
        }

        [SecuritySafeCritical]
        public void AddVerboseLog(int nodeId, string msg)
        {
            LogHelper.Debug<MerchelloLogService>("{0}, Provider={1}, NodeId={2}", () => msg, () => ProviderName, () => nodeId);
        }
    }
}