namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Examine;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Web.UI.Umbraco.Dashboard;

    /// <summary>
    /// The copy product task chain.
    /// </summary>
    /// <remarks>
    /// We could do this with a deep clone but some people want control over which bits are copied and 
    /// the ability to augment the process without having to handle an event each time.
    /// </remarks>
    internal sealed class CopyProductTaskChain : CopyEntityAttemptChainBase<IProduct>
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The original <see cref="IProduct"/>
        /// </summary>
        private readonly IProduct _original;

        /// <summary>
        /// The new product name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The new products SKU.
        /// </summary>
        private readonly string _sku;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProductTaskChain"/> class.
        /// </summary>
        /// <param name="original">
        /// The original <see cref="IProduct"/>
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public CopyProductTaskChain(IProduct original, string name, string sku)
            : this(MerchelloContext.Current, original, name, sku)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProductTaskChain"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original <see cref="IProduct"/>
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public CopyProductTaskChain(IMerchelloContext merchelloContext, IProduct original, string name, string sku)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(original, "original");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");

            this._merchelloContext = merchelloContext;
            
            _original = original;
            _name = name;
            _sku = sku;

            this.ResolveChain(Core.Constants.TaskChainAlias.CopyProduct);
        }

        /// <summary>
        /// Gets the count of tasks - Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return this.TaskHandlers.Count(); }
        }


        /// <summary>
        /// Gets the constructor argument values.
        /// </summary>
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return this._constructorParameters ??
                    (this._constructorParameters = new List<object>(new object[] { this._merchelloContext, _original }));
            }
        }

        /// <summary>
        /// Executes the tasks in the task chain
        /// </summary>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>        
        public override Attempt<IProduct> Copy()
        {
            if (!this.ValidateSku()) return Attempt<IProduct>.Fail(new InvalidSkuException("A product or product variant already exists with the sku: " + _sku + ". SKUs must be unique."));

            var clone = _merchelloContext.Services.ProductService.CreateProduct(_name, _sku, _original.Price, false);
            clone.Barcode = _original.Barcode;
            clone.Available = false;
            clone.CostOfGoods = _original.CostOfGoods;
            clone.Download = _original.Download;
            clone.DownloadMediaId = _original.DownloadMediaId;
            clone.Height = _original.Height;
            clone.Length = _original.Length;
            clone.Weight = _original.Weight;
            clone.Width = _original.Width;
            clone.OnSale = _original.OnSale;
            clone.SalePrice = _original.SalePrice;
            clone.Manufacturer = _original.Manufacturer;
            clone.ManufacturerModelNumber = _original.ManufacturerModelNumber;
            clone.TrackInventory = _original.TrackInventory;
            clone.OutOfStockPurchase = _original.OutOfStockPurchase;
            clone.Shippable = _original.Shippable;
            clone.Taxable = _original.Taxable;
            clone.VirtualVariants = _original.VirtualVariants;

            var attempt = this.TaskHandlers.Any()
                      ? this.TaskHandlers.First().Execute(clone)
                      : Attempt<IProduct>.Fail(clone, new NotSupportedException("No tasks were found to continue copying the product"));

            if (attempt.Success)
            {
                _merchelloContext.Services.ProductService.Save(attempt.Result);
            }

            return attempt;
        }

        /// <summary>
        /// Asserts the SKU does not already exist.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidateSku()
        {
            return !_merchelloContext.Services.ProductService.SkuExists(_sku);
        }
    }
}