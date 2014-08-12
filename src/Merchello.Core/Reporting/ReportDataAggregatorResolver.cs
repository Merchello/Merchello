namespace Merchello.Core.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.ObjectResolution;

    using Umbraco.Core;

    /// <summary>
    /// Represents a report data aggregator resolver.
    /// </summary>
    internal class ReportDataAggregatorResolver : MerchelloManyObjectsResolverBase<ReportDataAggregatorResolver, object>,  IReportDataAggregatorResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public ReportDataAggregatorResolver(IEnumerable<Type> value) 
            : base(value)
        {
        }

        /// <summary>
        /// Gets a collection of instantiated <see cref="object"/>s
        /// </summary>
        protected override IEnumerable<object> Values
        {
            get
            {
                using (GetWriteLock())
                {
                    return
                        InstanceTypes.Select(x => this.CreateInstance(x, new object[] { }))
                            .Where(x => x.Success)
                            .Select(x => x.Result);
                }
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="object"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="object"/>s
        /// </returns>
        public IEnumerable<object> GetAll()
        {
            return Values;
        }

        /// <summary>
        /// Gets a report data aggregator by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report aggregator alias
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetByAlias(string alias)
        {
            var type = InstanceTypes.FirstOrDefault(x => x.GetCustomAttribute<ReportDataAggregatorAttribute>(true).Alias == alias);
            if (type == null) return null;

            var attempt = CreateInstance(type, new object[] { });

            return !attempt.Success ? null : attempt.Result;
        }
    }
}