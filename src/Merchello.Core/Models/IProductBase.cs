using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface IProductBase : IKeyEntity
    {
        /// <summary>
        /// The sku for the Product
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The name for the Product
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The price for the Product
        /// </summary>
        [DataMember]
        decimal Price { get; set; }
       
    }
}