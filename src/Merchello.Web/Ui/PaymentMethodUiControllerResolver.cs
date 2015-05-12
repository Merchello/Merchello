namespace Merchello.Web.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core.Gateways;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.ObjectResolution;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Resolves CheckoutOperationControllers
    /// </summary>
    public class PaymentMethodUiControllerResolver : ResolverBase<PaymentMethodUiControllerResolver>, IPaymentMethodUiControllerResolver
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
        /// Initializes a new instance of the <see cref="PaymentMethodUiControllerResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public PaymentMethodUiControllerResolver(IEnumerable<Type> value)
        {
            var enumerable = value as Type[] ?? value.ToArray();

            this._instanceTypes = enumerable.ToList();

            this.Initialize();
        }

        /// <summary>
        /// Returns a list of all types resolved.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        internal IEnumerable<Type> GetAllTypes()
        {
            return this._instanceTypes;
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
        internal Type GetTypeByGatewayMethodUiAlias(string alias)
        {
            return this.HasTypeWithGatewayMethodUiAlias(alias) ? this._gatewayMethods[alias] : null;
        }

        /// <summary>
        /// The get url action params by gateway method ui alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="UrlActionParams"/>.
        /// </returns>
        public UrlActionParams GetUrlActionParamsByGatewayMethodUiAlias(string alias)
        {
            var type = this.GetTypeByGatewayMethodUiAlias(alias);
            if (type == null) return null;

            var urlActionParams = new UrlActionParams()
                                      {
                                          Controller = type.Name.EndsWith("Controller") ? type.Name.Remove(type.Name.LastIndexOf("Controller", StringComparison.Ordinal), 10) : type.Name,
                                          Method = "RenderForm"
                                      };

            var att = type.GetCustomAttribute<PluginControllerAttribute>(false);
            if (att != null)
            {
                urlActionParams.RouteParams.Add(new Tuple<string, string>("area", att.AreaName));
            }

            return urlActionParams;
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
            return this._gatewayMethods.ContainsKey(alias);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            foreach (var t in this._instanceTypes)
            {
                var att = t.GetCustomAttribute<GatewayMethodUiAttribute>(false);
                if (att == null) continue;

                if (!this._gatewayMethods.ContainsKey(att.Alias))
                {
                    this._gatewayMethods.Add(att.Alias, t);
                }
                else
                {
                    LogHelper.Info<PaymentMethodUiControllerResolver>("More than one GatewayMethodUiAttribute was found with the same alias.  Aliases are intended to be unique.");
                }
            }
        }
    }
}