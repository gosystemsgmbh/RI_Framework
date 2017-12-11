using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Dispatchers;
using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
using RI.Framework.Bus.Routers;
using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus
{
	/// <summary>
	///     Defines the interface for a message bus.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <b> GENERAL </b>
	///     </para>
	///     <para>
	///         A message bus, as defined and implemented in this framework, is a construct which allows the sending and receving of messages to and from distributed senders/receivers with one or multiple recipients.
	///     </para>
	///     <para>
	///         <b> SENDING </b>
	///     </para>
	///     <para>
	///         A message is sent using a <see cref="SendOperation" />.
	///     </para>
	///     <para>
	///         For each message to send, a new <see cref="SendOperation" /> is created, configured (e.g. with address and payload), and then sent.
	///     </para>
	///     <para>
	///         When finally sending the message, a task is returned which will complete when the response is received or fail if there is an exception (e.g. no response within timeout).
	///     </para>
	///     <para>
	///         Note that there is always a response, even if the receiver or sender explicitly state that no result is required.
	///         In such cases, an &quot;empty&quot; response is sent solely for the purpose to notify the sender that the message has been received and processed by at least one receiver.
	///     </para>
	///     <para>
	///         <b> RECEIVING </b>
	///     </para>
	///     <para>
	///         A message can be received and processed using <see cref="ReceiverRegistration" />s.
	///     </para>
	///     <para>
	///         For each message which should be processed by a receiver, a separate <see cref="ReceiverRegistration" /> is created and registered with the bus.
	///         When a message, matching the receiver registration, is received, the specified message handling callback will be executed.
	///     </para>
	///     <para>
	///         Note that the response is only sent to the sender, when the message handling callback has returned.
	///     </para>
	///     <para>
	///         <b> SINGLE vs. BROADCAST </b>
	///     </para>
	///     <para>
	///         A message can be sent either with a single intended recipient (single) or with multiple intended recipients (broadcast).
	///     </para>
	///     <para>
	///         However, this only states what the sender expects regarding the number of responses and does not limit multiple recipients to handle a message intended for a single recipient.
	///         In such cases, the sender will only accept the first response and ignore the others.
	///     </para>
	///     <para>
	///         When sending a message to a single recipient, the sender waits for the (first) response as long as <see cref="ResponseTimeout" />, throwing <see cref="BusResponseTimeoutException" /> if no response was received within that time.
	///         The first response is then used as the result of the send task.
	///     </para>
	///     <para>
	///         When sending a message to multiple recipients, there are two ways the sender waits for responses.
	///         If a number of expected responses is specified, the send task completes as soon as that number of responses was received, ignoring all further responses.
	///         If no number of expected responses is specified, the send task completes always (more or less) exactly after the time specified by <see cref="CollectionTimeout" />.
	///         Either way, the send task always completes no later than <see cref="CollectionTimeout" /> after the message was sent, ignoring all responses received afterwards.
	///         The result of the send task is always a list of responses (which can include null for recipients which did not send a response payload) and will never throw <see cref="BusResponseTimeoutException" /> because the bus cannot know the actual number of receivers which should respond.
	///     </para>
	///     <para>
	///         <b> LOCAL vs. GLOBAL / LOCAL BUS vs. REMOTE BUS </b>
	///     </para>
	///     <para>
	///         The message bus distinguishes between &quot;local&quot; and &quot;global&quot;.
	///         Local means that a message is sent to the &quot;local bus&quot; which is always within the same process as the sender, thus never leaving process boundaries.
	///         Global means that a message is both sent to the local bus and any connected &quot;remote busses&quot;, thus probably leaving process, machine, and/or network boundaries.
	///     </para>
	///     <para>
	///         Therefore, the local/global concept can be used to only let messages cross process boundaries which are known to be used by remote bus recipients, saving bandwith (transmitting) and CPU time (serialization).
	///     </para>
	///     <para>
	///         Each <see cref="SendOperation" /> and <see cref="ReceiverRegistration" /> always has its local bus, the bus which is associated with it upon creation.
	///         If the associated bus has no connections to remote busses, the meaning of local and global are the same (meaning that the messages are only sent to the local bus and only received from the local bus).
	///         However, if the local bus has connections to remote busses, global messages are sent through these connections to those connected remote busses (which are inherenly also local busses for <see cref="SendOperation" />s and <see cref="ReceiverRegistration" />s associated with them).
	///         In other words: each bus is a local bus to itself, other, connected, busses are remote busses.
	///     </para>
	///     <para>
	///         Although local and global can be configured for a message when configuring its <see cref="SendOperation" />, the final decision whether to send a message and its responses locally/globally is done by the used <see cref="IBusRouter" />.
	///     </para>
	///     <para>
	///         The default local bus implementation, which can also be a remote bus to other local busses, is <see cref="LocalBus" />.
	///     </para>
	///     <para>
	///         <b> ADDRESS vs. PAYLOAD </b>
	///     </para>
	///     <para>
	///         The message bus can be used address-based, type-based, or both.
	///         The usage is defined on a per message basis, when configuring <see cref="SendOperation" />, or per receiver basis, when configuring <see cref="ReceiverRegistration" />, respectively.
	///     </para>
	///     <para>
	///         A message can be sent to a specified address.
	///         Each receiver which registered for this exact address will then process the message.
	///         This approach is used when the sender knows exactly which receiver has to process a message or to broadcast very simple messages without payloads.
	///     </para>
	///     <para>
	///         A message can also be sent with a specified payload (any serializable object).
	///         Each receiver which registered for that payload type will then process the message.
	///         This approach is used when the sender can not or must not know which receiver has to or can process a message (resulting in a more decoupled system).
	///     </para>
	///     <para>
	///         However, most useful messages will have a payload.
	///         Therefore, the determination whether a message is type-based or address-based is not solely done by the sender.
	///         The rule is as following:
	///         If the sender specifies an address, the message is address-based, regardless of the payload (if any).
	///         If the sender specifies an address and a payload, the receiver registration defines the type: receiver configured for a specified address only, accepting any payload type (address-based), -or- receiver configured for a specified address and specified payload type (both address- and type-based).
	///         If the sender only specifies the payload, the message is type-based.
	///     </para>
	///     <para>
	///         However, whether a receiver will receive a certain message is eventually decided by the used <see cref="IBusRouter" />.
	///     </para>
	///     <para>
	///         <b> IBus </b>
	///     </para>
	///     <para>
	///         An <see cref="IBus" /> implementation is the front-end of the message bus towards the message bus users, meaning that only an <see cref="IBus" /> instance is needed to perform normal bus operations (sending and receiving of messages).
	///         Besides being the API for sending and receiving, <see cref="IBus" /> is responsible for instantiating and managing the other message bus components as described below.
	///     </para>
	///     <para>
	///         As described above, an <see cref="IBus" /> implementation is inherently meant to be a local bus for all its associated <see cref="SendOperation" />s and <see cref="ReceiverRegistration" />s.
	///     </para>
	///     <para>
	///         A default local bus, which allows optional connections to remote busses, is implemented in <see cref="LocalBus" />.
	///     </para>
	///     <para>
	///         <b> IBusPipeline </b>
	///     </para>
	///     <para>
	///         <see cref="IBusPipeline" /> is responsible for interconnecting all the message bus components, the control of the message flow between the components, and the timeout and connection state handling.
	///     </para>
	///     <para>
	///         Exactly one <see cref="IBusPipeline" /> instance is required for one <see cref="IBus" /> instance.
	///     </para>
	///     <para>
	///         A default <see cref="IBusPipeline" /> implementation is provided by <see cref="DefaultBusPipeline" />.
	///     </para>
	///     <para>
	///         <b> IBusDispatcher </b>
	///     </para>
	///     <para>
	///         <see cref="IBusDispatcher" /> is responsible for executing the processing of received messages.
	///         This is, depending on the actual <see cref="IBusDispatcher" /> implementation, usually done by dispatching the corresponding message callbacks to the desired execution contexts, allowing the control on which thread and/or in which execution context a received message is processed (and also allowing serialization/synchronization of message processing).
	///     </para>
	///     <para>
	///         Although mainly used for dispatching the processing of received messages, the other bus components can use <see cref="IBusDispatcher" /> for their own purposes.
	///     </para>
	///     <para>
	///         Exactly one <see cref="IBusDispatcher" /> instance is required for one <see cref="IBus" /> instance.
	///     </para>
	///     <para>
	///         <b> IBusRouter </b>
	///     </para>
	///     <para>
	///         <see cref="IBusRouter" /> is responsible for deciding whether a message is sent globally, which remote connections are used to send a message, and whether a certain receiver will receive/process a message.
	///     </para>
	///     <para>
	///         Exactly one <see cref="IBusRouter" /> instance is required for one <see cref="IBus" /> instance.
	///     </para>
	///     <para>
	///         A default <see cref="IBusRouter" /> implementation is provided by <see cref="DefaultBusRouter" />.
	///     </para>
	///     <para>
	///         <b> IBusConnection &amp; IBusConnectionManager </b>
	///     </para>
	///     <para>
	///         An <see cref="IBusConnection" /> implementation implements a single connection from the local bus (to which the instance of <see cref="IBusConnection" /> belongs) to a remote bus.
	///         This is either a unidirectional or bidirectional connection for sending and/or receiving messages from the remote bus, depending on the actual used connection or its implementation respectively.
	///     </para>
	///     <para>
	///         <see cref="IBusConnection" /> is responsible for serializing messages, transmitting messages, and ensuring that the remote bus successfully received the messages (or detection of transmission failures respectively).
	///     </para>
	///     <para>
	///         There can be zero, one, or more <see cref="IBusConnection" /> instances for one <see cref="IBus" /> instance.
	///         Having no <see cref="IBusConnection" /> instances makes a bus a local-only bus.
	///     </para>
	///     <para>
	///         When using one or more <see cref="IBusConnection" /> instances with an <see cref="IBus" /> instance, exactly one <see cref="IBusConnectionManager" /> instance is required, otherwise its optional.
	///     </para>
	///     <para>
	///         <see cref="IBusConnectionManager" /> is responsible for managing the connections, forwarding a sent message to all available connections, and collecting received messages from all available connections.
	///         Therefore, other bus components should not directly use <see cref="IBusConnection" />, if possible, but rather use <see cref="IBusConnectionManager" />.
	///     </para>
	///     <para>
	///         <b> INSTANTIATION OF BUS COMPONENTS </b>
	///     </para>
	///     <para>
	///         As mentioned above, the instantiation of all the bus components is done by an <see cref="IBus" /> implementation.
	///         To do this, <see cref="IBus" /> uses <see cref="IDependencyResolver" /> which is passed to <see cref="Start(IDependencyResolver)" />.
	///     </para>
	///     <para>
	///         <b> EXCEPTION HANDLING AND FORWARDING </b>
	///     </para>
	///     <para>
	///         Depending on your applications needs and the context under which received messages are processed, you might want to forward exceptions, thrown by receivers when processing messages, back to the sender.
	///         This is called &quot;exception forwarding&quot; and can be enabled by the sender, the receiver, or an exception handler registered with a receiver.
	///     </para>
	///     <para>
	///         An exception is forwarded, and does therefore not lead to an unhandled exception in the receivers <see cref="IBusDispatcher" /> context, when an exception is thrown by the callback registered with a <see cref="ReceiverRegistration" /> AND if ANY of the following is true:
	///         The <see cref="SendOperation" /> enabled exception forwarding, the <see cref="ReceiverRegistration" /> enabled exception forwarding, the handler for <see cref="ProcessingException" />, if any, enabled exception forwarding, OR the <see cref="ReceiverRegistration" /> used a <see cref="ReceiverExceptionHandler" /> which enabled exception forwarding.
	///     </para>
	///     <para>
	///         When an exception from the receiver is forwarded to the sender, it will not be handled and therefore silently ignored on the receiver side, except a <see cref="ReceiverExceptionHandler" /> is registered with the corresponding <see cref="ReceiverRegistration" />.
	///         Instead, the sender or its task respectively will throw a <see cref="BusMessageProcessingException" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
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
		///     Gets or sets whether exception forwarding is used by default.
		/// </summary>
		/// <value>
		///     true if exception forwarding is used by default, false otherwise.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value should be true.
		///     </note>
		/// </remarks>
		bool DefaultExceptionForwarding { get; set; }

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
		///     Raised when a connection broke.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusConnectionEventArgs> ConnectionBroken;

		/// <summary>
		///     Raised when an exception occured while processing a received message using a <see cref="ReceiverRegistration" />.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusMessageProcessingExceptionEventArgs> ProcessingException;

		/// <summary>
		///     Raised after a request message was received from local or global but before it is processed.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusMessageEventArgs> ReceivingRequest;

		/// <summary>
		///     Raised after a response message was received from local or global but before it is added to the corresponding <see cref="SendOperation" />.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusMessageEventArgs> ReceivingResponse;

		/// <summary>
		///     Raised after a request message was created but before it is sent to local and/or global.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusMessageEventArgs> SendingRequest;

		/// <summary>
		///     Raised after a response message was created but before it is sent to local and/or global.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Be aware of the thread this event is raised on, depending on the used bus components or their implementation respectively.
		///     </note>
		/// </remarks>
		event EventHandler<BusMessageEventArgs> SendingResponse;

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
		///     Raises the <see cref="ConnectionBroken" /> event.
		/// </summary>
		/// <param name="connection"> The connection. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
		void RaiseConnectionBroken (IBusConnection connection);

		/// <summary>
		///     Raises the <see cref="ProcessingException" /> event.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="result"> The result to respond to the sender. </param>
		/// <param name="exception"> Specifies and returns the exception. </param>
		/// <param name="forwardException"> Specifies and returns whether the exception should be forwarded to the sender. </param>
		/// <returns>
		///     The result to respond to the sender.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="exception" /> is null. </exception>
		object RaiseProcessingException (MessageItem message, object result, ref Exception exception, ref bool forwardException);

		/// <summary>
		///     Raises the <see cref="ReceivingRequest" /> event.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void RaiseReceivingRequest (MessageItem message);

		/// <summary>
		///     Raises the <see cref="ReceivingRequest" /> event.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void RaiseReceivingResponse (MessageItem message);

		/// <summary>
		///     Raises the <see cref="SendingRequest" /> event.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void RaiseSendingRequest (MessageItem message);

		/// <summary>
		///     Raises the <see cref="SendingResponse" /> event.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is not intended to be used by the message bus user.
		///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void RaiseSendingResponse (MessageItem message);

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
