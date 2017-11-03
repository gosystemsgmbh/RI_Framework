using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
using RI.Framework.Bus.Routers;
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
	/// 
	/// <para>
	/// <b> GENERAL </b>
	/// </para>
	/// <para>
	/// A message bus, as defined and implemented in this framework, is a construct which allows the sending and receving of messages to and from distributed senders/receivers with one or multiple recipients.
	/// </para>
	/// <para>
	/// <b> SENDING </b>
	/// </para>
	/// <para>
	/// A message is sent using a <see cref="SendOperation"/>.
	/// </para>
	/// <para>
	/// For each message to send, a new <see cref="SendOperation"/> is created, configured (e.g. with address and payload), and then sent.
	/// </para>
	/// <para>
	/// When finally sending the message, a task is returned which will complete when the response is received or fail if there is an exception (e.g. no response within timeout).
	/// </para>
	/// <para>
	/// Note that there is always a response, even if the receiver or sender explicitly state that no result is required.
	/// In such cases, an &quot;empty&quot; response is sent solely for the purpose to notify the sender that the message has been received and processed by at least one receiver.
	/// </para>
	/// <para>
	/// <b> RECEIVING </b>
	/// </para>
	/// <para>
	/// A message can be received and processed using <see cref="ReceiverRegistration"/>s.
	/// </para>
	/// <para>
	/// For each message which should be processed by a receiver, a separate <see cref="ReceiverRegistration"/> is created and registered with the bus.
	/// When a message, matching the receiver registration, is received, the specified message handling callback will be executed.
	/// </para>
	/// <para>
	/// Note that the response is only sent to the sender, when the message handling callback has returned.
	/// </para>
	/// <para>
	/// <b> SINGLE vs. BROADCAST </b>
	/// </para>
	/// <para>
	/// A message can be sent either with a single intended recipient (single) or with multiple intended recipients (broadcast).
	/// </para>
	/// <para>
	/// However, this only states what the sender expects regarding the number of responses and does not limit multiple recipients to handle a message intended for a single recipient.
	/// In such cases, the sender will only accept the first response and ignore the others.
	/// </para>
	/// <para>
	/// When sending a message to a single recipient, the sender waits for the (first) response as long as <see cref="ResponseTimeout"/>, throwing <see cref="BusResponseTimeoutException"/> if no response was received within that time.
	/// The first response is then used as the result of the send task.
	/// </para>
	/// <para>
	/// When sending a message to multiple recipients, there are two ways the sender waits for responses.
	/// If a number of expected responses is specified, the send task completes as soon as that number of responses was received, ignoring all further responses.
	/// If no number of expected responses is specified, the send task completes always (more or less) exactly after the time specified by <see cref="CollectionTimeout"/>.
	/// Either way, the send task always completes no later than <see cref="CollectionTimeout"/> after the message was sent, ignoring all responses received afterwards.
	/// The result of the send task is always a list of responses (which can include null for recipients which did not send a response payload) and will never throw <see cref="BusResponseTimeoutException"/> because the bus cannot know the actual number of receivers which should respond.
	/// </para>
	/// <para>
	/// <b> LOCAL vs. GLOBAL / LOCAL BUS vs. REMOTE BUS</b>
	/// </para>
	/// <para>
	/// The message bus distinguishes between &quot;local&quot; and &quot;global&quot;.
	/// Local means that a message is sent to the &quot;local bus&quot; which is always within the same process as the sender, thus never leaving process boundaries.
	/// Global means that a message is both sent to the local bus and any connected &quot;remote busses&quot;, thus probably leaving process, machine, and/or network boundaries.
	/// </para>
	/// <para>
	/// Therefore, the local/global concept can be used to only let messages cross process boundaries which are known to be used by remote bus recipients, saving bandwith (transmitting) and CPU time (serialization).
	/// </para>
	/// <para>
	/// Each <see cref="SendOperation"/> and <see cref="ReceiverRegistration"/> always has its local bus, the bus which is associated with it upon creation.
	/// If the associated bus has no connections to remote busses, the meaning of local and global are the same (meaning that the messages are only sent to the local bus and only received from the local bus).
	/// However, if the local bus has connections to remote busses, global messages are sent through these connections to those connected remote busses (which are inherenly also local busses for <see cref="SendOperation"/>s and <see cref="ReceiverRegistration"/>s associated with them).
	/// In other words: each bus is a local bus to itself, other, connected, busses are remote busses.
	/// </para>
	/// <para>
	/// Although local and global can be configured for a message when configuring its <see cref="SendOperation"/>, the final decision whether to send a message and its responses locally/globally is done by the used <see cref="IBusRouter"/>.
	/// </para>
	/// <para>
	/// The default local bus implementation, which can also be a remote bus to other local busses, is <see cref="LocalBus"/>.
	/// </para>
	/// <para>
	/// <b> ADDRESS vs. PAYLOAD </b>
	/// </para>
	/// <para>
	/// The message bus can be used address-based, type-based, or both.
	/// The usage is defined on a per message basis, when configuring <see cref="SendOperation"/>, or per receiver basis, when configuring <see cref="ReceiverRegistration"/>, respectively.
	/// </para>
	/// <para>
	/// A message can be sent to a specified address.
	/// Each receiver which registered for this exact address will then process the message.
	/// This approach is used when the sender knows exactly which receiver has to process a message or to broadcast very simple messages without payloads.
	/// </para>
	/// <para>
	/// A message can also be sent with a specified payload (any serializable object).
	/// Each receiver which registered for that payload type will then process the message.
	/// This approach is used when the sender can not or must not know which receiver has to or can process a message (resulting in a more decoupled system).
	/// </para>
	/// <para>
	/// However, most useful messages will have a payload.
	/// Therefore, the determination whether a message is type-based or address-based is not solely done by the sender.
	/// The rule is as following:
	/// If the sender specifies an address, the message is address-based, regardless of the payload (if any).
	/// If the sender specifies an address and a payload, the receiver registration defines the type: receiver configured for a specified address only, accepting any payload type (address-based), or receiver configured for a specified address and specified payload type (both address- and type-based).
	/// If the sender only specifies the payload, the message is type-based.
	/// </para>
	/// <para>
	///
	/// router
	/// 
	/// </para>
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
