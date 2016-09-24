namespace Merchello.Core.Events
{
    using System;

    /// <summary>
    /// The typed event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <typeparam name="TSender">
    /// The type of the sender
    /// </typeparam>
    /// <typeparam name="TEventArgs">
    /// The type of the event arguments
    /// </typeparam>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Events/TypedEventHandler.cs"/>
    [Serializable]
	public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);
}