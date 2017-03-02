namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Defines the configuration of a state machine.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="StateMachine"/> for more details about state machines.
	/// </para>
	/// </remarks>
	public class StateMachineConfiguration
	{
		/// <summary>
		/// Creates a new instance of <see cref="StateMachineConfiguration"/>.
		/// </summary>
		public StateMachineConfiguration ()
		{
			this.Dispatcher = null;
			this.Resolver = null;
			this.Cache = null;

			this.EnableAutomaticCaching = true;
		}

		/// <summary>
		/// Gets or sets the used dispatcher.
		/// </summary>
		/// <value>
		/// The used dispatcher.
		/// </value>
		/// <remarks>
		/// <para>
		/// Implementations of <see cref="IStateDispatcher"/> can be shared among multiple state machines.
		/// </para>
		/// <note type="note">
		/// The dispatcher of a state machine should not be changed while the current state is not null.
		/// </note>
		/// </remarks>
		public IStateDispatcher Dispatcher { get; set; }

		/// <summary>
		/// Gets or sets the used state resolver.
		/// </summary>
		/// <value>
		/// The used state resolver.
		/// </value>
		/// <remarks>
		/// <para>
		/// Implementations of <see cref="IStateResolver"/> can be shared among multiple state machines.
		/// </para>
		/// <note type="note">
		/// The resolver of a state machine can be changed at any time.
		/// </note>
		/// </remarks>
		public IStateResolver Resolver { get; set; }

		/// <summary>
		/// Gets or sets the used state instance cache.
		/// </summary>
		/// <value>
		/// The used state instance cache.
		/// </value>
		/// <remarks>
		/// <note type="important">
		/// Implementations of <see cref="IStateCache"/> should not be shared among multiple state machines unless the cached <see cref="IState"/> instances know how to behave if they are the current state of more than one state machine at the same time.
		/// </note>
		/// <note type="note">
		/// The state cache of a state machine should not be changed while the current state is not null.
		/// </note>
		/// </remarks>
		public IStateCache Cache { get; set; }

		/// <summary>
		/// Gets or sets whether state instances are automatically added to the cache by <see cref="StateMachine"/> after they became the current state for the first time.
		/// </summary>
		/// <value>
		/// true if a state instance is added to the cache after it became the current state for the first time, false if the state instances must be added manually.
		/// </value>
		public bool EnableAutomaticCaching { get; set; }
	}
}