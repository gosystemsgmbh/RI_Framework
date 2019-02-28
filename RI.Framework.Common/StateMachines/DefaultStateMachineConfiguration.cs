using RI.Framework.StateMachines.Caches;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a state machine configuration with default values suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         By default, <see cref="DefaultStateMachineConfiguration" /> uses a <see cref="DefaultStateDispatcher" />, <see cref="DefaultStateResolver" />, and <see cref="DefaultStateCache"/>.
	///     </para>
	///     <para>
	///         See <see cref="StateMachineConfiguration" /> and <see cref="StateMachineConfiguration{T}"/> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultStateMachineConfiguration : StateMachineConfiguration<DefaultStateMachineConfiguration>
	{
	}
}
