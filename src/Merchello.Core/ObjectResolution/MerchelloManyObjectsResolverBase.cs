﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.ObjectResolution
{
    /// <summary>
    /// The base class for Merchello many-object resolvers.
    /// </summary>
    /// <typeparam name="TResolver">The type of the concrete resolver class.</typeparam>
    /// <typeparam name="TResolved">The type of the resolved objects.</typeparam>
    /// <remarks>
    /// Umbraco's internal Resolution class and other internal methods on the standard ManyObjectsResolverBase makes
    /// it difficult to test/control object resolution - so we went this route for the time being.
    /// </remarks>
    internal abstract class MerchelloManyObjectsResolverBase<TResolver, TResolved> : ResolverBase<TResolver> 
        where TResolved : class
        where TResolver : ResolverBase
    {

        private readonly List<Type> _instanceTypes = new List<Type>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        protected MerchelloManyObjectsResolverBase(IEnumerable<Type> value)
        {
            _instanceTypes = value.ToList();
        }

        /// <summary>
        /// Gets the resolved object instances.
        /// </summary>
        protected abstract IEnumerable<TResolved> Values { get; }

        /// <summary>
        /// Creates the object instances for the types contained in the types collection.
        /// </summary>
        /// <returns>A list of objects of type <typeparamref name="TResolved"/>.</returns>
        protected virtual IEnumerable<TResolved> CreateInstances(object[] ctrArgs)
        {
            var instances = new List<TResolved>();

            foreach (var et in InstanceTypes)
            {
                var attempt = ActivatorHelper.CreateInstance<TResolved>(et, ctrArgs.ToArray());
                if (attempt.Success) instances.Add(attempt.Result);
            }

            return instances;
        }

        /// <summary>
        /// Gets the list of types to create instances from.
        /// </summary>
        protected virtual IEnumerable<Type> InstanceTypes
        {
            get { return _instanceTypes; }
        }

        /// <summary>
        /// Returns a WriteLock to use when modifying collections
        /// </summary>
        /// <returns></returns>
        protected WriteLock GetWriteLock()
        {
            return new WriteLock(_lock);
        }


        #region Types collection manipulation

        /// <summary>
        /// Removes a type.
        /// </summary>
        /// <param name="value">The type to remove.</param>
        /// <exception cref="InvalidOperationException">the resolver does not support removing types, or 
        /// the type is not a valid type for the resolver.</exception>
        public virtual void RemoveType(Type value)
        {
            using (var l = new UpgradeableReadLock(_lock))
            {

                l.UpgradeToWriteLock();
                _instanceTypes.Remove(value);
            }
        }

        /// <summary>
        /// Removes a type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        /// <exception cref="InvalidOperationException">the resolver does not support removing types, or 
        /// the type is not a valid type for the resolver.</exception>
        public void RemoveType<T>()
            where T : TResolved
        {
            RemoveType(typeof(T));
        }

        /// <summary>
        /// Adds types.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <remarks>The types are appended at the end of the list.</remarks>
        /// <exception cref="InvalidOperationException">the resolver does not support adding types, or 
        /// a type is not a valid type for the resolver, or a type is already in the collection of types.</exception>
        protected void AddTypes(IEnumerable<Type> types)
        {                        
            using (new WriteLock(_lock))
            {
                foreach (var t in types)
                {
                    if (_instanceTypes.Contains(t))
                    {
                        throw new InvalidOperationException(string.Format(
                            "Type {0} is already in the collection of types.", t.FullName));
                    }
                    _instanceTypes.Add(t);
                }
            }
        }

        /// <summary>
        /// Adds a type.
        /// </summary>
        /// <param name="value">The type to add.</param>
        /// <remarks>The type is appended at the end of the list.</remarks>
        /// <exception cref="InvalidOperationException">the resolver does not support adding types, or 
        /// the type is not a valid type for the resolver, or the type is already in the collection of types.</exception>
        public virtual void AddType(Type value)
        {
            
            using (var l = new UpgradeableReadLock(_lock))
            {
                if (_instanceTypes.Contains(value))
                {
                    throw new InvalidOperationException(string.Format(
                        "Type {0} is already in the collection of types.", value.FullName));
                }

                l.UpgradeToWriteLock();
                _instanceTypes.Add(value);
            }
        }

        /// <summary>
        /// Adds a type.
        /// </summary>
        /// <typeparam name="T">The type to add.</typeparam>
        /// <remarks>The type is appended at the end of the list.</remarks>
        /// <exception cref="InvalidOperationException">the resolver does not support adding types, or 
        /// the type is not a valid type for the resolver, or the type is already in the collection of types.</exception>
        public void AddType<T>()
            where T : TResolved
        {
            AddType(typeof(T));
        }

        #endregion

    }
}