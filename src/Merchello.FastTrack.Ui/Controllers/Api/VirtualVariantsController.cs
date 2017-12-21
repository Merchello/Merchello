using System;
using Merchello.Web;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.VirtualContent;
using Umbraco.Web.WebApi;

namespace Merchello.FastTrack.Ui.Controllers.Api
{
    public class VirtualVariantsController : UmbracoApiController
    {
        [System.Web.Mvc.HttpGet]
        [System.Web.Http.AcceptVerbs("GET")]
        public ProductVariantDisplay GetVirtualVariant()
        {

            var helper = new MerchelloHelper();

            var product = helper.TypedProductContent("2dc655b5-7103-4adb-b193-f8a5fd1f88e8");

            var displayProduct = product.AsProductDisplay();

            var variant = displayProduct.GetProductVariantDisplayWithAttributes(new Guid[]
            {
                Guid.Parse("c71e3295-99c2-4596-afd0-38723cbe6a7b"), Guid.Parse("670b0fe6-a7c4-4470-b3b5-105aeb723c40")
            });

            return variant;

        }
    }
}