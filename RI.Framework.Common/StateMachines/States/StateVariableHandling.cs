namespace RI.Framework.StateMachines.States
{
	/// <summary>
	///     Specifies the handling of a state variable, declared by <see cref="StateVariableAttribute" />.
	/// </summary>
	public enum StateVariableHandling
	{
		/// <summary>
		///     The state variable handling is unspecified and behaves the same as if no <see cref="StateVariableAttribute" /> would be applied.
		///     The behaviour then depends on the used state resolver and cache.
		/// </summary>
		Unspecified = 0,

		/// <summary>
		///     When entering / for the entering state: The value of the property is taken from the property of the same name of the previous state (if available), or reset to its types default value (if not available).
		///     When leaving / for the leaving state: The value of the property is passed to the property of the same name in the next state (if available), then the property value is reset to its types default value.
		/// </summary>
		Transient = 1,

		/// <summary>
		///     When entering / for the entering state: The value of the property is reset to its types default value, even if the state comes from an instance-preserving cache.
		///     When leaving / for the leaving state: The value of the property is not passed to any property in the next state and is reset to its types default value.
		/// </summary>
		NonTransient = 2,
	}
}
