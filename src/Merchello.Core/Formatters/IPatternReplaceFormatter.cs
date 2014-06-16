namespace Merchello.Core.Formatters
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a PatternReplaceFormatter
    /// </summary>
    public interface IPatternReplaceFormatter : IFormatter
    {
        /// <summary>
        /// Adds a replaceable pattern to the formatter
        /// </summary>
        /// <param name="pattern">
        /// The <see cref="IReplaceablePattern"/> to be added or updated within the formatter
        /// </param>
        void AddOrUpdateReplaceablePattern(IReplaceablePattern pattern);

        /// <summary>
        /// Adds collection a replaceable pattern to the formatter
        /// </summary>
        /// <param name="patterns">
        /// The <see cref="IReplaceablePattern"/>s to be added or updated within the formatter
        /// </param>
        void AddOrUpdateReplaceablePattern(IEnumerable<IReplaceablePattern> patterns);

        /// <summary>
        /// Sets (or resets) and existing patterns replacement value
        /// </summary>
        /// <param name="alias">The alias of the <see cref="IReplaceablePattern"/> which replacement value is to be updated</param>
        /// <param name="replacement">The new "replacement" value</param>
        void SetReplacement(string alias, string replacement);

        /// <summary>
        /// Removes a replaceable pattern from the formatter
        /// </summary>
        /// <param name="pattern">
        /// The <see cref="IReplaceablePattern"/> to be added or updated within the formatter
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/> removed
        /// </returns>
        IReplaceablePattern RemoveReplaceablePattern(IReplaceablePattern pattern);

        /// <summary>
        /// Gets a replaceable pattern from the formatter by it's unique alias
        /// </summary>
        /// <param name="alias">
        /// The unique alias of the pattern to be returned
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/>.
        /// </returns>
        IReplaceablePattern GetReplaceablePatternByAlias(string alias);

        /// <summary>
        /// Gets a replaceable pattern from the formatter by the pattern to be replaced
        /// </summary>
        /// <param name="pattern">
        /// The pattern defined in the <see cref="IReplaceablePattern"/> to be returned
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/>.
        /// </returns>
        IReplaceablePattern GetReplaceablePatternByPattern(string pattern);
    }
}