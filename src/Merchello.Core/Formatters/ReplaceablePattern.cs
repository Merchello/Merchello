using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Represents a replaceable pattern
    /// </summary>
    public class ReplaceablePattern : IReplaceablePattern
    {
        internal ReplaceablePattern(ReplaceElement config)
            : this(config.Alias, config.Pattern, config.Replacement)
        {}

        public ReplaceablePattern(string alias, string pattern, string replacement)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
            Mandate.ParameterNotNullOrEmpty(pattern, "pattern");

            Alias = alias;
            Pattern = pattern;
            Replacement = replacement;
        }

        /// <summary>
        /// The unique alias of the pattern
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The patterned to be search for
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The replacement for the pattern
        /// </summary>
        public string Replacement { get; set; }


        internal static ReplaceablePattern GetConfigurationReplaceablePattern(string alias, string replacement = "")
        {
            if (MerchelloConfiguration.Current.PatternFormatter[alias] == null) return null;
            
            var pattern = new ReplaceablePattern(MerchelloConfiguration.Current.PatternFormatter[alias]);
            
            if (!string.IsNullOrEmpty(replacement)) pattern.Replacement = replacement;
            
            return pattern;
        }
    }

    
}