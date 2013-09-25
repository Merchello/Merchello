using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Gateway
{
     public enum PaymentResponseCode
     {
         Approved,
         Declined,
         Error,
         Fraud
     }
}
