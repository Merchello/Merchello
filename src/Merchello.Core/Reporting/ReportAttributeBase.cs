namespace Merchello.Core.Reporting
{
    using System;

    /// <summary>
    /// The report attribute base.
    /// </summary>
    public abstract class ReportAttributeBase : Attribute
    {
        protected ReportAttributeBase(string alias, string title, string description)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            Alias = alias;
            Title = title;
            Description = description;
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
    }
}