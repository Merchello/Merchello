namespace Merchello.Implementation.Factories
{
    using Core.Models;
    using Merchello.Implementation.Models;

    /// <summary>
    /// Overrides the default factory settings to use first name and last name to the <see cref="IAddress"/> Name field.
    /// </summary>
    public class CheckoutBillingAddressModelFactory : CheckoutAddressModelFactory<CheckoutBillingAddressModel>
    {
        /// <summary>
        /// Overrides model creation.
        /// </summary>
        /// <param name="address">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="CheckoutBillingAddressModel"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="IAddress"/>.
        /// </returns>
        protected override IAddress OnCreate(IAddress address, CheckoutBillingAddressModel adr)
        {
            var result = base.OnCreate(address, adr);

            result.Name = string.Format("{0} {1}", adr.FirstName, adr.Label);

            return base.OnCreate(result, adr);
        }
    }
}