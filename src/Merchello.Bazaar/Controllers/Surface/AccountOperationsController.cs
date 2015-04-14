namespace Merchello.Bazaar.Controllers.Surface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    using Merchello.Bazaar.Models.Account;
    using Merchello.Core.Models;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The account operations controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class AccountOperationsController : SurfaceControllerBase
    {
        /// <summary>
        /// The link to receipt.
        /// </summary>
        /// <param name="receiptContentId">
        /// The id of the receipt page
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult LinkToReceipt(int receiptContentId, Guid invoiceKey)
        {
            this.CustomerContext.SetValue("invoiceKey", invoiceKey.ToString());
            return this.RedirectToUmbracoPage(receiptContentId);
        }

        /// <summary>
        /// The render account profile form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderAccountProfileForm(AccountProfileModel model)
        {

            return this.PartialView(PathHelper.GetThemePartialViewPath(model.Theme, "ProfileForm"), model);
        }

        /// <summary>
        /// The update account profile.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult UpdateAccountProfile(AccountProfileModel model)
        {
            if (model.SetPassword && !this.ModelState.IsValid) return this.CurrentUmbracoPage();

            if (!(this.ModelState.IsValidField("firstName") && this.ModelState.IsValidField("lastName"))) return this.CurrentUmbracoPage();

            var customer = (ICustomer)this.CurrentCustomer;
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;

            this.MerchelloServices.CustomerService.Save(customer);

            if (model.SetPassword)
            {
                var member = Membership.GetUser();
                if (member == null)
                {
                    var nullReference = new NullReferenceException("Current member was null in change password operation");
                    LogHelper.Error<AccountOperationsController>("Attempt to change password failed", nullReference);
                    throw nullReference;
                }

                member.ChangePassword(model.OldPassword, model.Password); 
            }

            return this.SuccessfulRedirect(model.AccountPageId);
        }

        /// <summary>
        /// The render customer address form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderCustomerAddressForm(CustomerAddressModel model)
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(model.Theme, "CustomerAddressForm"), model);   
        }

        /// <summary>
        /// The save customer address.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult SaveCustomerAddress(CustomerAddressModel model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();
            ICustomerAddress customerAddress;
            if (!model.Key.Equals(Guid.Empty))
            {
                var existing = this.MerchelloServices.CustomerService.GetAddressByKey(model.Key);
                customerAddress = model.AsCustomerAddress(existing);
            }
            else
            {
                customerAddress = model.AsCustomerAddress();
            }

            this.MerchelloServices.CustomerService.Save(customerAddress);
            this.CustomerContext.Reinitialize(this.CurrentCustomer);
            return this.SuccessfulRedirect(model.AccountPageId);
        }

        /// <summary>
        /// The delete customer address.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="customerAddressKey">
        /// The customer address key.
        /// </param>
        /// <param name="accountPageId">
        /// The account page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult DeleteCustomerAddress(Guid customerKey, Guid customerAddressKey, int accountPageId)
        {
            var customer = this.MerchelloServices.CustomerService.GetByKey(customerKey);
            var address = customer.Addresses.FirstOrDefault(x => x.Key == customerAddressKey);
            if (address != null)
            {
                customer.DeleteCustomerAddress(address);
                this.CustomerContext.Reinitialize(customer);
            }

            return this.SuccessfulRedirect(accountPageId);
        }

        /// <summary>
        /// The set default address.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="customerAddressKey">
        /// The customer address key.
        /// </param>
        /// <param name="accountPageId">
        /// The account page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult SetDefaultAddress(Guid customerKey, Guid customerAddressKey, int accountPageId)
        {
            var customer = this.MerchelloServices.CustomerService.GetByKey(customerKey);
            var address = customer.Addresses.FirstOrDefault(x => x.Key == customerAddressKey);
            if (address != null)
            {
                var addresses = new List<ICustomerAddress>();
                foreach (var adr in customer.Addresses.Where(x => x.AddressType == address.AddressType && x.Key != address.Key))
                {
                    adr.IsDefault = false;
                    addresses.Add(adr);
                }

                address.IsDefault = true;
                this.MerchelloServices.CustomerService.Save(addresses);
                this.CustomerContext.Reinitialize(customer);
            }

            return this.SuccessfulRedirect(accountPageId);
        }

        /// <summary>
        /// The successful redirect.
        /// </summary>
        /// <param name="accountPageId">
        /// The account page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        private ActionResult SuccessfulRedirect(int accountPageId)
        {
            var accountPage = this.Umbraco.TypedContent(accountPageId);

            return this.Redirect(accountPage.Url + "#success");
        }
    }
}