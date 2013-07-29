using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.EntityBase;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface IInvoiceItemBase : IItemization, IAggregateRoot
    {
        [DataMember]
        int ParentId { get; set; }

        [IgnoreDataMember]
        IInvoiceBase Invoice { get; set; }

        [IgnoreDataMember]
        IShipmentBase Shipment { get; set; }

    }


}
