namespace Merchello.Implementation.Attributes
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// An attribute to "tag" a Merchello UI Component set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ComponentSetAliasAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentSetAliasAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The component set alias.
        /// </param>
        public ComponentSetAliasAttribute(string alias)
        {
            Mandate.ParameterNotNullOrEmpty("alias", alias);
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the component set alias.
        /// </summary>
        public string Alias { get; private set; }
    }
}