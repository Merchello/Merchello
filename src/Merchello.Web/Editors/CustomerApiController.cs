namespace Merchello.Web.Editors
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Http;
    using System.Web.Security;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The customer API controller.
    /// </summary>
    [PluginController("Merchello")]
    public class CustomerApiController : MerchelloApiController
    {
        #region Fields

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The customer service.
        /// </summary>
        private ICustomerService _customerService;

        /// <summary>
        /// The customer address service.
        /// </summary>
        private ICustomerAddressService _customerAddressService;

        /// <summary>
        /// The membership member service.
        /// </summary>
        private IMemberService _memberService;
        

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        public CustomerApiController()
            : this(Core.MerchelloContext.Current, global::Umbraco.Core.ApplicationContext.Current.Services.MemberService)
        {     
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="memberService">
        /// The member Service.
        /// </param>
        public CustomerApiController(IMerchelloContext merchelloContext, IMemberService memberService)
            : base(merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _customerAddressService = ((Core.Services.ServiceContext)merchelloContext.Services).CustomerAddressService;
            _memberService = memberService;

            _merchello = new MerchelloHelper(merchelloContext, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        /// <param name="memberService">
        /// The member Service.
        /// </param>
        internal CustomerApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext, IMemberService memberService)
            : base(merchelloContext, umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _memberService = memberService;
        }

        #endregion

        /// <summary>
        /// Gets the avatar URL.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpGet]
        public object GetGravatarUrl(string email)
        {
            var hash = new StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(email));

            foreach (byte t in bytes)
            {
                hash.Append(t.ToString("x2"));
            }

            return new
                       {
                            gravatarUrl = string.Format("https://www.gravatar.com/avatar/{0}?d=mm", hash.ToString().ToLowerInvariant())
                       };
        }

        /// <summary>
        /// Returns an customer by id (key)
        /// 
        /// GET /umbraco/Merchello/CustomerApi/GetCustomer/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [HttpGet]
        public CustomerDisplay GetCustomer(Guid id)
        {
            return _merchello.Query.Customer.GetByKey(id);        
        }

        /// <summary>
        /// Gets a customer item cache for baskets and wish lists.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="itemCacheType">
        /// The item cache type.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerItemCacheDisplay"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Throws an exception if itemCacheType is not Basket or Wish list
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if a customer cannot be found for the key passed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throws and invalid operation exception if when a wish list is attempted to be retrieved from an anonymous customer.
        /// </exception>
        [HttpGet]
        public CustomerItemCacheDisplay GetCustomerItemCache(Guid customerKey, ItemCacheType itemCacheType)
        {
            if (itemCacheType != ItemCacheType.Basket && itemCacheType != ItemCacheType.Wishlist)
            {
                var notSupported = new NotSupportedException("Basket and Wishlist are the only supported item caches");
                LogHelper.Error<CustomerApiController>("Unsupported item cache type", notSupported);
                throw notSupported;
            }
            
            var customer = _customerService.GetAnyByKey(customerKey);

            if (customer == null) throw new NullReferenceException("customer for customer key was null");

            if (itemCacheType == ItemCacheType.Wishlist && customer.IsAnonymous)
            {
                var invalid =
                    new InvalidOperationException(
                        "Wishlists are not supported with anonymous customers.  The customer key passed returned an anonymous customer.");
                MultiLogHelper.Error<CustomerApiController>("Could not retrieve customer wish list", invalid);
                throw invalid;
            }

            return itemCacheType == ItemCacheType.Basket ? 
                ((Basket)customer.Basket()).ItemCache.ToCustomerItemCacheDisplay() : 
                ((WishList)((Customer)customer).WishList()).ItemCache.ToCustomerItemCacheDisplay();
        }  

        /// <summary>
        /// Returns a filtered list of customers
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/SearchCustomers
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of customers..
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchCustomers(QueryDisplay query)
        {
            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");

            return term != null && !string.IsNullOrEmpty(term.Value)
              ?
               _merchello.Query.Customer.Search(
                  term.Value,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection)
              :
              _merchello.Query.Customer.Search(
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection);
        }       

        /// <summary>
        /// The search by date range.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchByDateRange(QueryDisplay query)
        {
            var lastActivityDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "lastActivityDateStart");
            var lastActivityDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "lastActivityDateEnd");
           
            DateTime startDate;
            DateTime endDate;
            Mandate.ParameterNotNull(lastActivityDateStart, "lastActivityDateStart is a required parameter");
            Mandate.ParameterCondition(DateTime.TryParse(lastActivityDateStart.Value, out startDate), "Failed to convert lastActivityDateStart to a valid DateTime");

            endDate = lastActivityDateEnd == null
                ? DateTime.MaxValue
                : DateTime.TryParse(lastActivityDateEnd.Value, out endDate)
                    ? endDate
                    : DateTime.MaxValue;

            return
                _merchello.Query.Customer.Search(
                    startDate,
                    endDate,
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection);
        }
                


        /// <summary>
        /// POST /umbraco/Merchello/CustomerApi/AddCustomer/
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [HttpPost]
        public CustomerDisplay AddCustomer(CustomerDisplay customer)
        {
            var newCustomer = _customerService.CreateCustomer(
                string.IsNullOrEmpty(customer.LoginName) ? customer.Email : customer.LoginName,
                customer.FirstName,
                customer.LastName,
                customer.Email);
            newCustomer.LastActivityDate = DateTime.Today;

            ////((Customer)newCustomer).Addresses = customer.Addresses.Select(x => x.ToCustomerAddress(new CustomerAddress(customer.Key)));

            _customerService.Save(newCustomer);

            return newCustomer.ToCustomerDisplay();
        }

        /// <summary>
        /// POST /umbraco/Merchello/CustomerApi/AddCustomer/
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [HttpPost]
        public IAnonymousCustomer AddAnonymousCustomer(CustomerDisplay customer)
        {            
            var newCustomer = _customerService.CreateAnonymousCustomerWithKey();
            
            newCustomer.LastActivityDate = DateTime.Today;
            
            _customerService.Save(newCustomer);

            return newCustomer;
        }

        /// <summary>
        /// PUT /umbraco/Merchello/CustomerApi/PutCustomer/
        /// Saves the customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [HttpPost, HttpPut]
        public CustomerDisplay PutCustomer(CustomerDisplay customer)
        {
            RemoveDeletedAddresses(customer);
            SaveNewAddresses(customer);

            var merchCustomer = _customerService.GetByKey(customer.Key);

            merchCustomer = customer.ToCustomer(merchCustomer);

           _customerService.Save(merchCustomer);

            return merchCustomer.ToCustomerDisplay();
        }


        /// <summary>
        /// Deletes an existing customer
        /// 
        /// DELETE /umbraco/Merchello/CustomerApi/{guid}
        /// </summary>
        /// <param name="id">
        /// The id of the customer to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpGet]
        public HttpResponseMessage DeleteCustomer(Guid id)
        {
            var customer = _customerService.GetByKey(id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _customerService.Delete(customer);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Removes addresses deleted in the back office.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void RemoveDeletedAddresses(CustomerDisplay customer)
        {
            var existing = _customerAddressService.GetByCustomerKey(customer.Key);

            var existingAddresses = existing as ICustomerAddress[] ?? existing.ToArray();
            if (!existingAddresses.Any()) return;

            foreach (var delete in existingAddresses.Where(address => customer.Addresses.All(x => x.Key != address.Key)).ToList())
            {
                _customerAddressService.Delete(delete);
            }
        }

        /// <summary>
        /// Saves any new customer addresses
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void SaveNewAddresses(CustomerDisplay customer)
        {
            foreach (var address in customer.Addresses.Where(x => x.Key == Guid.Empty))
            {
                var customerAddress = address.ToCustomerAddress(new CustomerAddress(customer.Key));
                customerAddress.AddressType = address.AddressType;
                _customerAddressService.Save(customerAddress);
                address.Key = customerAddress.Key;
            }
        }
    }
}