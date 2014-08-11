namespace Merchello.Core.Reporting
{
    using System;

    /// <summary>
    /// Decorates <see cref="IReport"/>s to associate <see cref="IReportDataAggregator"/>s
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ReportDataAggregatorAttribute : ReportAttributeBase 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public ReportDataAggregatorAttribute(string alias, string title)
            : this(alias, title, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public ReportDataAggregatorAttribute(string alias, string title, string description) 
            : base(alias, title, description)
        {
        }
    }
}