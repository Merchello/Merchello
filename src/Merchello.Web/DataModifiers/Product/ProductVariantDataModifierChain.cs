namespace Merchello.Web.DataModifiers
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a ProductVariantDataModifierData data modifier chain
    /// </summary>
    internal sealed class ProductVariantDataModifierChain : ConfigurationChainBase<IProductVariantDataModifierData>, IDataModifierChain<IProductVariantDataModifierData>
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDataModifierChain"/> class.
        /// </summary>
        public ProductVariantDataModifierChain()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDataModifierChain"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="MerchelloContext"/>.
        /// </param>
        public ProductVariantDataModifierChain(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            ResolveChain(Core.Constants.TaskChainAlias.MerchelloHelperProductDataModifiers);
        }

        /// <summary>
        /// Gets the count of tasks - Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }

        /// <summary>
        /// Gets the constructor argument values.
        /// </summary>
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ??
                    (_constructorParameters = new List<object>(new object[] { _merchelloContext }));
            }
        }

        /// <summary>
        /// Attempts to modify the data.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IProductVariantDataModifierData> Modify(IProductVariantDataModifierData value)
        {
            return TaskHandlers.Any()
                       ? TaskHandlers.First().Execute(value)
                       : Attempt<IProductVariantDataModifierData>.Succeed(value);
        }
    }
}