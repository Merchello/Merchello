namespace Merchello.Core.Reporting
{
    using System;

    /// <summary>
    /// The report attribute base.
    /// </summary>
    public abstract class ReportAttributeBase : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAttributeBase"/> class.
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