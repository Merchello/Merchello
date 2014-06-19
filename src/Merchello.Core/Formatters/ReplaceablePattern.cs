namespace Merchello.Core.Formatters
{
    using Configuration;
    using Configuration.Outline;

    /// <summary>
    /// Represents a replaceable pattern
    /// </summary>
    public class ReplaceablePattern : IReplaceablePattern
    {
        public ReplaceablePattern(string alias, string pattern, string replacement)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
            Mandate.ParameterNotNullOrEmpty(pattern, "pattern");

            Alias = alias;
            Pattern = pattern;
            Replacement = replacement;
        }

        internal ReplaceablePattern(ReplaceElement config)
            : this(config.Alias, config.Pattern, config.Replacement)
        {            
        }

        /// <summary>
        /// Gets the unique alias of the pattern
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets or sets the patterned to be search for
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the replacement for the pattern
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