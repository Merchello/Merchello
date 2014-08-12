namespace Merchello.Web.Reporting
{
    using System;
    using Core;

    /// <summary>
    /// The report view attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReportViewAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewAttribute"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="reportView">
        /// The report Angular view.
        /// </param>
        public ReportViewAttribute(string title, string description, string reportView)
        {            
            Mandate.ParameterNotNullOrEmpty(title, "title");
            Mandate.ParameterNotNullOrEmpty(description, "description");
            Mandate.ParameterNotNullOrEmpty(reportView, "editorView");

            Title = title;
            Description = description;
            ReportView = reportView;
        }

        /// <summary>
        /// Gets the name of the gateway provider editor title  
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of the gateway provider editor 
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the relative path to the report view html
        /// </summary>
        public string ReportView { get; private set; }
    }
}