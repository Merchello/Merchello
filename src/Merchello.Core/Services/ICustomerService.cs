using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the CustomerService, which provides access to operations involving <see cref="ICustomer"/>
    /// </summary>
    public interface ICustomerService : IService
    {

        /// <summary>
        /// Gets an <see cref="ICustomer"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid pk of the Customer to retrieve</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer GetById(Guid key);
    }
}
