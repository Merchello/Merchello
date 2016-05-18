namespace Merchello.Web.Factories
{
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory responsible for building <see cref="ICustomerProfile"/>.
    /// </summary>
    /// <typeparam name="TProfile">
    /// The type of <see cref="ICustomerProfile"/>
    /// </typeparam>
    public class CustomerProfileModelFactory<TProfile>
        where TProfile : class, ICustomerProfile, new()
    {
        /// <summary>
        /// Creates a <see cref="ICustomerProfile"/>.
        /// </summary>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TProfile"/>.
        /// </returns>
        public TProfile Create(CustomerBase customer)
        {
            var profile = new TProfile();

            if (!customer.IsAnonymous)
            {
                var c = (ICustomer)customer;
                profile.FirstName = c.FirstName;
                profile.LastName = c.LastName;
                profile.Email = c.Email;
            }

            return OnCreate(profile, customer);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICustomerProfile"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICustomerProfile"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICustomerProfile"/>.
        /// </returns>
        protected virtual TProfile OnCreate(TProfile model, CustomerBase customer)
        {
            return model;
        }
    }
}