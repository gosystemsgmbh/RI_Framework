using System;

using RI.Framework.Bus.Exceptions;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus
{
	/// <summary>
	/// Represents a receiver registration.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public sealed class ReceiverRegistration : ISynchronizable
	{
		internal ReceiverRegistration (IBus bus)
		{
			this.SyncRoot = new object();
			this.Bus = bus;

			this.Address = null;
			this.PayloadType = null;
			this.ResponseType = null;

			this.Started = false;
		}

		/// <summary>
		/// Gets the bus this receiver registration is associated with.
		/// </summary>
		/// <value>
		/// The bus this receiver registration is associated with.
		/// </value>
		public IBus Bus { get; }

		/// <summary>
		/// Gets the address this receiver listens to.
		/// </summary>
		/// <value>
		/// The address this receiver listens to or null if no address is used.
		/// </value>
		public string Address { get; private set; }

		/// <summary>
		/// Gets the payload type this receiver listens to.
		/// </summary>
		/// <value>
		/// The payload type this receiver listens to or null if no payload is used.
		/// </value>
		public Type PayloadType { get; private set; }

		/// <summary>
		/// Gets the response type this receiver produces.
		/// </summary>
		/// <value>
		/// The response type this receiver produces or null if no response is used.
		/// </value>
		public Type ResponseType { get; private set; }

		/// <summary>
		/// Gets the callback which is called upon message reception.
		/// </summary>
		/// <value>
		/// The callback which is called upon message reception.
		/// </value>
		public Func<string, object, object> Callback { get; private set; }

		private bool Started { get; set; }

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		private void VerifyNotStarted ()
		{
			if (this.Started)
			{
				throw new InvalidOperationException("The reception is already being processed.");
			}
		}

		/// <summary>
		/// Sets the address this receiver listens to.
		/// </summary>
		/// <param name="address">The address this receiver listens to or null if no address is used.</param>
		/// <returns>
		/// The receiver registration to continue configuration of the receiver.
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="address"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
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
		/// Sets the payload type this receiver listens to.
		/// </summary>
		/// <param name="type">The payload type this receiver listens to or null if no payload is used.</param>
		/// <returns>
		/// The receiver registration to continue configuration of the receiver.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
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
		/// Sets the response type this receiver produces.
		/// </summary>
		/// <param name="type">The response type this receiver produces or null if no response is used.</param>
		/// <returns>
		/// The receiver registration to continue configuration of the receiver.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
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
		/// Sets the payload type this receiver listens to.
		/// </summary>
		/// <typeparam name="TPayload">The payload type this receiver listens to.</typeparam>
		/// <returns>
		/// The receiver registration to continue configuration of the receiver.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiverRegistrationWithPayload<TPayload> WithPayload<TPayload> ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();
				this.PayloadType = typeof(TPayload);
				return new ReceiverRegistrationWithPayload<TPayload>(this);
			}
		}

		/// <summary>
		/// Sets the response type this receiver produces.
		/// </summary>
		/// <typeparam name="TResponse">The response type this receiver produces.</typeparam>
		/// <returns>
		/// The receiver registration to continue configuration of the receiver.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiverRegistrationWithResponse<TResponse> WithResponse<TResponse> ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();
				this.ResponseType = typeof(TResponse);
				return new ReceiverRegistrationWithResponse<TResponse>(this);
			}
		}

		/// <summary>
		/// Starts reception by a specified callback.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <returns>
		/// The receiver registration.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The reception is already being processed or the bus is stopped.</exception>
		/// <exception cref="LocalBusException">The bus processing pipeline encountered an exception.</exception>
		public ReceiverRegistration By (Func<string, object, object> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();
				this.Started = true;
				this.Callback = callback;
				this.Bus.Register(this);
				return this;
			}
		}

		/// <summary>
		/// Stops the reception of this receiver registration and continues with reconfiguration and restarting.
		/// </summary>
		/// <returns>
		/// The receive registration to continue configuration and restart.
		/// </returns>
		public ReceiverRegistration StopThenReceive ()
		{
			lock (this.SyncRoot)
			{
				this.Stop();
				return this;
			}
		}

		/// <summary>
		/// Stops the reception of this receiver registration.
		/// </summary>
		public void Stop ()
		{
			lock (this.SyncRoot)
			{
				this.Bus.Unregister(this);
				this.Started = false;
			}
		}
	}

	/// <inheritdoc cref="ReceiverRegistration"/>
	/// <typeparam name="TPayload">The type of the payload.</typeparam>
	public sealed class ReceiverRegistrationWithPayload<TPayload>
	{
		internal ReceiverRegistrationWithPayload (ReceiverRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiverRegistration Origin { get; }

		/// <summary>
		///     Implicitly converts a <see cref="ReceiverRegistrationWithPayload{TPayload}" /> to an <see cref="ReceiverRegistration"/>.
		/// </summary>
		/// <param name="value"> The <see cref="ReceiverRegistrationWithPayload{TPayload}" /> to convert. </param>
		/// <returns>
		///     The <see cref="ReceiverRegistration"/>.
		/// </returns>
		public static implicit operator ReceiverRegistration(ReceiverRegistrationWithPayload<TPayload> value)
		{
			return value.Origin;
		}

		/// <inheritdoc cref="ReceiverRegistration.AsAddress"/>
		public ReceiverRegistrationWithPayload<TPayload> AsAddress (string address)
		{
			this.Origin.AsAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload"/>
		public ReceiverRegistration WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this.Origin;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse"/>
		public ReceiverRegistrationWithPayload<TPayload> WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload{TNewPayload}"/>
		public ReceiverRegistrationWithPayload<TNewPayload> WithPayload<TNewPayload>()
		{
			return this.Origin.WithPayload<TNewPayload>();
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse{TResponse}"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithResponse<TResponse>()
		{
			this.Origin.WithResponse(typeof(TResponse));
			return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,object})"/>
		public ReceiverRegistrationWithPayload<TPayload> By (Action<string, TPayload> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.Origin.By((address, payload) =>
			{
				callback(address, (TPayload)payload);
				return null;
			});
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.StopThenReceive"/>
		public ReceiverRegistrationWithPayload<TPayload> StopThenReceive ()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.Stop"/>
		public void Stop ()
		{
			this.Origin.Stop();
		}
	}

	/// <inheritdoc cref="ReceiverRegistration"/>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	public sealed class ReceiverRegistrationWithResponse<TResponse>
	{
		internal ReceiverRegistrationWithResponse (ReceiverRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiverRegistration Origin { get; }

		/// <summary>
		///     Implicitly converts a <see cref="ReceiverRegistrationWithResponse{TResponse}" /> to an <see cref="ReceiverRegistration"/>.
		/// </summary>
		/// <param name="value"> The <see cref="ReceiverRegistrationWithResponse{TResponse}" /> to convert. </param>
		/// <returns>
		///     The <see cref="ReceiverRegistration"/>.
		/// </returns>
		public static implicit operator ReceiverRegistration(ReceiverRegistrationWithResponse<TResponse> value)
		{
			return value.Origin;
		}

		/// <inheritdoc cref="ReceiverRegistration.AsAddress"/>
		public ReceiverRegistrationWithResponse<TResponse> AsAddress (string address)
		{
			this.Origin.AsAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload"/>
		public ReceiverRegistrationWithResponse<TResponse> WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse"/>
		public ReceiverRegistration WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this.Origin;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload{TPayload}"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> WithPayload<TPayload>()
		{
			this.Origin.WithPayload(typeof(TPayload));
			return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse{TNewResponse}"/>
		public ReceiverRegistrationWithResponse<TNewResponse> WithResponse<TNewResponse>()
		{
			return this.Origin.WithResponse<TNewResponse>();
		}

		/// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,object})"/>
		public ReceiverRegistrationWithResponse<TResponse> By (Func<string, TResponse> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.Origin.By((address, payload) => callback(address));
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.StopThenReceive"/>
		public ReceiverRegistrationWithResponse<TResponse> StopThenReceive()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.Stop"/>
		public void Stop()
		{
			this.Origin.Stop();
		}
	}

	/// <inheritdoc cref="ReceiverRegistration"/>
	/// <typeparam name="TPayload">The type of the payload.</typeparam>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	public sealed class ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse>
	{
		internal ReceiverRegistrationWithPayloadAndResponse(ReceiverRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiverRegistration Origin { get; }

		/// <summary>
		///     Implicitly converts a <see cref="ReceiverRegistrationWithPayloadAndResponse{TPayload,TResponse}" /> to an <see cref="ReceiverRegistration"/>.
		/// </summary>
		/// <param name="value"> The <see cref="ReceiverRegistrationWithPayloadAndResponse{TPayload,TResponse}" /> to convert. </param>
		/// <returns>
		///     The <see cref="ReceiverRegistration"/>.
		/// </returns>
		public static implicit operator ReceiverRegistration(ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> value)
		{
			return value.Origin;
		}

		/// <inheritdoc cref="ReceiverRegistration.AsAddress"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> AsAddress(string address)
		{
			this.Origin.AsAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload"/>
		public ReceiverRegistrationWithResponse<TResponse> WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this.Origin.WithResponse<TResponse>();
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse"/>
		public ReceiverRegistrationWithPayload<TPayload> WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this.Origin.WithPayload<TPayload>();
		}

		/// <inheritdoc cref="ReceiverRegistration.WithPayload{TNewPayload}"/>
		public ReceiverRegistrationWithPayloadAndResponse<TNewPayload, TResponse> WithPayload<TNewPayload>()
		{
			this.Origin.WithPayload(typeof(TNewPayload));
			return new ReceiverRegistrationWithPayloadAndResponse<TNewPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiverRegistration.WithResponse{TNewResponse}"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TNewResponse> WithResponse<TNewResponse>()
		{
			this.Origin.WithResponse(typeof(TNewResponse));
			return new ReceiverRegistrationWithPayloadAndResponse<TPayload, TNewResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiverRegistration.By(Func{string,object,object})"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> By(Func<string, TPayload, TResponse> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.Origin.By((address, payload) => callback(address, (TPayload)payload));
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.StopThenReceive"/>
		public ReceiverRegistrationWithPayloadAndResponse<TPayload, TResponse> StopThenReceive()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiverRegistration.Stop"/>
		public void Stop()
		{
			this.Origin.Stop();
		}
	}
}
