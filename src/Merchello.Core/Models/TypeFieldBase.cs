using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    public abstract class TypeFieldBase
    {

        protected static TypeFieldDefinitionsElement Fields { get { return ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields; } }

        protected static ITypeField GetTypeField(TypeFieldElement element)
        {
            return element == null
                       ? NullTypeField()
                       : new TypeField(element);
        }

        /// <summary>
        /// Empty type - NullObject Pattern
        /// </summary>
        private static ITypeField NullTypeField()
        {
            return new TypeField("NotFound", "A TypeField with the configuration specified could not be found", Guid.Empty);
        }
    }
}
