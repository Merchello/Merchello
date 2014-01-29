using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AnonymousCustomer :  AnonymousCustomerBase, IAnonymousCustomer
    {
        public AnonymousCustomer() 
            : this(new ExtendedDataCollection())
        {}

        public AnonymousCustomer(ExtendedDataCollection extendedData)
            : base(true, extendedData)
        {}
    }
}
