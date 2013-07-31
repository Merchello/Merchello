using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    /// <summary>
    /// The type of a invoice line item
    /// </summary>
    public class InvoiceItemType : TypeFieldBase
    {         
        /// <summary>
        /// Catalog product sales
        /// </summary>
        public static ITypeField Product
        {
            get { return GetTypeField(Items["Product"]); }
        }

        /// <summary>
        /// A standard charge or fee
        /// </summary>
        public static ITypeField Charge
        {
            get { return GetTypeField(Items["Charge"]); }
        }

        /// <summary>
        /// A shipping specific charge
        /// </summary>
        public static ITypeField Shipping
        {
            get { return GetTypeField(Items["Shipping"]); }
        }

        /// <summary>
        /// A tax related charge
        /// </summary>
        public static ITypeField Tax
        {
            get { return GetTypeField(Items["Tax"]);  }
        }

        /// <summary>
        /// A credit
        /// </summary>
        public static ITypeField Credit
        {
            get { return GetTypeField(Items["Credit"]);  }
        }


        public static TypeFieldCollection Items
        {
            get { return Fields.InvoiceItem; }
        }

    }
}
