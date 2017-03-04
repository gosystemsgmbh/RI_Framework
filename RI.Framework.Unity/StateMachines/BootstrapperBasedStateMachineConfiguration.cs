using RI.Framework.Composition;
using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;




namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Implements a state machine configuration for bootstrapper-based scenarios.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This state machine configuration uses a default configuration which is suitable for common scenarios where <see cref="Bootstrapper"/> and its associated default services (<see cref="IDispatcherService"/>, <see cref="CompositionContainer"/>, <see cref="ServiceLocator"/>) are used.
	/// By default, during construction, <see cref="BootstrapperBasedStateMachineConfiguration"/> uses a <see cref="StateDispatcher"/> with <see cref="ServiceLocator"/>, a <see cref="StateResolver"/> with <see cref="ServiceLocator"/>, and a <see cref="StateCache"/>.
	/// Furthermore, <see cref="StateMachineConfiguration.EnableAutomaticCaching"/> is set to true.
	/// </para>
	/// <para>
	/// See <see cref="StateMachineConfiguration"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class BootstrapperBasedStateMachineConfiguration : StateMachineConfiguration
	{
		/// <summary>
		/// Creates a new instance of <see cref="BootstrapperBasedStateMachineConfiguration"/>.
		/// </summary>
		public BootstrapperBasedStateMachineConfiguration ()
		{
			this.EnableAutomaticCaching = true;

			this.Dispatcher = new StateDispatcher();
			this.Resolver = new StateResolver();
			this.Cache = new StateCache();
		}
	}
}