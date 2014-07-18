namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The customer element.
    /// </summary>
    public class CustomerElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the member types.
        /// </summary>
        [ConfigurationProperty("memberTypes", IsRequired = true)]
        public string MemberTypes
        {
            get { return (string)this["memberTypes"]; }
            set { this["memberTypes"] = value; }
        } 
    }
}