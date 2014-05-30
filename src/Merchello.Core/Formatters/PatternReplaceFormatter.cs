using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Configuration;
using Umbraco.Core;

namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Represents a PatternReplaceFormatter
    /// </summary>
    public class PatternReplaceFormatter : IPatternReplaceFormatter
    {
        private readonly IDictionary<string, IReplaceablePattern> _patterns;
 
        public PatternReplaceFormatter(IDictionary<string, IReplaceablePattern> patterns)
        {
            Mandate.ParameterNotNull(patterns, "patterns");
            _patterns = patterns;
        }

        /// <summary>
        /// Formats a message
        /// </summary>
        /// <returns>A formatted string</returns>
        public string Format(string value)
        {

            return _patterns.Aggregate(value, (current, search) => current.Replace(search.Value.Pattern, search.Value.Replacement));
        }

        /// <summary>
        /// Static constructor that pre populates values initial values from the Merchello Config
        /// </summary>
        /// <returns><see cref="PatternReplaceFormatter"/></returns>
        internal static PatternReplaceFormatter CreateEmptyReplaceFormatter()
        {
            var dictionary = new Dictionary<string, IReplaceablePattern>();

            foreach (var replacement in MerchelloConfiguration.Current.PatternFormatter.GetReplacements().Where(x => x.ReplacementInMonitor).Select(config => new ReplaceablePattern(config)).Where(replacement => !dictionary.ContainsKey(replacement.Alias)))
            {
                dictionary.Add(replacement.Alias, replacement);
            }
            
            return new PatternReplaceFormatter(dictionary);
        }

        /// <summary>
        /// Adds or updates a replaceable pattern to the formatter
        /// </summary>
        public void AddOrUpdateReplaceablePattern(IReplaceablePattern pattern)
        {
            if (_patterns.ContainsKey(pattern.Alias))
            {
                _patterns[pattern.Alias].Pattern = pattern.Pattern;
                _patterns[pattern.Alias].Replacement = pattern.Replacement;
            }
            else
            {
                _patterns.Add(pattern.Alias, pattern);    
            }
            
        }

        /// <summary>
        /// Adds or updates collection a replaceable pattern to the formatter
        /// </summary>
        public void AddOrUpdateReplaceablePattern(IEnumerable<IReplaceablePattern> patterns)
        {
            patterns.ForEach(AddOrUpdateReplaceablePattern);
        }

        /// <summary>
        /// Sets (or resets) and existing patterns replacement value
        /// </summary>
        /// <param name="alias">The alias of the <see cref="IReplaceablePattern"/> which replacement value is to be updated</param>
        /// <param name="replacement">The new "replacement" value</param>
        public void SetReplacement(string alias, string replacement)
        {
            if (!_patterns.ContainsKey(alias)) return;
            _patterns.FirstOrDefault(x => x.Key == alias).Value.Replacement = replacement;
        }

        /// <summary>
        /// Removes a replaceable pattern from the formatter
        /// </summary>      
        public IReplaceablePattern RemoveReplaceablePattern(IReplaceablePattern pattern)
        {
            if (!_patterns.ContainsKey(pattern.Alias)) return null;
            var ret = _patterns[pattern.Alias];
            _patterns.Remove(pattern.Alias);
            return ret;
        }

        /// <summary>
        /// Gets a replaceable pattern from the formatter by it's unique alias
        /// </summary>
        public IReplaceablePattern GetReplaceablePatternByAlias(string alias)
        {
            return _patterns.FirstOrDefault(x => x.Key == alias).Value;
        }

        /// <summary>
        /// Gets a replaceable pattern from the formatter by the pattern to be replaced
        /// </summary>
        public IReplaceablePattern GetReplaceablePatternByPattern(string pattern)
        {
            return _patterns.FirstOrDefault(x => x.Value.Pattern == pattern).Value;
        }
    }
}