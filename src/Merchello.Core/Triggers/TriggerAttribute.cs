using System;

namespace Merchello.Core.Triggers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TriggerAttribute : Attribute
    {
        public string Alias { get; private set; }

        public Type Service { get; private set; }

    }
}