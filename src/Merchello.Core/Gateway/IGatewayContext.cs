using System;

namespace Merchello.Core.Gateway
{
    public interface IGatewayContext
    {
        IGatewayProvider GetByKey(Guid key);   
        void Refresh();
    }
}