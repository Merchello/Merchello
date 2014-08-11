namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Search;
    using Merchello.Web.WebApi;

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
            : this(MerchelloContext.Current, global::Umbraco.Core.ApplicationContext.Current.Services.MemberService)
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
            : base((MerchelloContext)merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _customerAddressService = ((Core.Services.ServiceContext)merchelloContext.Services).CustomerAddressService;
            _memberService = memberService;
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
            : base((MerchelloContext)merchelloContext, umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _memberService = memberService;
        }

        #endregion

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
            var customer = _customerService.GetByKey(id);
            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return customer.ToCustomerDisplay();
        }

        /// <summary>
        /// GET /umbraco/Merchello/CustomerApi/GetAllCustomers/
        /// 
        /// 
        /// Gets a collection of all customers.
        /// </summary>
        /// <returns>
        /// The collection of all customers
        /// </returns>
        [HttpGet]
        public QueryResultDisplay GetAllCustomers()
        {
            var customers = CustomerQuery.GetAllCustomers().ToArray();

            return new QueryResultDisplay()
            {
                Results = customers,
                PageIndex = 0,
                TotalPages = 1,
                TotalResults = customers.Count()
            };
        }

        /// <summary>
        /// GET /umbraco/Merchello/CustomerApi/GetAllCustomers/{page}/{perPage}
        /// 
        /// 
        /// Gets a paged collection of customers.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="perPage">
        /// The per page.
        /// </param>
        /// <returns>
        /// The paged collection of customers.
        /// </returns>
        [HttpGet]
        public QueryResultDisplay GetAllCustomers(int page, int perPage)
        {
            var allCustomers = CustomerQuery.GetAllCustomers().ToArray();
            var customers = allCustomers.Skip((page - 1) * perPage).Take(perPage);

            return new QueryResultDisplay()
            {
                Results = customers,
                PageIndex = page,
                TotalPages = ((allCustomers.Count() - 1) / perPage) + 1,
                TotalResults = allCustomers.Count()
            };
        }

        /// <summary>
        /// Returns a filtered list of customers
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetFilteredCustomers
        /// </summary>
        /// <param name="term">
        /// The search term
        /// </param>
        /// <returns>
        /// The collection of customers..
        /// </returns>
        public QueryResultDisplay GetFilteredCustomers(string term)
        {
            var customers = CustomerQuery.Search(term).ToArray();

            return new QueryResultDisplay()
            {
                Results = customers,
                PageIndex = 0,
                TotalPages = 1,
                TotalResults = customers.Count()
            };
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/InvoicesApi/GetFilteredCustomers
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="perPage">
        /// The per Page.
        /// </param>
        /// <returns>
        /// The collection of invoices..
        /// </returns>
        public QueryResultDisplay GetFilteredCustomers(string term, int page, int perPage)
        {
            var allMatches = CustomerQuery.Search(term).ToArray();
            var customers = allMatches.Skip(page * perPage).Take(perPage);

            return new QueryResultDisplay()
            {
                Results = customers,
                PageIndex = page,
                TotalPages = ((allMatches.Count() - 1) / perPage) + 1,
                TotalResults = allMatches.Count()
            };
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
                customer.LoginName,
                customer.FirstName,
                customer.LastName,
                customer.Email);

            newCustomer.Notes = customer.Notes;
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
                _customerAddressService.Save(address.ToCustomerAddress(new CustomerAddress(customer.Key)));
            }
        }
    }
}