using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Simple Model for the Add To Cart form.
    /// </summary>
    public class AddItemModel
    {

        /// <summary>
        /// Content Id of the ProductDetail page
        /// </summary>
        [Required]
        public int ContentId { get; set; }

        /// <summary>
        /// The Product Key (pk) of the product to be added to the cart.
        /// </summary>
        /// <remarks>
        /// 
        /// NOTE : This is not the ProductVariantKey. The variant will be figured out
        /// by the product key and the array of Guids (OptionChoices).  These define the ProductVariant 
        /// 
        /// </remarks>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// The option choices (if there are any), used to determine the variant 
        /// </summary>
        public Guid[] OptionChoices { get; set; }
    }
}