namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Umbraco.Core;
    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// Represents a discount constraint resolver.
    /// </summary>
    internal class OfferComponentResolver : ResolverBase<OfferComponentResolver>, IOfferComponentResolver
    {
        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes = new List<Type>();

        /// <summary>
        /// The <see cref="OfferProviderResolver"/>.
        /// </summary>
        private readonly IOfferProviderResolver _offerProviderResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public OfferComponentResolver(IEnumerable<Type> values)
            : this(values, OfferProviderResolver.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="offerProviderResolver">
        /// The offer provider resolver.
        /// </param>
        internal OfferComponentResolver(IEnumerable<Type> values, IOfferProviderResolver offerProviderResolver)
        {
            Ensure.ParameterNotNull(offerProviderResolver, "offerProviderResolver");
            _instanceTypes = values.ToList();
            _offerProviderResolver = offerProviderResolver;
        }

        /// <summary>
        /// Gets the instance types.
        /// </summary>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal IEnumerable<Type> InstanceTypes
        {
            get
            {
                return _instanceTypes;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="OfferComponentBase"/> by the provider key.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferComponentBase}"/>.
        /// </returns>
        public IEnumerable<OfferComponentBase> GetOfferComponentsByProviderKey(Guid providerKey)
        {
            var provider = _offerProviderResolver.GetByKey(providerKey);
            if (provider == null) return Enumerable.Empty<OfferComponentBase>();

            var types = this.GetTypesRespectingRestriction(provider.ManagesTypeName);

            return types.Select(x => GetOfferComponent(CreateEmptyOfferComponentDefinition(x))).Where(x => x != null);
        }

        /// <summary>
        /// Gets an instantiated offer component by it's definition.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentBase"/>.
        /// </returns>
        public OfferComponentBase GetOfferComponent(OfferComponentDefinition definition)
        {
            var type = this.GetTypeByComponentKey(definition.ComponentKey);
            var ctlArgs = new object[] { definition };

            var attempt = ActivatorHelper.CreateInstance<OfferComponentBase>(type, ctlArgs);

            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Gets the collection of offer components.
        /// </summary>
        /// <param name="definitions">
        /// The definitions.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferComponentBase}"/>.
        /// </returns>
        public IEnumerable<OfferComponentBase> GetOfferComponents(IEnumerable<OfferComponentDefinition> definitions)
        {
            return definitions.Select(this.GetOfferComponent);
        }

        /// <summary>
        /// Finds a type by the key assigned in the <see cref="OfferComponentAttribute"/>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public Type GetTypeByComponentKey(Guid key)
        {
            return _instanceTypes.FirstOrDefault(x => x.GetCustomAttribute<OfferComponentAttribute>(false).Key == key);
        }

        /// <summary>
        /// The get offer component.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <typeparam name="T">
        /// The type of component to be returned
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <remarks>
        /// TODO decide if this should be removed - probably be more useful if it did not require a definition
        /// and simply returned an new instantion of the type T
        /// </remarks>
        internal T GetOfferComponent<T>(OfferComponentDefinition definition) where T : OfferComponentBase
        {
            var typeOfT = typeof(T);
            var type = _instanceTypes.FirstOrDefault(x => x.IsAssignableFrom(typeOfT));

            if (type == null) return null;

            var ctlArgs = new object[] { definition };

            var attempt = ActivatorHelper.CreateInstance<OfferComponentBase>(type, ctlArgs);

            return attempt.Success ? attempt.Result as T : null;

        }

        /// <summary>
        /// The get types respecting restriction.
        /// </summary>
        /// <param name="restrictedTypeName">
        /// The restricted type name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        private IEnumerable<Type> GetTypesRespectingRestriction(string restrictedTypeName)
        {
            return
                _instanceTypes.Where(
                    x =>
                    x.GetCustomAttribute<OfferComponentAttribute>(false).RestrictToType == null
                    || x.GetCustomAttribute<OfferComponentAttribute>(false)
                           .RestrictToType.Name.Equals(restrictedTypeName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Creates an empty component.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentDefinition"/>.
        /// </returns>
        private OfferComponentDefinition CreateEmptyOfferComponentDefinition(Type type)
        {
            var att = type.GetCustomAttribute<OfferComponentAttribute>(false);

            var configuration = new OfferComponentConfiguration()
                                    {                                        
                                        TypeFullName = type.FullName,
                                        ComponentKey = att.Key,
                                        Values = Enumerable.Empty<KeyValuePair<string, string>>()
                                    };

            return new OfferComponentDefinition(configuration);
        }
    }
}