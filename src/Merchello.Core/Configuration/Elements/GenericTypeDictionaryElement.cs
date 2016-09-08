namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired.Configuration;

    /// <summary>
    /// Configuration property wrapper to get a dictionary of aliased types.
    /// </summary>
    internal class GenericTypeDictionaryElement : RawXmlConfigurationElement
    {
        /// <summary>
        /// Creates a dictionary of aliased types.
        /// </summary>
        /// <param name="listElementName">
        /// The list element name.
        /// </param>
        /// <returns>
        /// The dictionary of types.
        /// </returns>
        public IDictionary<string, Type> CreateDictionary(string listElementName)
        {
            var xtypes = RawXml.Elements(listElementName);

            return xtypes.ToDictionary(xt => xt.Attribute("alias").Value, xt => Type.GetType(xt.Attribute("type").Value));
        } 
    }
}