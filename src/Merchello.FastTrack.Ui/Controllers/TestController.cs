using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Merchello.Web;
using Merchello.Web.Models.VirtualContent;
using Newtonsoft.Json;
using Umbraco.Web.WebApi;

namespace Merchello.FastTrack.Ui.Controllers
{
    public class OptionsController : UmbracoApiController
    { 

        [System.Web.Mvc.HttpGet]		
        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<Dictionary<string, object>> GetOptions()
        {
            var returner = new MerchelloHelper().TypeProductContentBySku("shrt-despite").Options.FirstOrDefault().Choices.Select(z=>z.Properties.ToDictionary(x => x.PropertyTypeAlias, x => x.Value));

            return returner;
        }
    }
}