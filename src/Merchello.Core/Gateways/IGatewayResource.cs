namespace Merchello.Core.Gateways
{
    public interface IGatewayResource
    {
        /// <summary>
        /// The unique provider service code or 'alias' for the gateway method.
        /// </summary>
        string ServiceCode { get; }

        /// <summary>
        /// The descriptive name of the Gateway Method
        /// </summary>
        string Name { get; }
        
    }
}