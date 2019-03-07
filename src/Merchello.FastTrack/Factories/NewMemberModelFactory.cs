namespace Merchello.FastTrack.Factories
{
    using System;
    using System.Web.Security;

    using Core.Models;
    using Merchello.FastTrack.Models.Membership;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Security;

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
        /// Creates a <see cref="NewMemberModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="NewMemberModel"/>.
        /// </param>
        /// <param name="status">
        /// The <see cref="MembershipCreateStatus"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TModel"/> created from <see cref="MembershipCreateStatus"/>.
        /// </returns>
        public TModel Create(TModel model, MembershipCreateStatus status)
        {
            model.ViewData = new StoreViewData();

            switch (status)
            {
                case MembershipCreateStatus.InvalidPassword:
                    model.ViewData.Exception = new Exception("Invalid password");
                    model.ViewData.Success = false;
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    model.ViewData.Exception = new Exception("The email address " + model.Email + " is already associated with a customer.");
                    model.ViewData.Success = false;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    model.ViewData.Exception = new Exception("The email address " + model.Email + " is already associated with a customer.");
                    model.ViewData.Success = false;
                    break;
                default:
                    model.ViewData.Success = true;
                    break;
            }

            if (model.ViewData.Exception != null) model.ViewData.Messages = new[] { model.ViewData.Exception.Message };

            return OnCreate(model, status);
        }

        /// <summary>
        /// Allows for overriding the creation of the <see cref="NewMemberModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="NewMemberModel"/>.
        /// </param>
        /// <param name="status">
        /// The <see cref="MembershipCreateStatus"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TModel"/> created from <see cref="MembershipCreateStatus"/>.
        /// </returns>
        protected virtual TModel OnCreate(TModel model, MembershipCreateStatus status)
        {
            return model;
        }

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
        protected override TModel OnCreate(TModel model, ICustomerBase customer)
        {
            model.MemberTypeAlias = "merchelloCustomer";
            model.PersistLogin = true;
            model.TermsAndConditions = false;
            model.ViewData = new StoreViewData();

            return base.OnCreate(model, customer);
        }
    }
}