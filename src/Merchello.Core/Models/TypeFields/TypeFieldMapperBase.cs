using System;

namespace Merchello.Core.Models.TypeFields
{
    public abstract class TypeFieldMapperBase
    {
        /// <summary>
        /// An empty type field
        /// </summary>
        internal static ITypeField NotFound
        {
            get { return new TypeField("NotFound", "A TypeField with the configuration specified could not be found", Guid.Empty); }
        }
       
    }
}
