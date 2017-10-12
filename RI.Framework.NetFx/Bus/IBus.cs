using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus
{
	/// <summary>
	/// Defines the interface for a message bus.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	/// TODO: Documentation
	public interface IBus : IDisposable, ISynchronizable
	{
		/// <summary>
		/// Registers a configured receiver registration for use with the bus.
		/// </summary>
		/// <param name="receiverRegistration">The receiver registration.</param>
		/// <exception cref="ArgumentNullException"><paramref name="receiverRegistration"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The bus is stopped.</exception>
		/// <exception cref="LocalBusException">The bus processing pipeline encountered an exception.</exception>
		void Register (ReceiverRegistration receiverRegistration);

		/// <summary>
		/// Unregisters a configured receiver registration to be no longer used with the bus.
		/// </summary>
		/// <param name="receiverRegistration">The receiver registration.</param>
		/// <exception cref="ArgumentNullException"><paramref name="receiverRegistration"/> is null.</exception>
		void Unregister (ReceiverRegistration receiverRegistration);

		/// <summary>
		/// Enqueues a configured send operation to send its message.
		/// </summary>
		/// <param name="sendOperation">The send operation.</param>
		/// <returns>
		/// The task used to wait for the response(s).
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="sendOperation"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The bus is stopped.</exception>
		/// <exception cref="LocalBusException">The bus processing pipeline encountered an exception.</exception>
		Task<object> Enqueue (SendOperation sendOperation);

		/// <summary>
		/// Creates a new send operation which can be configured.
		/// </summary>
		/// <returns>
		/// The new send operation.
		/// </returns>
		/// <exception cref="InvalidOperationException">The bus is stopped.</exception>
		/// <exception cref="LocalBusException">The bus processing pipeline encountered an exception.</exception>
		SendOperation Send ();

		/// <summary>
		/// Creates a new receiver registration which can be configured.
		/// </summary>
		/// <returns>
		/// The new receiver registration.
		/// </returns>
		/// <exception cref="InvalidOperationException">The bus is stopped.</exception>
		/// <exception cref="LocalBusException">The bus processing pipeline encountered an exception.</exception>
		ReceiverRegistration Receive ();

		/// <summary>
		/// Stops the bus and closes all remote connections.
		/// </summary>
		void Stop ();

		/// <summary>
		/// Starts the bus and opens all remote connections.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which is used for resolving types needed by the bus.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The bus is already started.</exception>
		void Start (IDependencyResolver dependencyResolver);

		/// <summary>
		/// Gets whether the bus is started.
		/// </summary>
		/// <value>
		/// true if the bus is started, false otherwise.
		/// </value>
		bool IsStarted { get; }

		/// <summary>
		/// Gets or sets the timeout for waiting for responses.
		/// </summary>
		/// <value>
		/// The timeout for waiting for responses.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value should be 10 seconds.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
		TimeSpan ResponseTimeout { get; set; }

		/// <summary>
		/// Gets or sets the timeout for collecting responses.
		/// </summary>
		/// <value>
		/// The timeout for collecting responses.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value should be 10 seconds.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
		TimeSpan CollectionTimeout { get; set; }

		/// <summary>
		/// Gets or sets whether messages are sent globally by default.
		/// </summary>
		/// <value>
		/// true if messages are sent globally by default, false otherwise.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value should be false.
		/// </note>
		/// </remarks>
		bool DefaultIsGlobal { get; set; }

		/// <summary>
		/// Gets or sets the polling interval used by the bus processing pipeline.
		/// </summary>
		/// <value>
		/// The polling interval.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value should be 20 milliseconds.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
		TimeSpan PollInterval { get; set; }

		/// <inheritdoc cref="ISynchronizable.SyncRoot"/>
		new object SyncRoot { get; }

		/// <summary>
		/// Gets the list of enqueued send operations.
		/// </summary>
		/// <value>
		/// The list of enqueued send operations.
		/// </value>
		/// <remarks>
		/// <note type="important">
		/// The returned list is not a copy and access to it needs to be synchronized using <see cref="SyncRoot"/>.
		/// </note>
		/// <note type="implement">
		/// This property must never return null.
		/// </note>
		/// </remarks>
		List<SendOperationItem> SendOperations { get; }

		/// <summary>
		/// Gets the list of registered receivers.
		/// </summary>
		/// <value>
		/// The list of registered receivers.
		/// </value>
		/// <remarks>
		/// <note type="important">
		/// The returned list is not a copy and access to it needs to be synchronized using <see cref="SyncRoot"/>.
		/// </note>
		/// <note type="implement">
		/// This property must never return null.
		/// </note>
		/// </remarks>
		List<ReceiveRegistrationItem> ReceiveRegistrations { get; }
	}
}
