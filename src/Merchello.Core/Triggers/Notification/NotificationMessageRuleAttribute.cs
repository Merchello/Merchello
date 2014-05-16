using System;

namespace Merchello.Core.Triggers.Notification
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NotificationMessageRuleAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public NotificationMessageRuleAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}