namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System;

    /// <summary>
    /// Decorates a monitor with the type of model.
    /// </summary>
    /// <remarks>
    /// Primarily used for notification messages that use partial views
    /// </remarks>
    public class MonitorModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorModelAttribute"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public MonitorModelAttribute(Type model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public Type Model { get; private set; }
    }
}