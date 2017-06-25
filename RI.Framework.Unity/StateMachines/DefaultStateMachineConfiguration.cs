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
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="DispatcherServiceStateDispatcher" /> and <see cref="CompositionContainerStateResolver" /> for which it gets the required instances of <see cref="IDispatcherService" /> and <see cref="CompositionContainer" /> through <see cref="ServiceLocator" />.
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
			CompositionContainer compositionContainer = ServiceLocator.GetInstance<CompositionContainer>();

			this.Dispatcher = dispatcherService == null ? null : new DispatcherServiceStateDispatcher(dispatcherService);
			this.Resolver = compositionContainer == null ? null : new CompositionContainerStateResolver(compositionContainer);
		}

		#endregion
	}
}
