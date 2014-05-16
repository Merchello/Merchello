﻿using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker Interface for the customer repository
    /// </summary>
    internal interface ICustomerRepository : IRepositoryQueryable<Guid, ICustomer>
    {
        
        /// <summary>
        /// Returns a <see cref="ICustomer"/> given an Umbraco Member Id or null if not found.
        /// </summary>
        ICustomer GetByMemberId(int? memberId);


        /// <summary>
        /// Returns a <see cref="ICustomer"/> by it's entity key
        /// </summary>
        /// <param name="entityKey">Guid</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer GetByEntityKey(Guid entityKey);

    }
}
