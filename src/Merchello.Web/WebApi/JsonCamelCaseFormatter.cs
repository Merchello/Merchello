﻿namespace Merchello.Web.WebApi
{
    using System;
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
            controllerSettings.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
