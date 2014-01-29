using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines a Checkout base class
    /// </summary>
    public interface ICheckoutBase
    {
        /// <summary>
        /// Starts the checkout process over
        /// </summary>
        void StartOver();

        //void SaveAddress(IAddress shipTo, );

    }
}