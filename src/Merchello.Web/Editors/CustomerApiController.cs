using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Umbraco.Web;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class CustomerApiController : MerchelloApiController
    {
		private ICustomerService _customerService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerApiController()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CustomerApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {

			_customerService = MerchelloContext.Services.CustomerService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal CustomerApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {

			_customerService = MerchelloContext.Services.CustomerService;
        }

        /// <summary>
        /// Returns customer by the key
        /// </summary>
        /// <param name="id"></param>
        public Customer GetCustomer(int id)
        {
			if (id != 0)
			{
				var customer = MerchelloContext.Services.CustomerService.GetById(id) as Customer;
				if (customer == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

				return customer;
			}
			else
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(String.Format("Parameter key is null")),
					ReasonPhrase = "Invalid Parameter"
				};
				throw new HttpResponseException(resp);
			}
        }

		/// <summary>
		///  Creates a customer from FirstName, LastName, Email and MemberId
		///  
		/// GET /umbraco/Merchello/CustomerApi/NewCustomer?firstName=FIRSTNAME&lastName=LASTNAME&email=EMAIL&memberId=0
		/// </summary>
		/// <param name="firstName">Customers First Name</param>
		/// <param name="lastName">Customers Last Name</param>
		/// <param name="email">Customers Email Address</param>
		/// <param name="memberId">Optional: MemberId</param>
		/// <returns>New Customer</returns>		
		[AcceptVerbs("GET", "POST")]
		public Customer NewCustomer(string firstName, string lastName, string email, int? memberId = null)
		{
			Customer newCustomer = null;
			
			try
			{
				newCustomer = _customerService.CreateCustomer(firstName, lastName, email, memberId) as Customer;
			}
			catch
			{		   
				throw new HttpResponseException(HttpStatusCode.InternalServerError);
			}
			return newCustomer;
		}
		/// <summary>
		/// Updates an existing Customer
		/// 
		/// PUT /umbraco/Merchello/CustomerApi/PutCustomer
		/// </summary>
		/// <param name="customer"> To Update</param>
		/// <returns>Http Response</returns>
		public HttpResponseMessage PutCustomer(Customer customer)
		{
			var response = Request.CreateResponse(HttpStatusCode.OK);

			try
			{
				_customerService.Save(customer);
			}
			catch (Exception ex)
			{
				response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
			}
			return response;  	
		}

        /// <summary>
        /// Deletes an existing customer
        /// 
        /// DELETE /umbraco/Merchello/CustomerApi/{guid}
        /// </summary>
        /// <param name="id">The id of the customer</param>
        /// <returns>Http Response</returns>
        public HttpResponseMessage Delete(int id)
		{
			var customerToDelete = _customerService.GetById(id);
			var response = Request.CreateResponse(HttpStatusCode.OK);

			if (customerToDelete == null)
			{
				response = Request.CreateResponse(HttpStatusCode.NotFound);
				return response;
			}

			_customerService.Delete(customerToDelete);

			return response;
		}
    }
}
