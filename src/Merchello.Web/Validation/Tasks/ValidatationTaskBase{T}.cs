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
    public abstract class ValidatationTaskBase<T> : AttemptChainTaskBase<T>
    {
        /// <summary>
        /// The <see cref="MerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The enable data modifiers.
        /// </summary>
        private readonly bool _enableDataModfiers;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        /// <remarks>
        /// We want to use cached values as much as possible when performing validation operations
        /// for performace.
        /// </remarks>
        private Lazy<MerchelloHelper> _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatationTaskBase{T}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModfiers">
        /// A value indicating whether or not to enable data modifiers in the MerchelloHelper
        /// </param>
        protected ValidatationTaskBase(IMerchelloContext merchelloContext, bool enableDataModfiers)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            _enableDataModfiers = enableDataModfiers;
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
            _merchello = new Lazy<MerchelloHelper>(() => new MerchelloHelper(_merchelloContext.Services, _enableDataModfiers));
        }
    }
}