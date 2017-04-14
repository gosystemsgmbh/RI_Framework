using System;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a default state machine operation dispatcher suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="DispatcherServiceStateDispatcher" /> internally uses a specified <see cref="IDispatcherService" />.
	///         If no such is provided, <see cref="ServiceLocator" /> and <see cref="Singleton{T}" /> are used to retrieve a <see cref="IDispatcherService" />.
	///     </para>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <note type="note">
	///         Signals and transitions are dispatched using the <see cref="DispatcherPriority.Default" /> priority.
	///     </note>
	/// </remarks>
	public sealed class DispatcherServiceStateDispatcher : IStateDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DispatcherServiceStateDispatcher" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="DispatcherServiceStateDispatcher" /> instances created with this constructor use <see cref="ServiceLocator" /> to retrieve an instance of <see cref="IDispatcherService" /> at construction time.
		///         If <see cref="ServiceLocator" /> does not return an instance, <see cref="Singleton{IDispatcherService}" /> is used.
		///     </para>
		///     <note type="important">
		///         For performance reasons, the <see cref="IDispatcherService" /> instance which is used is not dynamically retrieved from <see cref="ServiceLocator" /> or <see cref="Singleton{IDispatcherService}" /> for each dispatched operation.
		///         The <see cref="IDispatcherService" /> instance is only once retrieved when this constructor is executed.
		///     </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> Neither <see cref="ServiceLocator" /> nor <see cref="Singleton{IDispatcherService}" /> could retrieve an instance of <see cref="IDispatcherService" />. </exception>
		public DispatcherServiceStateDispatcher ()
		{
			this.DispatcherService = ServiceLocator.GetInstance<DispatcherService>() ?? ServiceLocator.GetInstance<IDispatcherService>() ?? Singleton<DispatcherService>.Instance ?? Singleton<IDispatcherService>.Instance;
			if (this.DispatcherService == null)
			{
				throw new InvalidOperationException("Could not retrieve an instance of " + nameof(IDispatcherService) + " for " + this.GetType().Name + ".");
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="DispatcherServiceStateDispatcher" />.
		/// </summary>
		/// <param name="dispatcherService"> The used dispatcher service. </param>
		/// <remarks>
		///     <para>
		///         <see cref="DispatcherServiceStateDispatcher" /> instances created with this constructor use the <see cref="IDispatcherService" /> specified by <paramref name="dispatcherService" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcherService" /> is null. </exception>
		public DispatcherServiceStateDispatcher (IDispatcherService dispatcherService)
		{
			if (dispatcherService == null)
			{
				throw new ArgumentNullException(nameof(dispatcherService));
			}

			this.DispatcherService = dispatcherService;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher service.
		/// </summary>
		/// <value>
		///     The used dispatcher service.
		/// </value>
		public IDispatcherService DispatcherService { get; private set; }

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			this.DispatcherService.Dispatch(DispatcherPriority.Default, (x, y) => x(y), signalDelegate, signalInfo);
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.DispatcherService.Dispatch(DispatcherPriority.Default, (x, y) => x(y), transientDelegate, transientInfo);
		}

		#endregion
	}
}
