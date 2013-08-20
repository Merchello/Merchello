using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    public abstract class TypeFieldProxyBase
    {

        protected static TypeFieldDefinitionsElement Fields { get { return ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields; } }

        protected static ITypeField GetTypeField(TypeFieldElement element)
        {
            return element == null
                       ? TypeFieldMapperBase.NotFound
                       : new TypeField(element);
        }


        public static ITypeField Custom(string alias)
        {
            return NullTypeField();
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
