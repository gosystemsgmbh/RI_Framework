using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Bus
{
	/// <summary>
	/// Represents a receive registration.
	/// </summary>
	public sealed class ReceiveRegistration
	{
		internal ReceiveRegistration (LocalBus localBus)
		{
			this.LocalBus = localBus;

			this.Address = null;
			this.PayloadType = null;
			this.ResponseType = null;

			this.Started = false;
		}

		/// <summary>
		/// Gets the local bus this receive registration is associated with.
		/// </summary>
		/// <value>
		/// The local bus this receive registration is associated with.
		/// </value>
		public LocalBus LocalBus { get; }

		public string Address { get; private set; }

		public Type PayloadType { get; private set; }

		public Type ResponseType { get; private set; }

		public Func<string, object, object> Callback { get; private set; }

		private bool Started { get; set; }

		private void VerifyNotStarted ()
		{
			if (this.Started)
			{
				throw new InvalidOperationException("The reception is already being processed.");
			}
		}

		private void VerifyStarted()
		{
			if (!this.Started)
			{
				throw new InvalidOperationException("The reception is not being processed.");
			}
		}

		/// <summary>
		/// Sets the address to receive from.
		/// </summary>
		/// <param name="address">The address to receive from or null if no address is used.</param>
		/// <returns>
		/// The receive registration to continue configuration of the reception.
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="address"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiveRegistration FromAddress (string address)
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
		/// Sets the type of payload to receive.
		/// </summary>
		/// <param name="type">The payload type or null if no payload type is used.</param>
		/// <returns>
		/// The receive registration to continue configuration of the reception.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiveRegistration WithPayload (Type type)
		{
			this.VerifyNotStarted();
			this.PayloadType = type;
			return this;
		}

		/// <summary>
		/// Sets the type of response to use.
		/// </summary>
		/// <param name="type">The response type or null if no response is used.</param>
		/// <returns>
		/// The receive registration to continue configuration of the reception.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiveRegistration WithResponse (Type type)
		{
			this.VerifyNotStarted();
			this.ResponseType = type;
			return this;
		}

		/// <summary>
		/// Sets the type of payload to receive.
		/// </summary>
		/// <typeparam name="TPayload">The payload type.</typeparam>
		/// <returns>
		/// The receive registration to continue configuration of the reception.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiveRegistrationWithPayload<TPayload> WithPayload<TPayload> ()
		{
			this.VerifyNotStarted();
			this.PayloadType = typeof(TPayload);
			return new ReceiveRegistrationWithPayload<TPayload>(this);
		}

		/// <summary>
		/// Sets the type of response to use.
		/// </summary>
		/// <typeparam name="TResponse">The response type.</typeparam>
		/// <returns>
		/// The receive registration to continue configuration of the reception.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is already being processed.</exception>
		public ReceiveRegistrationWithResponse<TResponse> WithResponse<TResponse> ()
		{
			this.VerifyNotStarted();
			this.ResponseType = typeof(TResponse);
			return new ReceiveRegistrationWithResponse<TResponse>(this);
		}

		/// <summary>
		/// Starts reception by using a callback.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <returns>
		/// The receive registration for later stopping reception.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The reception is already being processed or the local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		public ReceiveRegistration By (Func<string, object, object> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.VerifyNotStarted();
			this.Started = true;
			this.Callback = callback;
			this.LocalBus.Register(this);
			return this;
		}

		/// <summary>
		/// Stops the reception of this receiver registration and continues with reconfiguration and restarting.
		/// </summary>
		/// <returns>
		/// The receive registration to continue configuration of the reception and restart.
		/// </returns>
		/// <exception cref="InvalidOperationException">The reception is not being processed.</exception>
		public ReceiveRegistration StopThenReceive ()
		{
			this.Stop();
			return this;
		}

		/// <summary>
		/// Stops the reception of this receiver registration.
		/// </summary>
		/// <exception cref="InvalidOperationException">The reception is not being processed.</exception>
		public void Stop ()
		{
			this.VerifyStarted();
			this.LocalBus.Unregister(this);
			this.Started = false;
		}
	}

	/// <inheritdoc cref="ReceiveRegistration"/>
	/// <typeparam name="TPayload">The type of the payload.</typeparam>
	public sealed class ReceiveRegistrationWithPayload<TPayload>
	{
		internal ReceiveRegistrationWithPayload (ReceiveRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiveRegistration Origin { get; }

		/// <inheritdoc cref="ReceiveRegistration.FromAddress"/>
		public ReceiveRegistrationWithPayload<TPayload> FromAddress (string address)
		{
			this.Origin.FromAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload"/>
		public ReceiveRegistration WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this.Origin;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse"/>
		public ReceiveRegistrationWithPayload<TPayload> WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload{TNewPayload}"/>
		public ReceiveRegistrationWithPayload<TNewPayload> WithPayload<TNewPayload>()
		{
			return this.Origin.WithPayload<TNewPayload>();
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse{TResponse}"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse> WithResponse<TResponse>()
		{
			this.Origin.WithResponse(typeof(TResponse));
			return new ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiveRegistration.By(Func{string,object,object})"/>
		public ReceiveRegistrationWithPayload<TPayload> By (Action<string, TPayload> callback)
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

		/// <inheritdoc cref="ReceiveRegistration.StopThenReceive"/>
		public ReceiveRegistrationWithPayload<TPayload> StopThenReceive ()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.Stop"/>
		public void Stop ()
		{
			this.Origin.Stop();
		}
	}

	/// <inheritdoc cref="ReceiveRegistration"/>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	public sealed class ReceiveRegistrationWithResponse<TResponse>
	{
		internal ReceiveRegistrationWithResponse (ReceiveRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiveRegistration Origin { get; }

		/// <inheritdoc cref="ReceiveRegistration.FromAddress"/>
		public ReceiveRegistrationWithResponse<TResponse> FromAddress (string address)
		{
			this.Origin.FromAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload"/>
		public ReceiveRegistrationWithResponse<TResponse> WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse"/>
		public ReceiveRegistration WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this.Origin;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload{TPayload}"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse> WithPayload<TPayload>()
		{
			this.Origin.WithPayload(typeof(TPayload));
			return new ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse{TNewResponse}"/>
		public ReceiveRegistrationWithResponse<TNewResponse> WithResponse<TNewResponse>()
		{
			return this.Origin.WithResponse<TNewResponse>();
		}

		/// <inheritdoc cref="ReceiveRegistration.By(Func{string,object,object})"/>
		public ReceiveRegistrationWithResponse<TResponse> By (Func<string, TResponse> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.Origin.By((address, payload) => callback(address));
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.StopThenReceive"/>
		public ReceiveRegistrationWithResponse<TResponse> StopThenReceive()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.Stop"/>
		public void Stop()
		{
			this.Origin.Stop();
		}
	}

	/// <inheritdoc cref="ReceiveRegistration"/>
	/// <typeparam name="TPayload">The type of the payload.</typeparam>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	public sealed class ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse>
	{
		internal ReceiveRegistrationWithPayloadAndResponse(ReceiveRegistration origin)
		{
			this.Origin = origin;
		}

		private ReceiveRegistration Origin { get; }

		/// <inheritdoc cref="ReceiveRegistration.FromAddress"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse> FromAddress(string address)
		{
			this.Origin.FromAddress(address);
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload"/>
		public ReceiveRegistrationWithResponse<TResponse> WithPayload (Type type)
		{
			this.Origin.WithPayload(type);
			return this.Origin.WithResponse<TResponse>();
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse"/>
		public ReceiveRegistrationWithPayload<TPayload> WithResponse (Type type)
		{
			this.Origin.WithResponse(type);
			return this.Origin.WithPayload<TPayload>();
		}

		/// <inheritdoc cref="ReceiveRegistration.WithPayload{TNewPayload}"/>
		public ReceiveRegistrationWithPayloadAndResponse<TNewPayload, TResponse> WithPayload<TNewPayload>()
		{
			this.Origin.WithPayload(typeof(TNewPayload));
			return new ReceiveRegistrationWithPayloadAndResponse<TNewPayload, TResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiveRegistration.WithResponse{TNewResponse}"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TNewResponse> WithResponse<TNewResponse>()
		{
			this.Origin.WithResponse(typeof(TNewResponse));
			return new ReceiveRegistrationWithPayloadAndResponse<TPayload, TNewResponse>(this.Origin);
		}

		/// <inheritdoc cref="ReceiveRegistration.By(Func{string,object,object})"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse> By(Func<string, TPayload, TResponse> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.Origin.By((address, payload) => callback(address, (TPayload)payload));
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.StopThenReceive"/>
		public ReceiveRegistrationWithPayloadAndResponse<TPayload, TResponse> StopThenReceive()
		{
			this.Origin.StopThenReceive();
			return this;
		}

		/// <inheritdoc cref="ReceiveRegistration.Stop"/>
		public void Stop()
		{
			this.Origin.Stop();
		}
	}
}
