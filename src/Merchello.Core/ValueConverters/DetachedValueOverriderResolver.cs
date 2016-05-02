namespace Merchello.Core.ValueConverters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Merchello.Core.ValueConverters.ValueOverrides;

    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// A resolver for resolving detached value overrides.
    /// </summary>
    internal class DetachedValueOverriderResolver : ResolverBase<DetachedValueOverriderResolver>
    {
        /// <summary>
        /// The activated gateway provider cache.
        /// </summary>
        private readonly ConcurrentDictionary<string, IDetachedValueOverrider> _overrideCache = new ConcurrentDictionary<string, IDetachedValueOverrider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedValueOverriderResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public DetachedValueOverriderResolver(IEnumerable<Type> values)
        {
            BuildCache(values);
        }

        /// <summary>
        /// Gets the <see cref="IDetachedValueOverrider"/> by property editor alias.
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedValueOverrider"/>.
        /// </returns>
        public IDetachedValueOverrider GetFor(string propertyEditorAlias)
        {
            return _overrideCache.FirstOrDefault(x => x.Key == propertyEditorAlias).Value;
        }

        /// <summary>
        /// Builds the type cache.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        private void BuildCache(IEnumerable<Type> values)
        {
            foreach (var attempt in values.Select(type => ActivatorHelper.CreateInstance<DetachedValueOverriderBase>(type, new Type[] { })).Where(attempt => attempt.Success))
            {
                this.AddOrUpdateCache(attempt.Result);
            }
        }

        /// <summary>
        /// The add or update cache.
        /// </summary>
        /// <param name="overrider">
        /// The <see cref="IDetachedValueOverrider"/>.
        /// </param>
        private void AddOrUpdateCache(IDetachedValueOverrider overrider)
        {
            var att = overrider.GetType().GetCustomAttribute<DetachedValueOverriderAttribute>(false);
            if (att != null)
            {
                _overrideCache.AddOrUpdate(att.PropertyEditorAlias, overrider, (x, y) => overrider);
            }
        }
    }
}