using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Bus.Exceptions;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Represents a single send operation.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Use TaskScheduler optionally provided by dispatcher (?????)
    public sealed class SendOperation : ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SendOperation" />.
        /// </summary>
        /// <param name="bus"> The bus to be associated with this send operation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> is null. </exception>
        public SendOperation (IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.SyncRoot = new object();
            this.Bus = bus;

            this.OperationType = SendOperationType.Undefined;

            this.Address = null;
            this.Global = null;
            this.Payload = null;
            this.ResponseType = null;
            this.ExpectedResults = null;
            this.Timeout = null;
            this.CancellationToken = null;
            this.ExceptionForwarding = null;
            this.ExceptionHandler = null;
            this.IgnoreBrokenConnections = null;

            this.IsProcessed = false;

            this.Result = null;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the address the message is sent to.
        /// </summary>
        /// <value>
        ///     The address the message is sent to or null if no address is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="ToAddress" />.
        ///     </para>
        /// </remarks>
        public string Address { get; private set; }

        /// <summary>
        ///     Gets the bus this send operation is associated with.
        /// </summary>
        /// <value>
        ///     The bus this send operation is associated with.
        /// </value>
        public IBus Bus { get; }

        /// <summary>
        ///     Gets the cancellation token which can be used to cancel the wait for responses or the collection of responses.
        /// </summary>
        /// <value>
        ///     The cancellation token which can be used to cancel the wait for responses or the collection of responses or null if no cancellation token is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithCancellation" />.
        ///     </para>
        /// </remarks>
        public CancellationToken? CancellationToken { get; private set; }

        /// <summary>
        ///     Gets or sets whether exception forwarding to the sender is used when the message is processed by a receiver.
        /// </summary>
        /// <value>
        ///     true if exception forwarding is used, false if not, null if not defined where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithExceptionForwarding()" />, <see cref="WithExceptionForwarding(bool?)" />.
        ///     </para>
        /// </remarks>
        public bool? ExceptionForwarding { get; private set; }

        /// <summary>
        ///     Gets the used exception handler.
        /// </summary>
        /// <value>
        ///     The used exception handler or null if no specific exception handler is used by this send operation where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithExceptionHandler" />.
        ///     </para>
        /// </remarks>
        public SendExceptionHandler ExceptionHandler { get; private set; }

        /// <summary>
        ///     Gets the number of expected responses.
        /// </summary>
        /// <value>
        ///     The number of expected responses or null if no such number is specified.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="AsBroadcast(int?)" />, <see cref="AsBroadcast(int?,Type)" />, <see cref="AsBroadcast{T}(int?)" />.
        ///     </para>
        /// </remarks>
        public int? ExpectedResults { get; private set; }

        /// <summary>
        ///     Gets whether the message is sent globally.
        /// </summary>
        /// <value>
        ///     true if the message is sent globally, false if locally, null if not defined where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="ToGlobal()" />, <see cref="ToGlobal(bool?)" />, <see cref="ToLocal()" />, <see cref="ToLocal(bool?)" />.
        ///     </para>
        /// </remarks>
        public bool? Global { get; private set; }

        /// <summary>
        ///     Gets whether broken connections should be ignored.
        /// </summary>
        /// <value>
        ///     true if broken connections should be ignored (favoring timeouts), false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is false.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithIgnoredBrokenConnections()" />, <see cref="WithIgnoredBrokenConnections(bool?)" />.
        ///     </para>
        /// </remarks>
        public bool? IgnoreBrokenConnections { get; private set; }
        
        /// <summary>
        ///     Gets whether this send operation is being processed.
        /// </summary>
        /// <value>
        ///     true if the send operation is being processed, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is false.
        ///     </para>
        /// </remarks>
        public bool IsProcessed { get; private set; }

        /// <summary>
        ///     Gets whether the message is sent locally.
        /// </summary>
        /// <value>
        ///     true if the message is sent locally, false if globally, null if not defined where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="ToGlobal()" />, <see cref="ToGlobal(bool?)" />, <see cref="ToLocal()" />, <see cref="ToLocal(bool?)" />.
        ///     </para>
        /// </remarks>
        public bool? Local => this.Global.HasValue ? !this.Global.Value : (bool?)null;

        /// <summary>
        ///     Gets the send operation type of this send operation.
        /// </summary>
        /// <value>
        ///     The send operation type of this send operation.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is <see cref="SendOperationType.Undefined" />.
        ///     </para>
        /// </remarks>
        public SendOperationType OperationType { get; private set; }

        /// <summary>
        ///     Gets the payload of the message.
        /// </summary>
        /// <value>
        ///     The payload of the message or null if no payload is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithPayload" />.
        ///     </para>
        /// </remarks>
        public object Payload { get; private set; }

        /// <summary>
        ///     Gets the expected response type.
        /// </summary>
        /// <value>
        ///     The expected response type or null if no response is expected.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithResponse{TResponse}" />, <see cref="WithResponse(Type)" />.
        ///     </para>
        /// </remarks>
        public Type ResponseType { get; private set; }

        /// <summary>
        ///     Gets the timeout for the message (waiting for or collecting responses).
        /// </summary>
        /// <value>
        ///     The timeout for the message or null if not defined where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithTimeout(TimeSpan?)" />, <see cref="WithTimeout(int?)" />.
        ///     </para>
        /// </remarks>
        public TimeSpan? Timeout { get; private set; }

        private object Result { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Sends the message to one or more receivers and waits for zero, one, or more responses.
        /// </summary>
        /// <param name="expectedResults"> The number of expected responses or null if any number is accepted (collecting as long as the the timeout). </param>
        /// <returns>
        ///     The task used to wait until the timeout for completing round-trips expired or the expected number of responses was received.
        ///     The tasks result is the list of received responses (can be empty).
        /// </returns>
        /// <remarks>
        ///     <note type="important">
        ///         <see cref="AsBroadcast(int?)" /> does not throw <see cref="BusResponseTimeoutException" /> for not responding receivers.
        ///         Not responding receivers simply do not show up in the list of responses.
        ///     </note>
        ///     <para>
        ///         The collection of responses end after the used timeout or the expected number of responses was received (<paramref name="expectedResults" />).
        ///     </para>
        ///     <para>
        ///         The response type is defined by <see cref="ResponseType" />.
        ///         If <see cref="ResponseType" /> is null, any response is accepted.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="expectedResults" /> is below zero. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> One of the responses could not be casted to type <see cref="ResponseType" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<List<object>> AsBroadcast (int? expectedResults)
        {
            if (expectedResults.HasValue)
            {
                if (expectedResults.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(expectedResults));
                }
            }

            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Broadcast;
                this.ExpectedResults = expectedResults;
                task = this.Bus.Enqueue(this);
            }

            Task<List<object>> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                List<object> list = ((IEnumerable)this.Result).ToList();
                list.ForEach(this.VerifyResponse);
                return list;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <inheritdoc cref="AsBroadcast(int?)" />
        public Task<List<object>> AsBroadcast () => this.AsBroadcast((int?)null);

        /// <summary>
        ///     Sends the message to one or more receivers and waits for zero, one, or more responses.
        /// </summary>
        /// <param name="expectedResults"> The number of expected responses or null if any number is accepted (collecting as long as the the timeout). </param>
        /// <param name="responseType"> The type of the expected responses. </param>
        /// <returns>
        ///     The task used to wait until the timeout for completing round-trips expired or the expected number of responses was received.
        ///     The tasks result is the list of received responses (can be empty).
        /// </returns>
        /// <remarks>
        ///     <note type="important">
        ///         <see cref="AsBroadcast(int?,Type)" /> does not throw <see cref="BusResponseTimeoutException" /> for not responding receivers.
        ///         Not responding receivers simply do not show up in the list of responses.
        ///     </note>
        ///     <para>
        ///         The collection of responses end after the used timeout or the expected number of responses was received (<paramref name="expectedResults" />).
        ///     </para>
        ///     <para>
        ///         <see cref="ResponseType" /> is ignored or overwritten by <paramref name="responseType" /> respectively.
        ///         If <paramref name="responseType" /> is null, any response is accepted.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="expectedResults" /> is below zero. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> One of the responses could not be casted to type <paramref name="responseType" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<List<object>> AsBroadcast (int? expectedResults, Type responseType)
        {
            if (expectedResults.HasValue)
            {
                if (expectedResults.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(expectedResults));
                }
            }

            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = responseType;
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Broadcast;
                this.ExpectedResults = expectedResults;
                task = this.Bus.Enqueue(this);
            }

            Task<List<object>> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                List<object> list = ((IEnumerable)this.Result).ToList();
                list.ForEach(this.VerifyResponse);
                return list;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <inheritdoc cref="AsBroadcast(int?,Type)" />
        public Task<List<object>> AsBroadcast (Type responseType) => this.AsBroadcast(null, responseType);

        /// <summary>
        ///     Sends the message to one or more receivers and waits for zero, one, or more responses.
        /// </summary>
        /// <typeparam name="TResponse"> The type of the expected responses. </typeparam>
        /// <param name="expectedResults"> The number of expected responses or null if any number is accepted (collecting as long as the the timeout). </param>
        /// <returns>
        ///     The task used to wait until the timeout for completing round-trips expired or the expected number of responses was received.
        ///     The tasks result is the list of received responses (can be empty).
        /// </returns>
        /// <remarks>
        ///     <note type="important">
        ///         <see cref="AsBroadcast{TResponse}(int?)" /> does not throw <see cref="BusResponseTimeoutException" /> for not responding receivers.
        ///         Not responding receivers simply do not show up in the list of responses.
        ///     </note>
        ///     <para>
        ///         The collection of responses end after the used timeout or the expected number of responses was received (<paramref name="expectedResults" />).
        ///     </para>
        ///     <para>
        ///         <see cref="ResponseType" /> is ignored or overwritten by <typeparamref name="TResponse" /> respectively.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="expectedResults" /> is below zero. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> One of the responses could not be casted to type <typeparamref name="TResponse" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<List<TResponse>> AsBroadcast <TResponse> (int? expectedResults)
        {
            if (expectedResults.HasValue)
            {
                if (expectedResults.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(expectedResults));
                }
            }

            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = typeof(TResponse);
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Broadcast;
                this.ExpectedResults = expectedResults;
                task = this.Bus.Enqueue(this);
            }

            Task<List<TResponse>> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                List<object> list = ((IEnumerable)this.Result).ToList();
                list.ForEach(this.VerifyResponse);
                return list.Cast<TResponse>();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <inheritdoc cref="AsBroadcast{TResponse}(int?)" />
        public Task<List<TResponse>> AsBroadcast <TResponse> () => this.AsBroadcast<TResponse>(null);

        /// <summary>
        ///     Sends the message to an undefined amount of receivers and expects no responses.
        /// </summary>
        /// <returns>
        ///     The task used to wait until the message has been sent.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Sending messages using <see cref="AsFireAndForget" /> is truly fire-and-forget, meaning:
        ///         No cancellation token is observed as the send operation finishes immediately.
        ///         No timeout is observed and thus no <see cref="BusResponseTimeoutException" /> is thrown.
        ///         Exceptions are not forwarded by any receiver (causing the receivers to fail on exceptions).
        ///         No responses are expected.
        ///         No exception handling is performed (after the message has been sent).
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        public Task AsFireAndForget ()
        {
            lock (this.SyncRoot)
            {
                this.CancellationToken = null;
                this.Timeout = null;
                this.ExceptionForwarding = false;
                this.ResponseType = null;

                this.VerifyNotStarted();
                this.IsProcessed = true;
                this.OperationType = SendOperationType.FireAndForget;
                this.ExpectedResults = 0;

                return this.Bus.Enqueue(this);
            }
        }

        /// <summary>
        ///     Sends the message to a single receiver and waits for the first response.
        /// </summary>
        /// <returns>
        ///     The task used to wait until the round-trip completed.
        ///     The tasks result is the received response (which can be null).
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The response type is defined by <see cref="ResponseType" />.
        ///         If <see cref="ResponseType" /> is null, any response is accepted.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusResponseTimeoutException"> The intended receiver did not respond within the used timeout. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> The response could not be casted to type <see cref="ResponseType" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<object> AsSingle ()
        {
            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Single;
                this.ExpectedResults = 1;
                task = this.Bus.Enqueue(this);
            }

            Task<object> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                this.VerifyResponse(this.Result);
                return this.Result;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <summary>
        ///     Sends the message to a single receiver and waits for the first response.
        /// </summary>
        /// <param name="responseType"> The type of the expected response. </param>
        /// <returns>
        ///     The task used to wait until the round-trip completed.
        ///     The tasks result is the received response (which can be null).
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="ResponseType" /> is ignored or overwritten by <paramref name="responseType" /> respectively.
        ///         If <paramref name="responseType" /> is null, any response is accepted.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusResponseTimeoutException"> The intended receiver did not respond within the used timeout. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> The response could not be casted to type <paramref name="responseType" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<object> AsSingle (Type responseType)
        {
            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = responseType;
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Single;
                this.ExpectedResults = 1;
                task = this.Bus.Enqueue(this);
            }

            Task<object> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                this.VerifyResponse(this.Result);
                return this.Result;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <summary>
        ///     Sends the message to a single receiver and waits for the first response.
        /// </summary>
        /// <typeparam name="TResponse"> The type of the expected response. </typeparam>
        /// <returns>
        ///     The task used to wait until the round-trip completed.
        ///     The tasks result is the received response (which can be null).
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="ResponseType" /> is ignored or overwritten by <typeparamref name="TResponse" /> respectively.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The message is already being processed or the bus is not started. </exception>
        /// <exception cref="BusProcessingPipelineException"> The bus processing pipeline encountered an exception. </exception>
        /// <exception cref="BusResponseTimeoutException"> The intended receiver did not respond within the used timeout. </exception>
        /// <exception cref="BusConnectionBrokenException"> A used connection to a remote bus is broken. </exception>
        /// <exception cref="BusMessageProcessingException"> An exception which was thrown by the message receiver was forwarded to this sender. </exception>
        /// <exception cref="InvalidCastException"> The response could not be casted to type <typeparamref name="TResponse" />. </exception>
        /// TODO: Use TaskScheduler optionally provided by dispatcher
        public Task<TResponse> AsSingle <TResponse> ()
        {
            Task<object> task;
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = typeof(TResponse);
                this.IsProcessed = true;
                this.OperationType = SendOperationType.Single;
                this.ExpectedResults = 1;
                task = this.Bus.Enqueue(this);
            }

            Task<TResponse> resultTask = task.ContinueWith(t =>
            {
                this.Result = t.Result;
                this.VerifyResponse(this.Result);
                return (TResponse)this.Result;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);

            return resultTask;
        }

        /// <summary>
        ///     Sets the address the message is sent to.
        /// </summary>
        /// <param name="address"> The address the message is sent to or null if no address is used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="address" /> is an empty string. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation ToAddress (string address)
        {
            if (address != null)
            {
                if (address.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(address));
                }
            }

            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Address = address;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to be sent globally.
        /// </summary>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation ToGlobal ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Global = true;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to be sent locally or globally.
        /// </summary>
        /// <param name="sendGlobally"> Specifies whether the message should be sent globally (true), locally (false), or the default value from the associated bus should be used (null). </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation ToGlobal (bool? sendGlobally)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Global = sendGlobally;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to be sent locally.
        /// </summary>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation ToLocal ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Global = false;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to be sent locally or globally.
        /// </summary>
        /// <param name="sendLocally"> Specifies whether the message should be sent globally (false), locally (true), or the default value from the associated bus should be used (null). </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation ToLocal (bool? sendLocally)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Global = sendLocally.HasValue ? !sendLocally.Value : (bool?)null;
                return this;
            }
        }

        /// <summary>
        ///     Sets the cancellation token which can be used to cancel the wait for responses or the collection of responses.
        /// </summary>
        /// <param name="cancellationToken"> The cancellation token or null if cancellation is not used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithCancellation (CancellationToken? cancellationToken)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.CancellationToken = cancellationToken;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to forward exceptions from the receiver back to the sender.
        /// </summary>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithExceptionForwarding ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionForwarding = true;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to use or not use forward exceptions from the receiver back to the sender.
        /// </summary>
        /// <param name="forwardExceptions"> Specifies whether the message should forward exceptions from the receiver back to the sender (true), not (false), or the default value from the associated bus should be used (null). </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithExceptionForwarding (bool? forwardExceptions)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionForwarding = forwardExceptions;
                return this;
            }
        }

        /// <summary>
        ///     Sets the used exception handler.
        /// </summary>
        /// <param name="handler"> The used exception handler or null if no specific exception handler is used by this send operation where the default value of the associated bus is used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithExceptionHandler (SendExceptionHandler handler)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionHandler = handler;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to ignore broken connections.
        /// </summary>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithIgnoredBrokenConnections ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IgnoreBrokenConnections = true;
                return this;
            }
        }

        /// <summary>
        ///     Sets the message to ignore broken connections or not.
        /// </summary>
        /// <param name="ignore"> Specifies whether the message ignores broken connections (true), not (false), or the default value from the associated bus should be used (null). </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithIgnoredBrokenConnections (bool? ignore)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IgnoreBrokenConnections = ignore;
                return this;
            }
        }

        /// <summary>
        ///     Sets the payload of the message.
        /// </summary>
        /// <param name="payload"> The payload of the message or null if no payload is used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithPayload (object payload)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Payload = payload;
                return this;
            }
        }

        /// <summary>
        ///     Sets the expected response type.
        /// </summary>
        /// <param name="responseType"> The expected response type or null if no response is expected. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithResponse (Type responseType)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = responseType;
                return this;
            }
        }

        /// <summary>
        ///     Sets the expected response type.
        /// </summary>
        /// <typeparam name="TResponse"> The expected response type. </typeparam>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithResponse <TResponse> ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = typeof(TResponse);
                return this;
            }
        }

        /// <summary>
        ///     Sets the timeout for the message (waiting for or collecting responses).
        /// </summary>
        /// <param name="timeout"> The timeout or null if no timeout is used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithTimeout (TimeSpan? timeout)
        {
            if (timeout.HasValue)
            {
                if (timeout.Value.IsNegative())
                {
                    throw new ArgumentOutOfRangeException(nameof(timeout));
                }
            }

            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Timeout = timeout;
                return this;
            }
        }

        /// <summary>
        ///     Sets the timeout for the message (waiting for or collecting responses).
        /// </summary>
        /// <param name="milliseconds"> The timeout in milliseconds or null if no timeout is used. </param>
        /// <returns>
        ///     The send operation to continue configuration of the message.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        /// <exception cref="InvalidOperationException"> The message is already being processed. </exception>
        public SendOperation WithTimeout (int? milliseconds)
        {
            if (milliseconds.HasValue)
            {
                if (milliseconds < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(milliseconds));
                }
            }

            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.Timeout = milliseconds.HasValue ? TimeSpan.FromMilliseconds(milliseconds.Value) : (TimeSpan?)null;
                return this;
            }
        }

        private void VerifyNotStarted ()
        {
            if (this.IsProcessed)
            {
                throw new InvalidOperationException("The message is already being processed.");
            }
        }

        private void VerifyResponse (object response)
        {
            if (this.ResponseType == null)
            {
                return;
            }

            if (this.ResponseType.IsClass && (response == null))
            {
                return;
            }

            if ((!this.ResponseType.IsClass) && (response == null))
            {
                throw new InvalidCastException("Cannot use [null] as response for expected response type " + this.ResponseType.Name);
            }

            if (!this.ResponseType.IsAssignableFrom(response.GetType()))
            {
                throw new InvalidCastException("Cannot use " + response.GetType().Name + " as response for expected response type " + this.ResponseType.Name);
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("OperationType=");
            sb.Append(this.OperationType);

            sb.Append("; Address=");
            sb.Append(this.Address ?? "[null]");

            sb.Append("; Global=");
            sb.Append(this.Global?.ToString() ?? "[null]");

            sb.Append("; Payload=");
            sb.Append(this.Payload?.GetType().Name ?? "[null]");

            sb.Append("; ResponseType=");
            sb.Append(this.ResponseType?.Name ?? "[null]");

            sb.Append("; Result=");
            sb.Append(this.Result?.GetType().Name ?? "[null]");

            sb.Append("; Timeout=");
            sb.Append(this.Timeout.HasValue ? ((int)this.Timeout.Value.TotalMilliseconds).ToString() : "[null]");

            sb.Append("; ExpectedResults=");
            sb.Append(this.ExpectedResults?.ToString() ?? "[null]");

            sb.Append("; IgnoreBrokenConnections=");
            sb.Append(this.IgnoreBrokenConnections);

            sb.Append("; ExceptionForwarding=");
            sb.Append(this.ExceptionForwarding?.ToString() ?? "[null]");

            sb.Append("; ExceptionHandler=");
            sb.Append(this.ExceptionHandler?.GetFullName() ?? "[null]");

            sb.Append("; IsProcessed=");
            sb.Append(this.IsProcessed);

            return sb.ToString();
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }
}
