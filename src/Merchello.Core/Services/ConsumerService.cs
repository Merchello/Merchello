using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Services
{
    public class ConsumerService : IConsumerService
    {
        private CustomerService _customerService;


        public IAnonymousCustomerService Anonymous
        {
            get { throw new NotImplementedException(); }
        }

        public ICustomerService Customer
        {
            get { throw new NotImplementedException(); }
        }
    }
}
