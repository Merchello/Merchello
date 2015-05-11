namespace Merchello.Web.Ui.Payments
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core.Gateways;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// Resolves CheckoutOperationControllers
    /// </summary>
    internal class CheckoutOperationControllerResolver : ResolverBase<CheckoutOperationControllerResolver>, ICheckoutOperationControllerResolver
    {
        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes = new List<Type>();

        /// <summary>
        /// Types which have the GatewayMethodUi attribute.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly Dictionary<string, Type> _gatewayMethods = new Dictionary<string, Type>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutOperationControllerResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public CheckoutOperationControllerResolver(IEnumerable<Type> value)
        {
            var enumerable = value as Type[] ?? value.ToArray();

            _instanceTypes = enumerable.ToList();

            Initialize();
        }

        /// <summary>
        /// Returns a list of all types resolved.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        public IEnumerable<Type> GetAllTypes()
        {
            return _instanceTypes;
        }

        /// <summary>
        /// Gets a type that has the 
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public Type GetTypeByGatewayMethodUiAlias(string alias)
        {
            return HasTypeWithGatewayMethodUiAlias(alias) ? _gatewayMethods[alias] : null;
        }

        /// <summary>
        /// Returns a value indicating whether or not a type was found with the GatewayMethodUi alias
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// A value indicating whether or not a type was found with the GatewayMethodUi alias
        /// </returns>
        public bool HasTypeWithGatewayMethodUiAlias(string alias)
        {
            return _gatewayMethods.ContainsKey(alias);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            foreach (var t in _instanceTypes)
            {
                var att = t.GetCustomAttribute<GatewayMethodUiAttribute>(false);
                if (att == null) continue;

                if (!_gatewayMethods.ContainsKey(att.Alias))
                {
                    _gatewayMethods.Add(att.Alias, t);
                }
                else
                {
                    LogHelper.Info<CheckoutOperationControllerResolver>("More than one GatewayMethodUiAttribute was found with the same alias.  Aliases are intended to be unique.");
                }
            }
        }
    }
}