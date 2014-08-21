namespace Merchello.Web.Reporting
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.ObjectResolution;

    using Umbraco.Web;

    /// <summary>
    /// Represents an report controller resolver.
    /// </summary>
    internal class ReportApiControllerResolver : MerchelloManyObjectsResolverBase<ReportApiControllerResolver, ReportController>, IReportApiControllerResolver
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportApiControllerResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public ReportApiControllerResolver(IEnumerable<Type> value)
            : this(MerchelloContext.Current, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportApiControllerResolver"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal ReportApiControllerResolver(IMerchelloContext merchelloContext, IEnumerable<Type> value)
            : base(value)
        {  
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Gets the resolved types.
        /// Used for testing
        /// </summary>
        internal IEnumerable<Type> ResolvedTypes
        {
            get
            {
                return InstanceTypes;
            }
        }

        /// <summary>
        /// Gets the resolved values.
        /// </summary>
        protected override IEnumerable<ReportController> Values
        {
            get
            {
                var ctrArgs = new object[] { _merchelloContext };
                
                using (GetWriteLock())
                {
                    return CreateInstances(ctrArgs);
                }
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="ReportController"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="ReportController"/>s
        /// </returns>
        public IEnumerable<ReportController> GetAll()
        {
            return Values;
        }
    }
}