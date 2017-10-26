using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus
{
	/// <summary>
	///     Defines the interface for a message bus.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The &quot;Local&quot; in <see cref="LocalBus" /> stands for the in-process locality of the message bus.
	///         Basically, each process (or application domain, if you distinguish those) has its own local message bus, distributing the messages to all receivers within the same process.
	///         However, multiple local message busses can be connected, across processes, machines, or networks, to form a global message bus which consists of independent but connected local message busses.
	///     </para>
	///     <para>
	///         <see cref="IBus" /> or its implementations respectively is the main interface used by bus users while the other interfaces are merely the actual implementation of the bus, hidden to the bus users.
	///     </para>
	///     <para>
	///         <see cref="IBus" /> initializes and unloads instances of implementations of the other bus interfaces, provides the runtime environment (including threading and exception handling) for the bus processing pipeline, and acts as the main interface to the bus user.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	/// TODO: Documentation
	public interface IBus : IDisposable, ISynchronizable, ILogSource
	{
		/// <summary>
		///     Gets or sets the timeout for collecting responses.
		/// </summary>
		/// <value>
		///     The timeout for collecting responses.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value should be 10 seconds.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is negative. </exception>
		TimeSpan CollectionTimeout { get; set; }

		/// <summary>
		///     Gets or sets whether messages are sent globally by default.
		/// </summary>
		/// <value>
		///     true if messages are sent globally by default, false otherwise.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value should be false.
		///     </note>
		/// </remarks>
		bool DefaultIsGlobal { get; set; }

		/// <summary>
		///     Gets whether the bus is started.
		/// </summary>
		/// <value>
		///     true if the bus is started, false otherwise.
		/// </value>
		bool IsStarted { get; }

		/// <summary>
		///     Gets or sets the polling interval used by the bus processing pipeline.
		/// </summary>
		/// <value>
		///     The polling interval.
		/// </value>
		/// <remarks>
		///     <para>
		///         The polling interval defines the interval in which <see cref="IBusPipeline.DoWork" /> is called when there is no work to be processed by the pipeline (hence &quot;polling&quot;).
		///     </para>
		///     <note type="implement">
		///         The default value should be 20 milliseconds.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is negative. </exception>
		TimeSpan PollInterval { get; set; }

		/// <summary>
		///     Gets the list of registered receivers managed by the bus.
		/// </summary>
		/// <value>
		///     The list of registered receivers.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         Do not use this property directly, it is intended to be used from bus and bus pipeline implementations.
		///         Use <see cref="Register" /> and <see cref="Unregister" /> for dealing with receiver registrations.
		///     </note>
		///     <note type="important">
		///         The returned list is not a copy and access to it needs to be synchronized using <see cref="SyncRoot" />.
		///     </note>
		///     <note type="implement">
		///         This property must never return null.
		///     </note>
		/// </remarks>
		List<ReceiverRegistrationItem> ReceiveRegistrations { get; }

		/// <summary>
		///     Gets or sets the timeout for waiting for responses.
		/// </summary>
		/// <value>
		///     The timeout for waiting for responses.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value should be 10 seconds.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is negative. </exception>
		TimeSpan ResponseTimeout { get; set; }

		/// <summary>
		///     Gets the list of enqueued send operations managed by the bus.
		/// </summary>
		/// <value>
		///     The list of enqueued send operations.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         Do not use this property directly, it is intended to be used from bus and bus pipeline implementations.
		///         Use <see cref="Enqueue" /> for dealing with send operations.
		///     </note>
		///     <note type="important">
		///         The returned list is not a copy and access to it needs to be synchronized using <see cref="SyncRoot" />.
		///     </note>
		///     <note type="implement">
		///         This property must never return null.
		///     </note>
		/// </remarks>
		List<SendOperationItem> SendOperations { get; }

		/// <inheritdoc cref="ISynchronizable.SyncRoot" />
		new object SyncRoot { get; }

		/// <summary>
		///     Enqueues a configured send operation to send its message.
		/// </summary>
		/// <param name="sendOperation"> The send operation. </param>
		/// <returns>
		///     The task used to wait for the response(s).
		///     The result of the task is the response (for a non-broadcast message) or the list of responses as a <see cref="List{T}" /> of <see cref="object" /> (for a broadcast message).
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="sendOperation" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="sendOperation" /> was not properly configured. </exception>
		/// <exception cref="InvalidOperationException"> The bus is not started. </exception>
		/// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
		Task<object> Enqueue (SendOperation sendOperation);

		/// <summary>
		///     Registers a configured receiver registration for use with the bus.
		/// </summary>
		/// <param name="receiverRegistration"> The receiver registration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="receiverRegistration" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="receiverRegistration" /> was not properly configured. </exception>
		void Register (ReceiverRegistration receiverRegistration);

		/// <summary>
		///     Signals that new work is pending (e.g. a message has been received through a bus connection) which must be processed by the bus processing pipeline.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The bus is not started. </exception>
		void SignalWorkAvailable ();

		/// <summary>
		///     Starts the bus and opens all connections to remote busses.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which is used for resolving types needed by the bus. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The bus is already started. </exception>
		void Start (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Stops the bus and closes all connections to remote busses.
		/// </summary>
		void Stop ();

		/// <summary>
		///     Unregisters a configured receiver registration to be no longer used with the bus.
		/// </summary>
		/// <param name="receiverRegistration"> The receiver registration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="receiverRegistration" /> is null. </exception>
		void Unregister (ReceiverRegistration receiverRegistration);
	}
}
