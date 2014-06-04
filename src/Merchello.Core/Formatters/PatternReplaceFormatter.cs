using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            value = ExplodeLineItemIterations(value);
            return _patterns.Aggregate(value, (current, search) => current.Replace(search.Value.Pattern, search.Value.Replacement));
        }

        /// <summary>
        /// Static constructor that pre populates values initial values from the Merchello Config
        /// </summary>
        /// <returns><see cref="PatternReplaceFormatter"/></returns>
        internal static IPatternReplaceFormatter GetPatternReplaceFormatter()
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

        private const string IterationStart = "{{IterationStart[";
        private const string IterationEnd = "{{IterationEnd[";
        private const string IterationCap = "]}}";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal string ExplodeLineItemIterations(string value)
        {
            var token = GetIterationToken(value);
            if (string.IsNullOrEmpty(token)) return token;

            var startMarker = IterationMarker(token);
            var endMarker = IterationMarker(token, false);

            var startBlockIndex = value.IndexOf(startMarker, StringComparison.InvariantCulture);
           
            var endBlockIndex =
                value.IndexOf(IterationMarker(token, false), StringComparison.InvariantCulture) +
                IterationMarker(token, false).Length;

            // this is the "block" to ultimately be replaced in the value
            var valueBlock = value.Substring(startBlockIndex, endBlockIndex - startBlockIndex);

            // this is what we are going to build the replacements with
            var repeatBlock = valueBlock.Replace(startMarker, string.Empty).Replace(endMarker, string.Empty);

            var sb = new StringBuilder();


            for (var i = 0; i < GetLineItemCount(token); i++)
            {
                sb.Append(repeatBlock.Replace("}}", "." + i + "}}"));
            }

            return value.Replace(valueBlock, sb.ToString());
        }


        private int GetLineItemCount(string token)
        {
            var keyStart = string.Format("{0}.Sku", token);
            return _patterns.Keys.Count(x => x.StartsWith(keyStart));
        }
     
        /// <summary>
        /// Utility method to get the "iteration token" eg.  IternationStart[Invoice.Items] - where "Invoice.Items" is considered
        /// the token
        /// </summary>        
        private static string GetIterationToken(string value)
        {
            var startIndex = value.IndexOf(IterationStart, StringComparison.InvariantCulture);

            if (startIndex <= 0) return string.Empty;

            var start = startIndex + IterationStart.Length;
            var end = value.IndexOf(IterationCap, startIndex, StringComparison.InvariantCulture);
            return value.Substring(start, end - start);
        }

        /// <summary>
        /// Helper method to construct the starting and ending patterns for replacing iteration markers
        /// </summary>        
        private static string IterationMarker(string token, bool isStart = true)
        {
            return isStart
                ? string.Format("{0}{1}{2}", IterationStart, token, IterationCap)
                : string.Format("{0}{1}{2}", IterationEnd, token, IterationCap);
        }

        

        /// <summary>
        /// Used for testing
        /// </summary>
        internal IDictionary<string, IReplaceablePattern> Patterns
        {
            get { return _patterns; }
        }
    }
}