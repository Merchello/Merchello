using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Gateway
{
    public abstract class PaymentGatewayProviderBase : IPaymentGatewayProvider
    {

        public virtual IGatewayResponse Send()
        {
           throw new NotImplementedException();
        }
        
        public abstract PaymentResponseCode ResponseCode { get; set; }

        public void Dispose()
        { }

    }
}
