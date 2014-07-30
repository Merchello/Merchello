namespace Merchello.Core.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Configuration;
    using Umbraco.Core;

    /// <summary>
    /// Represents a PatternReplaceFormatter
    /// </summary>
    public class PatternReplaceFormatter : IPatternReplaceFormatter
    {
        /// <summary>
        /// The iteration start.
        /// </summary>
        private const string IterationStart = "{{IterationStart[";

        /// <summary>
        /// The iteration end.
        /// </summary>
        private const string IterationEnd = "{{IterationEnd[";

        /// <summary>
        /// The iteration cap.
        /// </summary>
        private const string IterationCap = "]}}";

        /// <summary>
        /// The _patterns.
        /// </summary>
        private readonly IDictionary<string, IReplaceablePattern> _patterns;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternReplaceFormatter"/> class.
        /// </summary>
        /// <param name="patterns">
        /// The patterns.
        /// </param>
        public PatternReplaceFormatter(IDictionary<string, IReplaceablePattern> patterns)
        {
            Mandate.ParameterNotNull(patterns, "patterns");
            _patterns = patterns;
        }

        /// <summary>
        /// Gets the Patterns dictionary.  Used for testing
        /// </summary>
        internal IDictionary<string, IReplaceablePattern> Patterns
        {
            get { return _patterns; }
        }

        /// <summary>
        /// Formats a message
        /// </summary>
        /// <param name="value">
        /// The value to be formatted
        /// </param>
        /// <returns>
        /// A formatted string
        /// </returns>
        public string Format(string value)
        {
            value = ExplodeLineItemIterations(value);
            return _patterns.Aggregate(value, (current, search) => current.Replace(search.Value.Pattern, search.Value.Replacement));
        }

        /// <summary>
        /// Adds or updates a replaceable pattern to the formatter
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
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
        /// <param name="patterns">
        /// The patterns.
        /// </param>
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
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/>.
        /// </returns>
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
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/>.
        /// </returns>
        public IReplaceablePattern GetReplaceablePatternByAlias(string alias)
        {
            return _patterns.FirstOrDefault(x => x.Key == alias).Value;
        }

        /// <summary>
        /// Gets a replaceable pattern from the formatter by the pattern to be replaced
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="IReplaceablePattern"/>.
        /// </returns>
        public IReplaceablePattern GetReplaceablePatternByPattern(string pattern)
        {
            return _patterns.FirstOrDefault(x => x.Value.Pattern == pattern).Value;
        }


        /// <summary>
        /// Static constructor that pre populates values initial values from the Merchello Config
        /// </summary>
        /// <returns>
        /// Returns an instantiated an instance of the <see cref="PatternReplaceFormatter"/>
        /// </returns>
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
        /// Replaces the short hand line item iteration call with a flushed out repeated patterns that
        /// can be used in the search and replace operation.
        /// </summary>
        /// <param name="value">
        /// The value to be worked on
        /// </param>
        /// <returns>
        /// Returns a string based on the value passed with line item identifiers repeated and suffixed with indexes matching
        /// their respective index in the enumeration (ex. 0,1,2,3)
        /// </returns>
        internal string ExplodeLineItemIterations(string value)
        {
            var identifier = GetIterationIdentifier(value);
            if (string.IsNullOrEmpty(identifier)) return value;

            var startMarker = IterationMarker(identifier);
            var endMarker = IterationMarker(identifier, false);

            var startBlockIndex = value.IndexOf(startMarker, StringComparison.InvariantCulture);
           
            var endBlockIndex =
                value.IndexOf(IterationMarker(identifier, false), StringComparison.InvariantCulture) +
                IterationMarker(identifier, false).Length;

            // this is the "block" to ultimately be replaced in the value
            var valueBlock = value.Substring(startBlockIndex, endBlockIndex - startBlockIndex);

            // this is what we are going to build the replacements with
            var repeatBlock = valueBlock.Replace(startMarker, string.Empty).Replace(endMarker, string.Empty);

            var sb = new StringBuilder();


            for (var i = 0; i < GetLineItemCount(identifier); i++)
            {
                sb.Append(repeatBlock.Replace("}}", "." + i + "}}"));
            }

            return value.Replace(valueBlock, sb.ToString());
        }


        /// <summary>
        /// Utility method to get the "iteration identifier" ex.  IternationStart[Invoice.Items] - where "Invoice.Items" is considered
        /// the identifier
        /// </summary>
        /// <param name="value">
        /// The value inspect for the token
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetIterationIdentifier(string value)
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
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="isStart">
        /// True or false indicatng whether or not the identifier is the start or end of the iteration to 
        /// be found.
        /// </param>
        /// <returns>
        /// The iteration marker
        /// </returns>
        private static string IterationMarker(string identifier, bool isStart = true)
        {
            return isStart
                ? string.Format("{0}{1}{2}", IterationStart, identifier, IterationCap)
                : string.Format("{0}{1}{2}", IterationEnd, identifier, IterationCap);
        }

        /// <summary>
        /// Gets the count of the line items based on a repeated pattern
        /// </summary>
        /// <param name="identifier">
        /// The identifier
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetLineItemCount(string identifier)
        {
            var keyStart = string.Format("{0}.Sku", identifier);
            return _patterns.Keys.Count(x => x.StartsWith(keyStart));
        }
    }
}