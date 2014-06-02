using System.Collections.Generic;
using System.Linq;

namespace Merchello.Core.Gateways.Notification.Formatters
{
    public class PatternReplaceNotificationFormatter : INotificationFormatter
    {
        private readonly IDictionary<string, string> _patterns;
 
        public PatternReplaceNotificationFormatter(IDictionary<string, string> patterns)
        {
            Mandate.ParameterNotNull(patterns, "patterns");
            _patterns = patterns;
        }

        public string Format(string message)
        {
            return _patterns.Aggregate(message, (current, pattern) => current.Replace(pattern.Key, pattern.Value));
        }
    }
}