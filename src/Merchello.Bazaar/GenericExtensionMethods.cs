namespace Merchello.Bazaar
{
    using Merchello.Core.Models;

    public static class GenericExtensionMethods
    {

        #region Decimal

        public static string FormattedPrice(this decimal price, ICurrency currency)
        {
            return ModelExtensions.FormatPrice(price, currency);
        }

        #endregion
    }
}
