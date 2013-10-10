using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AnonymousCustomer :  CustomerBase, IAnonymousCustomer
    {
        public AnonymousCustomer() 
            : base(true)
        {}
    }
}
