namespace Merchello.Core.Chains.CopyEntity.Product
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Saves the copied product.
    /// </summary>    
    public sealed class SaveProductCopyTask : CopyEntityTaskBase<IProduct>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveProductCopyTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        public SaveProductCopyTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        /// <summary>
        /// Saves the product.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct entity)
        {
            Services.ProductService.Save(entity);
            return Attempt<IProduct>.Succeed(entity);
        }
    }
}