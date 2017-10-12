using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Bus
{
	/// <summary>
	/// Represents a send operation.
	/// </summary>
	public sealed class SendOperation
	{
		internal SendOperation (LocalBus localBus)
		{
			this.LocalBus = localBus;

			this.Address = null;
			this.Global = null;
			this.Payload = null;
			this.Timeout = null;
			this.CancellationToken = null;

			this.IsBroadcast = false;
			this.Started = false;

			this.Result = null;
		}

		/// <summary>
		/// Gets the local bus this send operation is associated with.
		/// </summary>
		/// <value>
		/// The local bus this send operation is associated with.
		/// </value>
		public LocalBus LocalBus { get; }

		public string Address { get; private set; }

		public bool? Global { get; private set; }

		public object Payload { get; private set; }

		public TimeSpan? Timeout { get; private set; }

		public CancellationToken? CancellationToken { get; private set; }

		public bool IsBroadcast { get; private set; }

		private bool Started { get; set; }

		private object Result { get; set; }

		private void VerifyNotStarted ()
		{
			if (this.Started)
			{
				throw new InvalidOperationException("The message is already being processed.");
			}
		}

		/// <summary>
		/// Sets the address the message is sent to.
		/// </summary>
		/// <param name="address">The address the message is sent to or null if no address is used.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="address"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation ToAddress (string address)
		{
			if (address != null)
			{
				if (address.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(address));
				}
			}

			this.VerifyNotStarted();
			this.Address = address;
			return this;
		}

		/// <summary>
		/// Sets the message to be sent only to receivers of this local message bus.
		/// </summary>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation ToLocal()
		{
			this.VerifyNotStarted();
			this.Global = false;
			return this;
		}

		/// <summary>
		/// Sets the message to be sent to receivers of this local message bus and all connected message busses.
		/// </summary>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation ToGlobal()
		{
			this.VerifyNotStarted();
			this.Global = true;
			return this;
		}

		/// <summary>
		/// Sets the message to be sent locally or globally.
		/// </summary>
		/// <param name="sendGlobally">Specifes whether the message should be sent globally.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation ToGlobal(bool sendGlobally)
		{
			this.VerifyNotStarted();
			this.Global = sendGlobally;
			return this;
		}

		/// <summary>
		/// Sets to use the default value whether to send the message globally.
		/// </summary>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation ToDefaultGlobal ()
		{
			this.VerifyNotStarted();
			this.Global = null;
			return this;
		}

		/// <summary>
		/// Sets the payload of the message.
		/// </summary>
		/// <param name="payload">The payload of the message or null if no payload is used.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation WithPayload (object payload)
		{
			this.VerifyNotStarted();
			this.Payload = payload;
			return this;
		}

		/// <summary>
		/// Sets the timeout after which a <see cref="ResponseTimeoutException"/> is thrown (<see cref="AsSingle"/>, <see cref="AsSingle{TResponse}"/>) or the collection of responses is finished (<see cref="AsBroadcast"/>, <see cref="AsBroadcast{TResponse}"/>).
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation WithTimeout (TimeSpan timeout)
		{
			if (timeout.IsNegative())
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			this.VerifyNotStarted();
			this.Timeout = timeout;
			return this;
		}

		/// <summary>
		/// Sets the timeout after which a <see cref="ResponseTimeoutException"/> is thrown (<see cref="AsSingle"/>, <see cref="AsSingle{TResponse}"/>) or the collection of responses is finished (<see cref="AsBroadcast"/>, <see cref="AsBroadcast{TResponse}"/>).
		/// </summary>
		/// <param name="milliseconds">The timeout in milliseconds.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation WithTimeout (int milliseconds) => this.WithTimeout(TimeSpan.FromMilliseconds(milliseconds));

		/// <summary>
		/// Sets to use the default timeout after which a <see cref="ResponseTimeoutException"/> is thrown (<see cref="AsSingle"/>, <see cref="AsSingle{TResponse}"/>) or the collection of responses is finished (<see cref="AsBroadcast"/>, <see cref="AsBroadcast{TResponse}"/>).
		/// </summary>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation WithDefaultTimeout()
		{
			this.VerifyNotStarted();
			this.Timeout = null;
			return this;
		}

		/// <summary>
		/// Sets cancellation token used to stop waiting for the round-trip or collection of responses.
		/// </summary>
		/// <param name="cancellationToken">The used cancellation token or null if cancellation is not used.</param>
		/// <returns>
		/// The send operation to continue configuration of the message.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed.</exception>
		public SendOperation WithCancellation(CancellationToken? cancellationToken)
		{
			this.VerifyNotStarted();
			this.CancellationToken = cancellationToken;
			return this;
		}

		/// <summary>
		/// Sends the message to a single receiver without a response.
		/// </summary>
		/// <returns>
		/// The task used to wait until the round-trip completed.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed or the local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		/// <exception cref="ResponseTimeoutException">The intended receiver did not respond within the specified timeout.</exception>
		/// <exception cref="ConnectionBrokenException">A used connection to a connected message bus is broken.</exception>
		public async Task AsSingle ()
		{
			this.VerifyNotStarted();
			this.Started = true;
			this.IsBroadcast = false;
			this.Result = await this.LocalBus.Enqueue(this);
		}

		/// <summary>
		/// Sends the message to a single receiver expecting a response.
		/// </summary>
		/// <typeparam name="TResponse">The type of the expected response.</typeparam>
		/// <returns>
		/// The task used to wait until the round-trip completed.
		/// The tasks result is the received response.
		/// </returns>
		/// <exception cref="InvalidOperationException">The message is already being processed or the local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		/// <exception cref="ResponseTimeoutException">The intended receiver did not respond within the specified timeout.</exception>
		/// <exception cref="ConnectionBrokenException">A used connection to a connected message bus is broken.</exception>
		/// <exception cref="InvalidCastException">The response could not be casted to type <typeparamref name="TResponse"/>.</exception>
		public async Task<TResponse> AsSingle <TResponse> ()
		{
			this.VerifyNotStarted();
			this.Started = true;
			this.IsBroadcast = false;
			this.Result = await this.LocalBus.Enqueue(this);
			return (TResponse)this.Result;
		}

		/// <summary>
		/// Broadcasts the message to multiple receivers without responses.
		/// </summary>
		/// <returns>
		/// The task used to wait until the timeout for completing round-trips expired.
		/// The tasks result is the number of receivers which acknowledged the message.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// <see cref="AsBroadcast"/> does not throw <see cref="ResponseTimeoutException"/> for not responding receivers.
		/// </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException">The message is already being processed or the local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		/// <exception cref="ConnectionBrokenException">A used connection to a connected message bus is broken.</exception>
		public async Task<int> AsBroadcast()
		{
			this.VerifyNotStarted();
			this.Started = true;
			this.IsBroadcast = true;
			this.Result = await this.LocalBus.Enqueue(this);
			return ((ICollection)this.Result).Count;
		}

		/// <summary>
		/// Broadcasts the message to multiple receivers expecting a response from each receiver.
		/// </summary>
		/// <typeparam name="TResponse">The type of the expected responses.</typeparam>
		/// <returns>
		/// The task used to wait until the timeout for completing round-trips expired.
		/// The tasks result is the list of responses.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// <see cref="AsBroadcast"/> does not throw <see cref="ResponseTimeoutException"/> for not responding receivers.
		/// </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException">The message is already being processed or the local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		/// <exception cref="ConnectionBrokenException">A used connection to a connected message bus is broken.</exception>
		/// <exception cref="InvalidCastException">The responses could not be casted to type <typeparamref name="TResponse"/>.</exception>
		public async Task<List<TResponse>> AsBroadcast <TResponse> ()
		{
			this.VerifyNotStarted();
			this.Started = true;
			this.IsBroadcast = true;
			this.Result = await this.LocalBus.Enqueue(this);
			return ((ICollection)this.Result).Cast<TResponse>().ToList();
		}
	}
}