using System;

namespace Merchello.Core.Reporting
{
    /// <summary>
    /// Decorates <see cref="IReportDataAggregator"/>s with information required for back office resolution
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReportDataAggregatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias of the report
        /// </param>
        /// <param name="title">
        /// The title or the report
        /// </param>
        /// <param name="reportView">
        /// The report view - path to the Angular view to view the report
        /// </param>
        public ReportDataAggregatorAttribute(string alias, string title, string reportView)
            : this(alias, title, string.Empty, reportView)
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorAttribute"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias of the report
        /// </param>
        /// <param name="title">
        /// The title of the report
        /// </param>
        /// <param name="description">
        /// An optional description for the report
        /// </param>
        /// <param name="reportView">
        /// The report view - path to the Angular view to view the report
        /// </param>
        public ReportDataAggregatorAttribute(string alias, string title, string description, string reportView)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
            Mandate.ParameterNotNullOrEmpty(reportView, "reportView");

            Alias = alias;
            Title = title;
            Description = description;
            ReportView = reportView;
        }

        /// <summary>
        /// Gets the alias of the report.
        /// </summary>
        /// <remarks>
        /// This should be a unique value
        /// </remarks>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the name of the gateway provider editor title  
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of the report
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the relative path to the report view html
        /// </summary>
        public string ReportView { get; private set; }
    }
}