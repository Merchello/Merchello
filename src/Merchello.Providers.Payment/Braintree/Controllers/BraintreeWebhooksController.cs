namespace Merchello.Providers.Payment.Braintree.Controllers
{
    using System;
    using System.Web.Mvc;

    public partial class BraintreeWebhooksController : Controller
    {        

        public ActionResult Accept()
        {
            throw new NotImplementedException();
            //return Content(Constants.Gateway.WebhookNotification.Verify(Request.QueryString["bt_challenge"]));
        }
         
    }
}