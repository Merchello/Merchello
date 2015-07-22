namespace Merchello.Core.Gateways.Taxation
{
    using System;

    /// <summary>
    /// Marker interface for a TaxationProvider that can tax by product
    /// </summary>
    public interface ITaxationByProductProvider
    {
        ITaxationByProductMethod GetTaxationByProductMethod(Guid key);
    }
}