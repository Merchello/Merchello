namespace Merchello.Bazaar.Attributes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Allows for dynamically assigning the RequireSsl property at runtime
    /// either with an explicit boolean constant, a configuration setting,
    /// or a Reflection based 'delegate' 
    /// </summary>
    /// <remarks>
    /// http://weblog.west-wind.com/posts/2014/Jun/18/A-dynamic-RequireSsl-Attribute-for-ASPNET-MVC
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class RequireSslAttribute : RequireHttpsAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequireSslAttribute"/> class. 
        /// Default constructor forces SSL required
        /// </summary>
        public RequireSslAttribute()
        {
            RequireSsl = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireSslAttribute"/> class. 
        /// Allows assignment of the SSL status via parameter
        /// </summary>
        /// <param name="requireSsl">
        /// True or false indicating whether or not to require SSL
        /// </param>
        public RequireSslAttribute(bool requireSsl)
        {
            RequireSsl = requireSsl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireSslAttribute"/> class. 
        /// Allows invoking a static method at runtime to check for a 
        /// value dynamically.
        /// 
        /// Note: The method called must be a static method
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="method">
        /// Static method on this type to invoke with no parameters
        /// </param>
        public RequireSslAttribute(Type type, string method)
        {
            var mi = type.GetMethod(method, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public);
            RequireSsl = (bool)mi.Invoke(type, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireSslAttribute"/> class. 
        /// Looks for an appSetting key you specify and if it exists
        /// and is set to true or 1 which forces SSL.
        /// </summary>
        /// <param name="appSettingsKey">
        /// App setting key
        /// </param>
        public RequireSslAttribute(string appSettingsKey)
        {
            var key = WebConfigurationManager.AppSettings[appSettingsKey] as string;
            RequireSsl = false;
            if (!string.IsNullOrEmpty(key))
            {
                key = key.ToLower();
                if (key == "true" || key == "1")
                    RequireSsl = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require SSL.
        /// </summary>
        public bool RequireSsl { get; set; }


        /// <summary>
        /// Overrides the on authorization.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null &&
                RequireSsl &&
                !filterContext.HttpContext.Request.IsSecureConnection)
            {
                HandleNonHttpsRequest(filterContext);
            }
        }
    }
}