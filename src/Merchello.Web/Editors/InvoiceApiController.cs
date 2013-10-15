using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
	[PluginController("Merchello")]
	public class InvoiceApiController : MerchelloApiController
	{
		private IInvoiceService _invoiceService;

		/// <summary>
		/// Constructor
		/// </summary>
		public InvoiceApiController()
			: this(MerchelloContext.Current)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="merchelloContext"></param>
		public InvoiceApiController(MerchelloContext merchelloContext)
			: base(merchelloContext)
		{

			_invoiceService = MerchelloContext.Services.InvoiceService;
		}

		/// <summary>
		/// This is a helper contructor for unit testing
		/// </summary>
		internal InvoiceApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
			: base(merchelloContext, umbracoContext)
		{

			_invoiceService = MerchelloContext.Services.InvoiceService;
		}

		/// <summary>
		/// Returns customer by the key
		/// </summary>
		/// <param name="key"></param>
		public Invoice GetInvoiceById(int id)
		{
			if (id != null)
			{
				var invoice = MerchelloContext.Services.InvoiceService.GetById(id) as Invoice;
				if (invoice == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

				return invoice;
			}
			else
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(String.Format("Parameter id is null")),
					ReasonPhrase = "Invalid Parameter"
				};
				throw new HttpResponseException(resp);
			}
		}

		public IEnumerable<Invoice> GetInvoicesByIds(List<int> ids)
		{
			if (ids != null)
			{
				var invoices = MerchelloContext.Services.InvoiceService.GetByIds(ids) as IEnumerable<Invoice>;
				if (invoices == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

				return invoices;
			}
			else
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(String.Format("Parameter id is null")),
					ReasonPhrase = "Invalid Parameter"
				};
				throw new HttpResponseException(resp);
			}

		}


		public IEnumerable<Invoice> GetInvoicesByCustomer(Guid key)
		{
			if (key != null)
			{
				var invoices = MerchelloContext.Services.InvoiceService.GetInvoicesByCustomer(key) as IEnumerable<Invoice>;
				if (invoices == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

				return invoices;
			}
			else
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(String.Format("Parameter id is null")),
					ReasonPhrase = "Invalid Parameter"
				};
				throw new HttpResponseException(resp);
			}

		}


		public IEnumerable<Invoice> GetInvoicesByInvoiceStatus(int invoiceStatusId)
		{
			if (invoiceStatusId != null)
			{
				var invoices = MerchelloContext.Services.InvoiceService.GetInvoicesByInvoiceStatus(invoiceStatusId) as IEnumerable<Invoice>;
				if (invoices == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

				return invoices;
			}
			else
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(String.Format("Parameter id is null")),
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
		public Invoice NewInvoice(Customer customer, CustomerAddress customerAddress, InvoiceStatus invoiceStatus, string invoiceNumber)
		{
			Invoice newInvoice = null;

			try
			{
				newInvoice = _invoiceService.CreateInvoice(customer, customerAddress, invoiceStatus, invoiceNumber) as Invoice;
			}
			catch
			{
				throw new HttpResponseException(HttpStatusCode.InternalServerError);
			}
			return newInvoice;
		}
		/// <summary>
		/// Updates an existing Customer
		/// 
		/// PUT /umbraco/Merchello/CustomerApi/PutCustomer
		/// </summary>
		/// <param name="customer">Customer To Update</param>
		/// <returns>Http Response</returns>
		public HttpResponseMessage PutInvoice(Invoice invoice)
		{
			var response = Request.CreateResponse(HttpStatusCode.OK);

			try
			{
				_invoiceService.Save(invoice);
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
		/// <param name="key">Guid of Customer</param>
		/// <returns>Http Response</returns>
		public HttpResponseMessage Delete(int id)
		{
			var invoiceToDelete = _invoiceService.GetById(id);
			var response = Request.CreateResponse(HttpStatusCode.OK);

			if (invoiceToDelete == null)
			{
				response = Request.CreateResponse(HttpStatusCode.NotFound);
				return response;
			}

			_invoiceService.Delete(invoiceToDelete);

			return response;
		}
	}
}
