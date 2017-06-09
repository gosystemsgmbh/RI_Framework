using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="StateDispatcher" /> and <see cref="StateResolver"/>.
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
		}

		#endregion
	}
}
