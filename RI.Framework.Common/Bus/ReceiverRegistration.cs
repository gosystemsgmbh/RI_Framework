﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections.Concurrent;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Represents a receiver registration.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Use TaskScheduler optionally provided by dispatcher
    public sealed class ReceiverRegistration : ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ReceiverRegistration" />.
        /// </summary>
        /// <param name="bus"> The bus to be associated with this receiver registration. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> is null. </exception>
        public ReceiverRegistration (IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.SyncRoot = new object();
            this.Bus = bus;

            this.Callback = null;
            this.Address = null;
            this.PayloadType = null;
            this.ResponseType = null;
            this.ExceptionForwarding = null;
            this.ExceptionHandler = null;

            this.IncludeCompatiblePayloadTypes = false;

            this.IsProcessed = false;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the address this receiver listens to.
        /// </summary>
        /// <value>
        ///     The address this receiver listens to or null if no address is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="AsAddress" />.
        ///     </para>
        /// </remarks>
        public string Address { get; private set; }

        /// <summary>
        ///     Gets the bus this receiver registration is associated with.
        /// </summary>
        /// <value>
        ///     The bus this receiver registration is associated with.
        /// </value>
        public IBus Bus { get; }

        /// <summary>
        ///     Gets the callback which is called upon message reception.
        /// </summary>
        /// <value>
        ///     The callback which is called upon message reception.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         This value is set by <see cref="By" />.
        ///     </para>
        /// </remarks>
        public Func<string, object, Task<object>> Callback { get; private set; }

        /// <summary>
        ///     Gets or sets whether exception forwarding to the sender is used when the message is processed by the receiver.
        /// </summary>
        /// <value>
        ///     true if exception forwarding is used, false if not, null if not defined where the default value of the associated bus is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithExceptionForwarding()" />, <see cref="WithExceptionForwarding(bool?)"/>.
        ///     </para>
        /// </remarks>
        public bool? ExceptionForwarding { get; private set; }

        /// <summary>
        ///     Gets the exception handler which is called for unhandled exceptions within <see cref="Callback" />.
        /// </summary>
        /// <value>
        ///     The exception handler which is called for unhandled exceptions within <see cref="Callback" /> or null if no exception handler is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithExceptionHandler" />.
        ///     </para>
        /// </remarks>
        public ReceiverExceptionHandler ExceptionHandler { get; private set; }

        /// <summary>
        ///     Gets whether compatible payload types, which are convertible to <see cref="PayloadType" />, are also accepted (true) or only those payloads which are of exactly <see cref="PayloadType" /> (false).
        /// </summary>
        /// <value>
        ///     true if compatible payload types are accepted, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is false.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="IncludeCompatiblePayloadTypes" />.
        ///     </para>
        /// </remarks>
        public bool IncludeCompatiblePayloadTypes { get; private set; }

        /// <summary>
        ///     Gets whether this receiver registration is being processed.
        /// </summary>
        /// <value>
        ///     true if the receiver registration is being processed, false otherwise.
        /// </value>
        public bool IsProcessed { get; private set; }

        /// <summary>
        ///     Gets the payload type this receiver listens to.
        /// </summary>
        /// <value>
        ///     The payload type this receiver listens to or null if no payload is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithPayload" />, <see cref="WithPayload{TPayload}"/>.
        ///     </para>
        /// </remarks>
        public Type PayloadType { get; private set; }

        /// <summary>
        ///     Gets the response type this receiver produces.
        /// </summary>
        /// <value>
        ///     The response type this receiver produces or null if no response is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        ///     <para>
        ///         This value can be set by <see cref="WithResponse" />, <see cref="WithResponse{TResponse}"/>.
        ///     </para>
        /// </remarks>
        public Type ResponseType { get; private set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Sets the address this receiver listens to.
        /// </summary>
        /// <param name="address"> The address this receiver listens to or null if no address is used. </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="address" /> is an empty string. </exception>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration AsAddress (string address)
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
        ///     Starts reception by a specified callback.
        /// </summary>
        /// <param name="callback"> The callback. </param>
        /// <returns>
        ///     The receiver registration.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration By (Func<string, object, Task<object>> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IsProcessed = true;
                this.Callback = callback;
                this.Bus.Register(this);
                return this;
            }
        }

        /// <summary>
        ///     Sets that compatible payload types, which are convertible to <see cref="PayloadType" />, are also accepted and not only those payloads which are of exactly <see cref="PayloadType" />.
        /// </summary>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        public ReceiverRegistration WithCompatiblePayloads ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IncludeCompatiblePayloadTypes = true;
                return this;
            }
        }

        /// <summary>
        ///     Sets whether compatible payload types, which are convertible to <see cref="PayloadType" />, are also accepted or only those payloads which are of exactly <see cref="PayloadType" />.
        /// </summary>
        /// <param name="includeCompatiblePayloads"> Specifies whether compatible payload types are accepted. </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        public ReceiverRegistration WithCompatiblePayloads (bool includeCompatiblePayloads)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.IncludeCompatiblePayloadTypes = includeCompatiblePayloads;
                return this;
            }
        }

        /// <summary>
        ///     Stops the reception of this receiver registration.
        /// </summary>
        public void Stop ()
        {
            lock (this.SyncRoot)
            {
                this.Bus.Unregister(this);
                this.IsProcessed = false;
            }
        }

        /// <summary>
        ///     Stops the reception of this receiver registration and continues with reconfiguration and restarting.
        /// </summary>
        /// <returns>
        ///     The receive registration to continue configuration and restart.
        /// </returns>
        public ReceiverRegistration StopThenContinue ()
        {
            lock (this.SyncRoot)
            {
                this.Stop();
                return this;
            }
        }

        /// <summary>
        ///     Sets the receiver to forward exceptions from the receiver back to the sender.
        /// </summary>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration WithExceptionForwarding ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionForwarding = true;
                return this;
            }
        }

        /// <summary>
        ///     Sets the receiver to use or not use forward exceptions from the receiver back to the sender.
        /// </summary>
        /// <param name="forwardExceptions"> Specifes whether the message should forward exceptions from the receiver back to the sender (true) or not (false). </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration WithExceptionForwarding (bool? forwardExceptions)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionForwarding = forwardExceptions;
                return this;
            }
        }

        /// <summary>
        ///     Sets the exception handler this receiver uses.
        /// </summary>
        /// <param name="exceptionHandler"> The exception handler this receiver uses or null if no exception handler is used. </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration WithExceptionHandler (ReceiverExceptionHandler exceptionHandler)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ExceptionHandler = exceptionHandler;
                return this;
            }
        }

        /// <summary>
        ///     Sets the payload type this receiver listens to.
        /// </summary>
        /// <param name="type"> The payload type this receiver listens to or null if no payload is used. </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration WithPayload (Type type)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.PayloadType = type;
                return this;
            }
        }

        /// <summary>
        ///     Sets the payload type this receiver listens to.
        /// </summary>
        /// <typeparam name="TPayload"> The payload type this receiver listens to. </typeparam>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistrationWithPayload<TPayload> WithPayload <TPayload> ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.PayloadType = typeof(TPayload);
                return new ReceiverRegistrationWithPayload<TPayload>(this);
            }
        }

        /// <summary>
        ///     Sets the response type this receiver produces.
        /// </summary>
        /// <param name="type"> The response type this receiver produces or null if no response is used. </param>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistration WithResponse (Type type)
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = type;
                return this;
            }
        }

        /// <summary>
        ///     Sets the response type this receiver produces.
        /// </summary>
        /// <typeparam name="TResponse"> The response type this receiver produces. </typeparam>
        /// <returns>
        ///     The receiver registration to continue configuration of the receiver.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistrationWithResponse<TResponse> WithResponse <TResponse> ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotStarted();
                this.ResponseType = typeof(TResponse);
                return new ReceiverRegistrationWithResponse<TResponse>(this);
            }
        }

        private void VerifyNotStarted ()
        {
            if (this.IsProcessed)
            {
                throw new InvalidOperationException("The reception is already being processed.");
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Address=");
            sb.Append(this.Address ?? "[null]");

            sb.Append("; PayloadType=");
            sb.Append(this.PayloadType?.Name ?? "[null]");

            sb.Append("; IncludeCompatiblePayloadTypes=");
            sb.Append(this.IncludeCompatiblePayloadTypes);

            sb.Append("; ResponseType=");
            sb.Append(this.ResponseType?.Name ?? "[null]");

            sb.Append("; Callback=");
            sb.Append(this.Callback?.GetFullName() ?? "[null]");

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

    /// <inheritdoc cref="ReceiverRegistration" />
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public sealed class ReceiverRegistrationWithPayload <TPayload> : ISynchronizable
    {
        #region Static Methods

        /// <summary>
        ///     Implicitly converts a <see cref="ReceiverRegistrationWithPayload{TPayload}" /> to an <see cref="ReceiverRegistration" />.
        /// </summary>
        /// <param name="value"> The <see cref="ReceiverRegistrationWithPayload{TPayload}" /> to convert. </param>
        /// <returns>
        ///     The <see cref="ReceiverRegistration" />.
        /// </returns>
        public static implicit operator ReceiverRegistration (ReceiverRegistrationWithPayload<TPayload> value)
        {
            return value.Origin;
        }

        #endregion




        #region Instance Constructor/Destructor

        internal ReceiverRegistrationWithPayload (ReceiverRegistration origin)
        {
            this.Origin = origin;
        }

        #endregion




        #region Instance Properties/Indexer

        private ReceiverRegistration Origin { get; }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ReceiverRegistration.AsAddress" />
        public ReceiverRegistrationWithPayload<TPayload> AsAddress (string address)
        {
            this.Origin.AsAddress(address);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,Task{object}})" />
        public ReceiverRegistrationWithPayload<TPayload> By (Func<string, TPayload, Task> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Origin.By((address, payload) => callback(address, (TPayload)payload).ContinueWith(_ => (object)null, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current));
            return this;
        }

        /// <summary>
        ///     Starts reception by a specified queue.
        /// </summary>
        /// <param name="queue"> The queue. </param>
        /// <returns>
        ///     The receiver registration.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="queue" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistrationWithPayload<TPayload> By (RequestResponseQueue<TPayload, object> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            this.By((address, payload) => queue.EnqueueAsync(payload));
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads()" />
        public ReceiverRegistrationWithPayload<TPayload> WithCompatiblePayloads ()
        {
            this.Origin.WithCompatiblePayloads();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads(bool)" />
        public ReceiverRegistrationWithPayload<TPayload> WithCompatiblePayloads (bool includeCompatiblePayloads)
        {
            this.Origin.WithCompatiblePayloads(includeCompatiblePayloads);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.Stop" />
        public void Stop ()
        {
            this.Origin.Stop();
        }

        /// <inheritdoc cref="ReceiverRegistration.StopThenContinue" />
        public ReceiverRegistrationWithPayload<TPayload> StopThenContinue ()
        {
            this.Origin.StopThenContinue();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding()" />
        public ReceiverRegistrationWithPayload<TPayload> WithExceptionForwarding ()
        {
            this.Origin.WithExceptionForwarding();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding(bool?)" />
        public ReceiverRegistrationWithPayload<TPayload> WithExceptionForwarding (bool? forwardExceptions)
        {
            this.Origin.WithExceptionForwarding(forwardExceptions);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionHandler" />
        public ReceiverRegistrationWithPayload<TPayload> WithExceptionHandler (ReceiverExceptionHandler exceptionHandler)
        {
            this.Origin.WithExceptionHandler(exceptionHandler);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload" />
        public ReceiverRegistration WithPayload (Type type)
        {
            this.Origin.WithPayload(type);
            return this.Origin;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload{TNewPayload}" />
        public ReceiverRegistrationWithPayload<TNewPayload> WithPayload <TNewPayload> ()
        {
            return this.Origin.WithPayload<TNewPayload>();
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse" />
        public ReceiverRegistrationWithPayload<TPayload> WithResponse (Type type)
        {
            this.Origin.WithResponse(type);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse{TResponse}" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithResponse <TResponse> ()
        {
            this.Origin.WithResponse(typeof(TResponse));
            return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString () => this.Origin.ToString();

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => ((ISynchronizable)this.Origin).IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => this.Origin.SyncRoot;

        #endregion
    }

    /// <inheritdoc cref="ReceiverRegistration" />
    /// <typeparam name="TResponse"> The type of the response. </typeparam>
    public sealed class ReceiverRegistrationWithResponse <TResponse> : ISynchronizable
    {
        #region Static Methods

        /// <summary>
        ///     Implicitly converts a <see cref="ReceiverRegistrationWithResponse{TResponse}" /> to an <see cref="ReceiverRegistration" />.
        /// </summary>
        /// <param name="value"> The <see cref="ReceiverRegistrationWithResponse{TResponse}" /> to convert. </param>
        /// <returns>
        ///     The <see cref="ReceiverRegistration" />.
        /// </returns>
        public static implicit operator ReceiverRegistration (ReceiverRegistrationWithResponse<TResponse> value)
        {
            return value.Origin;
        }

        #endregion




        #region Instance Constructor/Destructor

        internal ReceiverRegistrationWithResponse (ReceiverRegistration origin)
        {
            this.Origin = origin;
        }

        #endregion




        #region Instance Properties/Indexer

        private ReceiverRegistration Origin { get; }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ReceiverRegistration.AsAddress" />
        public ReceiverRegistrationWithResponse<TResponse> AsAddress (string address)
        {
            this.Origin.AsAddress(address);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,Task{object}})" />
        public ReceiverRegistrationWithResponse<TResponse> By (Func<string, Task<TResponse>> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Origin.By((address, payload) => callback(address).ContinueWith(t => (object)t.Result, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current));
            return this;
        }

        /// <summary>
        ///     Starts reception by a specified queue.
        /// </summary>
        /// <param name="queue"> The queue. </param>
        /// <returns>
        ///     The receiver registration.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="queue" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistrationWithResponse<TResponse> By(RequestResponseQueue<object, TResponse> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            this.By((address) => queue.EnqueueAsync(null));
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads()" />
        public ReceiverRegistrationWithResponse<TResponse> WithCompatiblePayloads ()
        {
            this.Origin.WithCompatiblePayloads();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads(bool)" />
        public ReceiverRegistrationWithResponse<TResponse> WithCompatiblePayloads (bool includeCompatiblePayloads)
        {
            this.Origin.WithCompatiblePayloads(includeCompatiblePayloads);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.Stop" />
        public void Stop ()
        {
            this.Origin.Stop();
        }

        /// <inheritdoc cref="ReceiverRegistration.StopThenContinue" />
        public ReceiverRegistrationWithResponse<TResponse> StopThenContinue ()
        {
            this.Origin.StopThenContinue();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding()" />
        public ReceiverRegistrationWithResponse<TResponse> WithExceptionForwarding ()
        {
            this.Origin.WithExceptionForwarding();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding(bool?)" />
        public ReceiverRegistrationWithResponse<TResponse> WithExceptionForwarding (bool? forwardExceptions)
        {
            this.Origin.WithExceptionForwarding(forwardExceptions);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionHandler" />
        public ReceiverRegistrationWithResponse<TResponse> WithExceptionHandler (ReceiverExceptionHandler exceptionHandler)
        {
            this.Origin.WithExceptionHandler(exceptionHandler);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload" />
        public ReceiverRegistrationWithResponse<TResponse> WithPayload (Type type)
        {
            this.Origin.WithPayload(type);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload{TPayload}" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithPayload <TPayload> ()
        {
            this.Origin.WithPayload(typeof(TPayload));
            return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse" />
        public ReceiverRegistration WithResponse (Type type)
        {
            this.Origin.WithResponse(type);
            return this.Origin;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse{TNewResponse}" />
        public ReceiverRegistrationWithResponse<TNewResponse> WithResponse <TNewResponse> ()
        {
            return this.Origin.WithResponse<TNewResponse>();
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString() => this.Origin.ToString();

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => ((ISynchronizable)this.Origin).IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => this.Origin.SyncRoot;

        #endregion
    }

    /// <inheritdoc cref="ReceiverRegistration" />
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TResponse"> The type of the response. </typeparam>
    public sealed class ReceiverRegistrationWithPayloadAndResponse <TPayload, TResponse> : ISynchronizable
    {
        #region Static Methods

        /// <summary>
        ///     Implicitly converts a <see cref="ReceiverRegistrationWithPayloadAndResponse{TPayload,TResponse}" /> to an <see cref="ReceiverRegistration" />.
        /// </summary>
        /// <param name="value"> The <see cref="ReceiverRegistrationWithPayloadAndResponse{TPayload,TResponse}" /> to convert. </param>
        /// <returns>
        ///     The <see cref="ReceiverRegistration" />.
        /// </returns>
        public static implicit operator ReceiverRegistration (ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> value)
        {
            return value.Origin;
        }

        #endregion




        #region Instance Constructor/Destructor

        internal ReceiverRegistrationWithPayloadAndResponse (ReceiverRegistration origin)
        {
            this.Origin = origin;
        }

        #endregion




        #region Instance Properties/Indexer

        private ReceiverRegistration Origin { get; }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ReceiverRegistration.AsAddress" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> AsAddress (string address)
        {
            this.Origin.AsAddress(address);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,Task{object}})" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> By (Func<string, TPayload, Task<TResponse>> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Origin.By((address, payload) => callback(address, (TPayload)payload).ContinueWith(t => (object)t.Result, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current));
            return this;
        }

        /// <summary>
        ///     Starts reception by a specified queue.
        /// </summary>
        /// <param name="queue"> The queue. </param>
        /// <returns>
        ///     The receiver registration.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="queue" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The reception is already being processed. </exception>
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> By(RequestResponseQueue<TPayload, TResponse> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            this.By((address, payload) => queue.EnqueueAsync(payload));
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads()" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithCompatiblePayloads ()
        {
            this.Origin.WithCompatiblePayloads();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithCompatiblePayloads(bool)" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithCompatiblePayloads (bool includeCompatiblePayloads)
        {
            this.Origin.WithCompatiblePayloads(includeCompatiblePayloads);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.Stop" />
        public void Stop ()
        {
            this.Origin.Stop();
        }

        /// <inheritdoc cref="ReceiverRegistration.StopThenContinue" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> StopThenContinue ()
        {
            this.Origin.StopThenContinue();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding()" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithExceptionForwarding ()
        {
            this.Origin.WithExceptionForwarding();
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionForwarding(bool?)" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithExceptionForwarding (bool? forwardExceptions)
        {
            this.Origin.WithExceptionForwarding(forwardExceptions);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithExceptionHandler" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithExceptionHandler (ReceiverExceptionHandler exceptionHandler)
        {
            this.Origin.WithExceptionHandler(exceptionHandler);
            return this;
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload" />
        public ReceiverRegistrationWithResponse<TResponse> WithPayload (Type type)
        {
            this.Origin.WithPayload(type);
            return this.Origin.WithResponse<TResponse>();
        }

        /// <inheritdoc cref="ReceiverRegistration.WithPayload{TNewPayload}" />
        public ReceiverRegistrationWithPayloadAndResponse<TNewPayload, TResponse> WithPayload <TNewPayload> ()
        {
            this.Origin.WithPayload(typeof(TNewPayload));
            return new ReceiverRegistrationWithPayloadAndResponse<TNewPayload, TResponse>(this.Origin);
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse" />
        public ReceiverRegistrationWithPayload<TPayload> WithResponse (Type type)
        {
            this.Origin.WithResponse(type);
            return this.Origin.WithPayload<TPayload>();
        }

        /// <inheritdoc cref="ReceiverRegistration.WithResponse{TNewResponse}" />
        public ReceiverRegistrationWithPayloadAndResponse<TPayload, TNewResponse> WithResponse <TNewResponse> ()
        {
            this.Origin.WithResponse(typeof(TNewResponse));
            return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TNewResponse>(this.Origin);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString() => this.Origin.ToString();

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => ((ISynchronizable)this.Origin).IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => this.Origin.SyncRoot;

        #endregion
    }
}
