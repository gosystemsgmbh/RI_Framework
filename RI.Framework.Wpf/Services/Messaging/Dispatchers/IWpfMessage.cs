using System.Windows.Threading;




namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Defines the interface which allows messages to have more control over the dispatching of the message when used with <see cref="WpfMessageDispatcher"/>.
	/// </summary>
	public interface IWpfMessage
	{
		/// <summary>
		///     Gets the priority for this message, if available.
		/// </summary>
		/// <value>
		///     The priority of the message or null if the dispatchers default priority should be used.
		/// </value>
		DispatcherPriority? Priority { get; }
	}
}
