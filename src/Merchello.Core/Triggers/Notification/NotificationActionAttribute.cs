using System;

namespace Merchello.Core.Triggers.Notification
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NotificationActionAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public NotificationActionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}