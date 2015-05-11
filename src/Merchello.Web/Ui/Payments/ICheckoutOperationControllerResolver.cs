namespace Merchello.Web.Ui.Payments
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the CheckoutOperationControllerResolver.
    /// </summary>
    internal interface ICheckoutOperationControllerResolver
    {
        /// <summary>
        /// Returns a list of all types resolved.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        IEnumerable<Type> GetAllTypes();

        /// <summary>
        /// Gets a type that has the 
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        Type GetTypeByGatewayMethodUiAlias(string alias);

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