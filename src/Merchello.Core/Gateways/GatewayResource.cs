using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Gateways
{
    using Umbraco.Core;

    /// <summary>
    /// Defines a GatewayMethod 
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class GatewayResource : IGatewayResource
    {
        public GatewayResource(string serviceCode, string name)
        {
            Ensure.ParameterNotNullOrEmpty(serviceCode, "serviceCode");
            Ensure.ParameterNotNullOrEmpty(name, "name");

            ServiceCode = serviceCode;
            Name = name;
        }

        /// <summary>
        /// The unique provider service code or 'alias' for the gateway method.
        /// </summary>
        [DataMember]
        public string ServiceCode { get; private set; }

        /// <summary>
        /// The descriptive name of the Gateway Method
        /// </summary>
        [DataMember]
        public string Name { get; internal set; }
    }
}