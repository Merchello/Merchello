using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.WebApi;
using Umbraco.Web.WebApi.Filters;

namespace Merchello.Web.WebApi
{
    /// <summary>
    /// Applying this attribute to any webapi controller will ensure that it only contains one json formatter compatible with the angular json vulnerability prevention.
    /// </summary>
    public class JsonCamelCaseFormatter : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
