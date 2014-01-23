using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the payment repository
    /// </summary>
    internal interface IPaymentRepository : IRepositoryQueryable<Guid, IPayment>
    {
    }
}
