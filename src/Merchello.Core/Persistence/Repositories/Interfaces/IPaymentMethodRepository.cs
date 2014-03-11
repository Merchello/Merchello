using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the PaymentMethodRepository
    /// </summary>
    internal interface IPaymentMethodRepository : IRepositoryQueryable<Guid, IPaymentMethod>
    { }
}