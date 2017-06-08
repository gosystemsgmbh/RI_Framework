using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="DefaultStateMachineConfiguration" /> uses the same default values as <see cref="StateMachineConfiguration" />, except for <see cref="StateMachineConfiguration.Dispatcher" /> where an instance of <see cref="SynchronizationContextStateDispatcher" /> is used and <see cref="StateMachineConfiguration.Resolver" /> where an instance of <see cref="StateResolver" /> is used.
	///     </para>
	///     <para>
	///         See <see cref="StateMachineConfiguration" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class DefaultStateMachineConfiguration : StateMachineConfiguration
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateMachineConfiguration" />.
		/// </summary>
		public DefaultStateMachineConfiguration ()
		{
			this.Dispatcher = new SynchronizationContextStateDispatcher();
			this.Resolver = new StateResolver();
		}

		#endregion
	}
}
