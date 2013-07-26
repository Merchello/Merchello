namespace Merchello.Core
{

    /// <summary>
    /// Used in basket and invoice (product) items to identify if a specific shipping method is required.
    /// </summary>
    /// <remarks>
    /// TODO: JP - This should wind up being exposed as a datatype in Umbraco
    /// </remarks>
    public enum ShipMethodType
    {
        Unspecified = 0,
        FlatRate = 1,
        PercentTotal = 2,
        Carrier = 3
    }
}
