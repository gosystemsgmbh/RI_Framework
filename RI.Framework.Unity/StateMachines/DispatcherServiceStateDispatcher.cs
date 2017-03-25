using System;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;




namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Implements a default state machine operation dispatcher suitable for most scenarios.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="DispatcherServiceStateDispatcher"/> internally uses a specified <see cref="IDispatcherService"/> or <see cref="ServiceLocator"/> to retrieve a <see cref="IDispatcherService"/>, depending on the used constructor..
	/// </para>
	/// <para>
	/// See <see cref="IStateDispatcher"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class DispatcherServiceStateDispatcher : IStateDispatcher
	{
		/// <summary>
		/// Gets the used dispatcher service.
		/// </summary>
		/// <value>
		/// The used dispatcher service.
		/// </value>
		public IDispatcherService DispatcherService { get; private set; }

		/// <summary>
		/// Creates a new instance of <see cref="DispatcherServiceStateDispatcher"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="DispatcherServiceStateDispatcher"/> instances created with this constructor use <see cref="ServiceLocator"/> to retrieve an instance of <see cref="IDispatcherService"/> at construction time.
		/// </para>
		/// <note type="important">
		/// For performance reasons, the <see cref="IDispatcherService"/> instance which is used is not dynamically retrieved from <see cref="ServiceLocator"/> for each dispatched operation.
		/// The <see cref="IDispatcherService"/> instance is only once retrieved when this constructor is run.
		/// </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"><see cref="ServiceLocator"/> could not retrieve an instance of <see cref="IDispatcherService"/>.</exception>
		public DispatcherServiceStateDispatcher ()
		{
			this.DispatcherService = ServiceLocator.GetInstance<IDispatcherService>();
			if (this.DispatcherService == null)
			{
				throw new InvalidOperationException("Could not retrieve an instance of " + nameof(IDispatcherService) + " for " + this.GetType().Name + ".");
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="DispatcherServiceStateDispatcher"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="DispatcherServiceStateDispatcher"/> instances created with this constructor use the <see cref="IDispatcherService"/> specified by <paramref name="dispatcherService"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dispatcherService"/> is null.</exception>
		public DispatcherServiceStateDispatcher (IDispatcherService dispatcherService)
		{
			if (dispatcherService == null)
			{
				throw new ArgumentNullException(nameof(dispatcherService));
			}

			this.DispatcherService = dispatcherService;
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.DispatcherService.Dispatch(DispatcherPriority.Frame, (x, y) => x(y), transientDelegate, transientInfo);
		}

		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			this.DispatcherService.Dispatch(DispatcherPriority.Frame, (x, y) => x(y), signalDelegate, signalInfo);
		}
	}
}