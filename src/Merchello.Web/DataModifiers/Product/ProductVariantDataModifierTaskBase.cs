namespace Merchello.Web.DataModifiers.Product
{
    using Merchello.Core;
    using Merchello.Core.Chains;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The modifiable product variant data modifier task base.
    /// </summary>
    public abstract class ProductVariantDataModifierTaskBase : AttemptChainTaskBase<IProductVariantDataModifierData>
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDataModifierTaskBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected ProductVariantDataModifierTaskBase(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            this._merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Gets the merchello context.
        /// </summary>
        protected IMerchelloContext MerchelloContext
        {
            get
            {
                return this._merchelloContext;
            }
        }
    }
}