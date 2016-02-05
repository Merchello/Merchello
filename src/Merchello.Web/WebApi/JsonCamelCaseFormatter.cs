namespace Merchello.Web.WebApi
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http.Filters;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Applying this attribute to any WebAPI controller will ensure that it only contains one JSON formatter compatible with the angular JSON vulnerability prevention.
    /// </summary>
    /// <remarks>
    /// http://issues.merchello.com/youtrack/issue/M-247
    /// </remarks>
    /// <seealso cref="http://stackoverflow.com/questions/19956838/force-camalcase-on-asp-net-webapi-per-controller"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1627:DocumentationTextMustNotBeEmpty", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonCamelCaseFormatter : ActionFilterAttribute 
    {
        /// <summary>
        /// The _camel casing formatter.
        /// </summary>
        private static readonly JsonMediaTypeFormatter CamelCasingFormatter = new JsonMediaTypeFormatter();

        /// <summary>
        /// Initializes static members of the <see cref="JsonCamelCaseFormatter"/> class.
        /// </summary>
        static JsonCamelCaseFormatter()
        {
            CamelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        /// <summary>
        /// The on action executed.
        /// </summary>
        /// <param name="httpActionExecutedContext">
        /// The http action executed context.
        /// </param>
        public override void OnActionExecuted(HttpActionExecutedContext httpActionExecutedContext)
        {
            var objectContent = (ObjectContent)httpActionExecutedContext.Response.Content;
            if (objectContent != null)
            {
                if (objectContent.Formatter is JsonMediaTypeFormatter)
                {
                    httpActionExecutedContext.Response.Content = new ObjectContent(objectContent.ObjectType, objectContent.Value, CamelCasingFormatter);
                }
            }
        }
    }
}
