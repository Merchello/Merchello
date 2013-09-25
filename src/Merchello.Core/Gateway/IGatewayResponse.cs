using System.Collections.Generic;

namespace Merchello.Core.Gateway
{
    /// <summary>
    /// Represents the raw response from a gateway provider
    /// </summary>
    public interface IGatewayResponse
    {
        bool IsError { get; set; }

        IDictionary<string, object> Values { get; set; }  
    }
}
