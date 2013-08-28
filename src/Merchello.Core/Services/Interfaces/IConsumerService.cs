using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    public interface IConsumerService : IService
    {
        /// <summary>
        /// The service associated with Anonymous consumers
        /// </summary>
        IAnonymousCustomerService Anonymous { get; }

        /// <summary>
        /// The service associated with customer consumers
        /// </summary>
        ICustomerService Customer { get; }
    }
}
