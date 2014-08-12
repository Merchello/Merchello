namespace Merchello.Web.Reporting
{
    using System;
    using Core;

    /// <summary>
    /// The report view attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReportAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="reportView">
        /// The report Angular view.
        /// </param>
        public ReportAttribute(string title, string reportView)
        {            
            Mandate.ParameterNotNullOrEmpty(title, "title");
            Mandate.ParameterNotNullOrEmpty(reportView, "reportView");

            Title = title;
            ReportView = reportView;
        }

        /// <summary>
        /// Gets the name of the report
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the relative path to the report view html
        /// </summary>
        public string ReportView { get; private set; }
    }
}