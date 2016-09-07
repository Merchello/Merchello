namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Models.TypeFields;

    /// <inheritdoc/>
    internal class TypeFieldsElement : RawXmlConfigurationElement, ITypeFieldsSection
    {
        /// <inheritdoc/>
        public IDictionary<CustomTypeFieldType, IEnumerable<ITypeField>> CustomTypeFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}