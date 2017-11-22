using RI.Framework.Composition;
using RI.Framework.Services.Dispatcher;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="DispatcherServiceStateDispatcher" /> for which it gets the required instances of <see cref="IDispatcherService" /> through a constructor parameter or through <see cref="ServiceLocator" />.
	///         If no instance of <see cref="IDispatcherService" /> can be retrieved, no <see cref="DispatcherServiceStateDispatcher" /> is created and you must supply an instance of <see cref="IStateDispatcher" /> (using <see cref="StateMachineConfiguration.Dispatcher" />).
	///     </para>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="DependencyResolverStateResolver" /> for which it gets the required instances of <see cref="IDependencyResolver" /> through a constructor parameter or through <see cref="ServiceLocator" />.
	///         If no instance of <see cref="IDependencyResolver" /> can be retrieved, no <see cref="DependencyResolverStateResolver" /> is created and you must supply an instance of <see cref="IStateResolver" /> (using <see cref="StateMachineConfiguration.Resolver" />).
	///         Note that <see cref="CompositionContainer" /> implements <see cref="IDependencyResolver" /> and can therefore be used as a resolver constructor parameter.
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
			: this(null, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateMachineConfiguration" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher service. Can be null to use <see cref="ServiceLocator" /> to retrieve an instance of <see cref="IDispatcherService" />. </param>
		public DefaultStateMachineConfiguration (IDispatcherService dispatcher)
			: this(dispatcher, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateMachineConfiguration" />.
		/// </summary>
		/// <param name="resolver"> The used dependency resolver. Can be null to use <see cref="ServiceLocatorStateResolver" />. </param>
		public DefaultStateMachineConfiguration (IDependencyResolver resolver)
			: this(null, resolver)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateMachineConfiguration" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher service. Can be null to use <see cref="ServiceLocator" /> to retrieve an instance of <see cref="IDispatcherService" />. </param>
		/// <param name="resolver"> The used dependency resolver. Can be null to use <see cref="ServiceLocatorStateResolver" />. </param>
		public DefaultStateMachineConfiguration (IDispatcherService dispatcher, IDependencyResolver resolver)
		{
			IDispatcherService dispatcherService = dispatcher ?? ServiceLocator.GetInstance<IDispatcherService>() ?? ServiceLocator.GetInstance<DispatcherService>();
			IDependencyResolver dependencyResolver = resolver ?? ServiceLocator.GetInstance<IDependencyResolver>() ?? ServiceLocator.GetInstance<CompositionContainer>();

			this.Dispatcher = dispatcherService == null ? null : new DispatcherServiceStateDispatcher(dispatcherService);
			this.Resolver = dependencyResolver == null ? null : new DependencyResolverStateResolver(dependencyResolver);
		}

		#endregion
	}
}
