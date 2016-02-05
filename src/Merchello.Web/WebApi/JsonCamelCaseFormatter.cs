namespace Merchello.Web.WebApi
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Applying this attribute to any WebAPI controller will ensure that it only contains one JSON formatter compatible with the angular JSON vulnerability prevention.
    /// </summary>
    /// <remarks>
    /// http://issues.merchello.com/youtrack/issue/M-247
    /// </remarks>
    /// <seealso cref="http://stackoverflow.com/questions/19956838/force-camalcase-on-asp-net-webapi-per-controller"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1627:DocumentationTextMustNotBeEmpty", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonCamelCaseFormatter : Attribute, IControllerConfiguration
    {
        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="controllerSettings">
        /// The controller settings.
        /// </param>
        /// <param name="controllerDescriptor">
        /// The controller descriptor.
        /// </param>
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Clear();

            var formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = { ContractResolver = new CamelCasePropertyNamesContractResolver() }
            };

            controllerSettings.Formatters.Add(formatter);
        }
    }
}
