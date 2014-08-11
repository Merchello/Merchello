namespace Merchello.Core.Reporting
{
    using System;

    /// <summary>
    /// Decorates <see cref="IReport"/>s with information required for back office resolution and rendering
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReportAttribute : ReportAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAttribute"/> class.
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
        public ReportAttribute(string alias, string title, string reportView)
            : this(alias, title, string.Empty, reportView)
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAttribute"/> class.
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
        public ReportAttribute(string alias, string title, string description, string reportView)
            : base(alias, title, description)
        {
            Mandate.ParameterNotNullOrEmpty(reportView, "reportView");

            ReportView = reportView;
        }

        /// <summary>
        /// Gets the relative path to the report view html
        /// </summary>
        public string ReportView { get; private set; }
    }
}