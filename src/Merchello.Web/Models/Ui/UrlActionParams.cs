namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A utility class intended to be used in Razor views to store 
    /// </summary>
    public class UrlActionParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlActionParams"/> class.
        /// </summary>
        public UrlActionParams()
        {
            RouteParams = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets a collection of route parameters where the first is the parameter name and the second is the value.
        /// </summary>
        public List<Tuple<string, string>> RouteParams { get; private set; } 
    }
}