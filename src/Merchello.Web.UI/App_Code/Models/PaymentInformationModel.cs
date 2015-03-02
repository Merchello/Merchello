using System;

namespace Models
{
    /// <summary>
    /// Model to hold payment selection information acquired from the payment selection view
    /// </summary>
    public class PaymentInformationModel : AddressModel
    {

        public Guid PaymentMethodKey { get; set; }


    }
}