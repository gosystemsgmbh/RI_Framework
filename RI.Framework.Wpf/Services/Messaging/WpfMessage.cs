using System;
using System.Windows.Threading;

using RI.Framework.Services.Messaging.Dispatchers;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Implements a default message usable in WPF applications (using a <see cref="WpfMessageDispatcher" />).
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="Message" /> for more details.
	///     </para>
	/// </remarks>
	public class WpfMessage : Message, IWpfMessage
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WpfMessage" />.
		/// </summary>
		/// <param name="name"> The name of the message. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public WpfMessage (string name)
			: base(name)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="WpfMessage" />.
		/// </summary>
		/// <param name="name"> The name of the message. </param>
		/// <param name="priority"> The priority of the message. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public WpfMessage (string name, DispatcherPriority priority)
			: this(name)
		{
			this.Priority = priority;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <inheritdoc cref="IWpfMessage.Priority" />
		public DispatcherPriority Priority { get; set; }

		#endregion




		#region Interface: IWpfMessage

		/// <inheritdoc />
		DispatcherPriority? IWpfMessage.Priority => this.Priority;

		#endregion
	}
}
