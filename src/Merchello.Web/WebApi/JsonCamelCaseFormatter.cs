namespace Merchello.Web.WebApi
{
    using System;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Applying this attribute to any WebAPI controller will ensure that it only contains one JSON formatter compatible with the angular JSON vulnerability prevention.
    /// </summary>
    public class JsonCamelCaseFormatter : Attribute, IControllerConfiguration
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="controllerSettings">
        /// The controller settings.
        /// </param>
        /// <param name="controllerDescriptor">
        /// The controller descriptor.
        /// </param>
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            ////http://issues.merchello.com/youtrack/issue/M-247
         
            var formatter = controllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().Single();
            controllerSettings.Formatters.Remove(formatter);

            formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = { ContractResolver = new CamelCasePropertyNamesContractResolver() }
            };

            controllerSettings.Formatters.Add(formatter);            
        }
    }
}
