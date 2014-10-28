using System;
using Merchello.Core.Gateways;

namespace Merchello.Plugin.Payments.Braintree.Controllers
{
    using System.Web.Mvc;

    public class BraintreeWebhooksController : Controller
    {        

        public ActionResult Accept()
        {
            throw new NotImplementedException();
            //return Content(Constants.Gateway.WebhookNotification.Verify(Request.QueryString["bt_challenge"]));
        }
         
    }
}