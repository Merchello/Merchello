using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Marker interface for anonymous customers
    /// </summary>
    public interface IAnonymousCustomer : ICustomerBase, IKeyEntity
    {
         
    }
}