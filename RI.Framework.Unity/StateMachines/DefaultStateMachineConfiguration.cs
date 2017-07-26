using RI.Framework.Composition;
using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="DispatcherServiceStateDispatcher" /> for which it gets the required instances of <see cref="IDispatcherService" /> through <see cref="ServiceLocator" />.
	///         If no instance of <see cref="IDispatcherService"/> can be retrieved, no <see cref="DispatcherServiceStateDispatcher"/> is created and you must supply an instance of <see cref="IStateDispatcher"/> (using <see cref="StateMachineConfiguration.Dispatcher"/>).
	///     </para>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="CompositionContainerStateResolver" /> for which it gets the required instances of <see cref="CompositionContainer" /> through <see cref="ServiceLocator" />.
	///         If no instance of <see cref="CompositionContainer"/> can be retrieved, no <see cref="CompositionContainerStateResolver"/> is created and you must supply an instance of <see cref="IStateResolver"/> (using <see cref="StateMachineConfiguration.Resolver"/>).
	///     </para>
	///     <para>
	///         See <see cref="StateMachineConfiguration" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultStateMachineConfiguration : StateMachineConfiguration<DefaultStateMachineConfiguration>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateMachineConfiguration" />.
		/// </summary>
		public DefaultStateMachineConfiguration ()
		{
			IDispatcherService dispatcherService = ServiceLocator.GetInstance<DispatcherService>() ?? ServiceLocator.GetInstance<IDispatcherService>();
			CompositionContainer compositionContainer = ServiceLocator.GetInstance<CompositionContainer>(); //TODO: Also get ICompositionContainer

			this.Dispatcher = dispatcherService == null ? null : new DispatcherServiceStateDispatcher(dispatcherService);
			this.Resolver = compositionContainer == null ? null : new CompositionContainerStateResolver(compositionContainer);
		}

		#endregion
	}
}
