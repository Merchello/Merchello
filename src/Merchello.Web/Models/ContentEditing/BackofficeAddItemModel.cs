using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// The Backoffice add item model.
    /// </summary>
    public class BackofficeAddItemModel
    {
        ///// <summary>
        ///// Gets or sets the content id.
        ///// </summary>
        //[Required]
        //public int ContentId { get; set; }

        ///// <summary>
        ///// Gets or sets the product key.
        ///// </summary>
        //public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the Merchello product.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public string CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the Merchello Payment Method.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public string PaymentKey { get; set; }
        
        /// <summary>
        /// Gets or sets the Merchello Payment Provider.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public string PaymentProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the Merchello Ship Method.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public string ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets the Merchello product.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public string[] ProductKeys { get; set; }

        /// <summary>
        /// Gets or sets the Merchello product.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public AddressDisplay ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Merchello product.
        /// </summary>
        /// <remarks>
        /// Used to populate the add item form
        /// </remarks>
        public AddressDisplay BillingAddress { get; set; }

        ///// <summary>
        ///// Gets or sets the option choices.
        ///// </summary>
        ///// <remarks>
        ///// Used to determine the product variant (if applicable) to add to the basket
        ///// </remarks>
        //public Guid[] OptionChoices { get; set; }

        ///// <summary>
        ///// Gets or sets the quantity to add to the basket
        ///// </summary>
        //[Required]
        //public int Quantity { get; set; }
    }
}
