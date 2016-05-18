namespace Merchello.FastTrack.Factories
{
    using Core.Models;
    using Merchello.FastTrack.Models.Membership;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory responsible for creating <see cref="ICustomerProfile"/>.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of <see cref="NewMemberModel"/>
    /// </typeparam>
    public class NewMemberModelFactory<TModel> : CustomerProfileModelFactory<TModel>
        where TModel : NewMemberModel, new()
    {
        /// <summary>
        /// Overrides the creation of the <see cref="ICustomerProfile"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="NewMemberModel"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="NewMemberModel"/>.
        /// </returns>
        protected override TModel OnCreate(TModel model, CustomerBase customer)
        {
            model.MemberTypeAlias = "merchelloCustomer";
            model.PersistLogin = true;

            return base.OnCreate(model, customer);
        }
    }
}