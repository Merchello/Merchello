namespace Merchello.Core.Gateways
{
    using System;

    /// <summary>
    /// The gateway method ui attribute.
    /// </summary>
    /// <remarks>
    /// Intended to be used to provide context to front end designers during method selection.  
    /// 
    /// Initial use case is for payment methods and credit card forms and notification methods for data entry.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GatewayMethodUiAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayMethodUiAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        public GatewayMethodUiAttribute(string alias)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; private set; }
    }
}