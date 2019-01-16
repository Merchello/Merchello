namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The copy product collections task.
    /// </summary>
    internal sealed class CopyProductCollectionsTask : CopyEntityTaskBase<IProduct>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProductCollectionsTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        public CopyProductCollectionsTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        /// <summary>
        /// The perform task.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct value)
        {
            var collections = Original.GetCollectionsContaining().ToArray();

            foreach (var entityCollection in collections)
            {
                value.AddToCollection(entityCollection);
            }

            return Attempt<IProduct>.Succeed(value);
        }
    }
}