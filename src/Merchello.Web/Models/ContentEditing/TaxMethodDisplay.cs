namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;

    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;

    /// <summary>
    /// The tax method display.
    /// </summary>
    public class TaxMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the percentage tax rate.
        /// </summary>
        public decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this method is used for product based taxation like VAT.
        /// </summary>
        public bool ProductTaxMethod { get; set; }

        /// <summary>
        /// Gets or sets the provinces.
        /// </summary>
        public IEnumerable<TaxProvinceDisplay> Provinces { get; set; }
    }

    /// <summary>
    /// The tax method display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]        
    public static class TaxMethodDisplayExtensions
    {
        #region TaxMethodDisplay

        /// <summary>
        /// Maps a <see cref="TaxMethodDisplay"/> to <see cref="ITaxMethod"/>.
        /// </summary>
        /// <param name="taxMethodDisplay">
        /// The tax method display.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        internal static ITaxMethod ToTaxMethod(this TaxMethodDisplay taxMethodDisplay, ITaxMethod destination)
        {
            if (taxMethodDisplay.Key != Guid.Empty) destination.Key = taxMethodDisplay.Key;


            destination.Name = taxMethodDisplay.Name;
            destination.PercentageTaxRate = taxMethodDisplay.PercentageTaxRate;
            destination.ProductTaxMethod = taxMethodDisplay.ProductTaxMethod;

            // this may occur when creating a new tax method since the UI does not 
            // query for provinces 
            // TODO fix
            if (destination.HasProvinces && !taxMethodDisplay.Provinces.Any())
            {
                taxMethodDisplay.Provinces = destination.Provinces.Select(x => x.ToTaxProvinceDisplay()).ToArray();
            }

            foreach (var province in taxMethodDisplay.Provinces)
            {
                var p = destination.Provinces.FirstOrDefault(x => x.Code == province.Code);
                if (p != null) p.PercentAdjustment = province.PercentAdjustment;
            }

            return destination;
        }

        /// <summary>
        /// The to tax method display.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <returns>
        /// The <see cref="TaxMethodDisplay"/>.
        /// </returns>
        internal static TaxMethodDisplay ToTaxMethodDisplay(this ITaxMethod taxMethod)
        {
            return AutoMapper.Mapper.Map<TaxMethodDisplay>(taxMethod);
        }

        /// <summary>
        /// The to tax method display.
        /// </summary>
        /// <param name="taxGatewayMethod">
        /// The tax gateway method.
        /// </param>
        /// <returns>
        /// The <see cref="TaxMethodDisplay"/>.
        /// </returns>
        internal static TaxMethodDisplay ToTaxMethodDisplay(this ITaxationGatewayMethod taxGatewayMethod)
        {
            return AutoMapper.Mapper.Map<TaxMethodDisplay>(taxGatewayMethod);
        }

        #endregion
    }
}