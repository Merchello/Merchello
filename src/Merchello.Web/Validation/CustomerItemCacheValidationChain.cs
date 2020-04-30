namespace Merchello.Web.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains;
    using Merchello.Core.Models;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;

    /// <summary>
    /// A task chain to validate <see cref="IItemCache"/>.
    /// </summary>
    internal sealed class CustomerItemCacheValidationChain : ConfigurationChainBase<ValidationResult<CustomerItemCacheBase>>, IValidationChain<CustomerItemCacheBase>
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The enable data modifiers.
        /// </summary>
        private bool _enableDataModifiers = true;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerItemCacheValidationChain"/> class.
        /// </summary>
        public CustomerItemCacheValidationChain()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerItemCacheValidationChain"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public CustomerItemCacheValidationChain(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            ResolveChain(Core.Constants.TaskChainAlias.ItemCacheValidation);
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable data modifiers.
        /// </summary>
        public bool EnableDataModifiers
        {
            get
            {
                return _enableDataModifiers;
            }

            set
            {
                _enableDataModifiers = value;
            }
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
                    (_constructorParameters = new List<object>(new object[] { _merchelloContext, _enableDataModifiers }));
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<ValidationResult<CustomerItemCacheBase>> Validate(CustomerItemCacheBase value)
        {
            var validated = new ValidationResult<CustomerItemCacheBase>() { Validated = value };

            if (!value.Items.Any()) return Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(validated);

            return TaskHandlers.Any()
            ? TaskHandlers.First().Execute(validated)
            : Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(validated);
        }
    }
}