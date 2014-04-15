﻿using System;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// An attribute used to decorate gateway providers to be resolved and "activated/deactivated"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GatewayProviderActivationAttribute : Attribute
    {
        /// <summary>
        /// The unique 'Key' for the GatewayProvider.  This is important to assert that the same provider cannot be registered more than once. 
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// The name of the gateway provider.  This typically shows up in the back office UI
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The description of the gateway provider.  
        /// </summary>
        public string Description { get; private set; }

        public GatewayProviderActivationAttribute(string key, string name, string description)
        {            
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(description, "description");

            Key = new Guid(key);
            Name = name;
            Description = description;
        }

        public GatewayProviderActivationAttribute(string key, string name)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            Key = new Guid(key);
            Name = name;
            Description = string.Empty;
        }
         
    }
}