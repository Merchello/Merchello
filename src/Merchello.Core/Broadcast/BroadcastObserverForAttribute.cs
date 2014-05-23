using System;

namespace Merchello.Core.Broadcast
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BroadcastObserverForAttribute : Attribute
    {
        /// <summary>
        /// Gets the mandatory key for the trigger
        /// </summary>        
        public Guid Key { get; private set; }
       
        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public BroadcastType Area { get; private set; }

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The name of the broadcaster
        /// </summary>
        public string Alias { get; private set; }

        // ctor
        public BroadcastObserverForAttribute(string key, string name, string alias, BroadcastType area = BroadcastType.Notification)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            Key = new Guid(key);
            Alias = alias;
            Name = name;
            Area = area;            
        }
    }
}