namespace Merchello.Web.Validation.Tasks
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Chains;

    using Umbraco.Core;

    /// <summary>
    /// A base class for validation tasks.
    /// </summary>
    /// <typeparam name="T">
    /// The type to validate
    /// </typeparam>
    public abstract class CustomerItemCacheValidatationTaskBase<T> : AttemptChainTaskBase<T>
    {
        /// <summary>
        /// The <see cref="MerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The enable data modifiers.
        /// </summary>
        private readonly bool _enableDataModifiers;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        /// <remarks>
        /// We want to use cached values as much as possible when performing validation operations
        /// for performance.
        /// </remarks>
        private Lazy<MerchelloHelper> _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerItemCacheValidatationTaskBase{T}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers in the MerchelloHelper
        /// </param>
        protected CustomerItemCacheValidatationTaskBase(IMerchelloContext merchelloContext, bool enableDataModifiers)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            this._enableDataModifiers = enableDataModifiers;
            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>.
        /// </summary>
        protected IMerchelloContext MerchelloContext
        {
            get
            {
                return _merchelloContext;
            }
        }

        /// <summary>
        /// Gets the merchello.
        /// </summary>
        protected MerchelloHelper Merchello
        {
            get
            {
                return _merchello.Value;
            }
        }

        /// <summary>
        /// Initializes the task.
        /// </summary>
        private void Initialize()
        {
            _merchello = new Lazy<MerchelloHelper>(() => new MerchelloHelper(_merchelloContext, this._enableDataModifiers));
        }
    }
}