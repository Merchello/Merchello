using System.Collections.Generic;

namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Defines a PatternReplaceFormatter
    /// </summary>
    public interface IPatternReplaceFormatter : IFormatter
    {
        /// <summary>
        /// Adds a replaceable pattern to the formatter
        /// </summary>
        void AddOrUpdateReplaceablePattern(IReplaceablePattern pattern);

        /// <summary>
        /// Adds collection a replaceable pattern to the formatter
        /// </summary>
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
        IReplaceablePattern RemoveReplaceablePattern(IReplaceablePattern pattern);

        /// <summary>
        /// Gets a replaceable pattern from the formatter by it's unique alias
        /// </summary>
        IReplaceablePattern GetReplaceablePatternByAlias(string alias);

        /// <summary>
        /// Gets a replaceable pattern from the formatter by the pattern to be replaced
        /// </summary>
        IReplaceablePattern GetReplaceablePatternByPattern(string pattern);
    }
}