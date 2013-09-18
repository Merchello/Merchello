﻿using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    public interface IWarehouseRepository : IRepositoryQueryable<int, IWarehouse>
    {
       
    }
}
