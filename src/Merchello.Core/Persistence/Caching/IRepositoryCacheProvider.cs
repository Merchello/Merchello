﻿using System;
using System.Collections.Generic;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Persistence.Caching
{
    /// <summary>
    /// Defines the implementation of a Cache Provider intented to back a repository
    /// </summary>
    internal interface IRepositoryCacheProvider
    {
        /// <summary>
        /// Gets an EntityEntity from the cache by Type and Id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IEntity GetById(Type type, Guid id);

        /// <summary>
        /// Gets an EntityEntity from the cache by Type and Ids
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<IEntity> GetByIds(Type type, List<Guid> ids);

        /// <summary>
        /// Gets all Entities of specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<IEntity> GetAllByType(Type type);

        /// <summary>
        /// Saves the EntityEntity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        void Save(Type type, IEntity entity);

        /// <summary>
        /// Deletes the EntityEntity from the cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        void Delete(Type type, IEntity entity);

        /// <summary>
        /// Clears the cache by type
        /// </summary>
        /// <param name="type"></param>
        void Clear(Type type);
    }
}