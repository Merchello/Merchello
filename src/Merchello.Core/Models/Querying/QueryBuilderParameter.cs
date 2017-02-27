namespace Merchello.Core.Models.Querying
{
    /// <summary>
    /// Represents a query builder parameter.
    /// </summary>
    internal class QueryBuilderParameter
    {
        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the match type.
        /// </summary>
        public FieldMatchCondition MatchCondition { get; set; }
    }
}