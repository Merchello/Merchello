namespace Merchello.Web.Ui
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Defines the PaymentMethodUiControllerResolver.
    /// </summary>
    public interface IPaymentMethodUiControllerResolver
    {       
        /// <summary>
        /// The get url action parameters by GatewayMethodUi alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="UrlActionParams"/>.
        /// </returns>
        UrlActionParams GetUrlActionParamsByGatewayMethodUiAlias(string alias);

        /// <summary>
        /// Returns a value indicating whether or not a type was found with the GatewayMethodUi alias
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// A value indicating whether or not a type was found with the GatewayMethodUi alias
        /// </returns>
        bool HasTypeWithGatewayMethodUiAlias(string alias);
    }
}