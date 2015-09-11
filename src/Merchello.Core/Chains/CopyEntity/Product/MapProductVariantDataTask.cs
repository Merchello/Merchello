namespace Merchello.Core.Chains.CopyEntity.Product
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Maps the variant specific information.
    /// </summary>
    internal sealed class MapProductVariantDataTask : CopyEntityTaskBase<IProduct>
    {
        public MapProductVariantDataTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        public override Attempt<IProduct> PerformTask(IProduct entity)
        {
            throw new System.NotImplementedException();
        }
    }
}