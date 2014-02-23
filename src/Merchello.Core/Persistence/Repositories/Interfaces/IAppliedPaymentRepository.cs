using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the AppliedPaymentRepository
    /// </summary>
    internal interface IAppliedPaymentRepository : IRepositoryQueryable<Guid, IAppliedPayment>
    { }
}