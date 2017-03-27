using RI.Framework.Composition;
using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;




namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This state machine configuration uses a default configuration which is suitable for common scenarios where <see cref="Bootstrapper"/> and its associated default services (<see cref="IDispatcherService"/>, <see cref="CompositionContainer"/>, <see cref="ServiceLocator"/>) are used.
	/// By default, during construction, <see cref="DefaultStateMachineConfiguration"/> uses a <see cref="DispatcherServiceStateDispatcher"/> with <see cref="ServiceLocator"/>, a <see cref="StateResolver"/> with <see cref="ServiceLocator"/>, and a <see cref="StateCache"/>.
	/// </para>
	/// <para>
	/// <see cref="DefaultStateMachineConfiguration"/> uses the same default values as <see cref="StateMachineConfiguration"/>, except for <see cref="StateMachineConfiguration.Dispatcher"/> where an instance of <see cref="DispatcherServiceStateDispatcher"/> is used and <see cref="StateMachineConfiguration.Resolver"/> where an instance of <see cref="StateResolver"/> is used.
	/// Therefore, <see cref="DefaultStateMachineConfiguration"/> requires an <see cref="IDispatcherService"/> which can be retrieved using <see cref="ServiceLocator"/>.
	/// </para>
	/// <para>
	/// See <see cref="StateMachineConfiguration"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class DefaultStateMachineConfiguration : StateMachineConfiguration
	{
		/// <summary>
		/// Creates a new instance of <see cref="DefaultStateMachineConfiguration"/>.
		/// </summary>
		public DefaultStateMachineConfiguration ()
		{
			this.Dispatcher = new DispatcherServiceStateDispatcher();
			this.Resolver = new StateResolver();
		}
	}
}