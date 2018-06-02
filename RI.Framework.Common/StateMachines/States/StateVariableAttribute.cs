using System;




namespace RI.Framework.StateMachines.States
{
	/// <summary>
	///     Used to declare state variables in <see cref="IState" />s.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="StateVariableAttribute" /> can be applied to properties in states to turn them into state variables.
	///         State variables will be automatically copied by the <see cref="StateMachine" /> from leaving to entering states during transients.
	///     </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class StateVariableAttribute : Attribute
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateVariableAttribute" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="StateVariableHandling.Transient" /> is used.
		///     </para>
		/// </remarks>
		public StateVariableAttribute ()
		{
			this.Handling = StateVariableHandling.Transient;
		}

		/// <summary>
		///     Creates a new instance of <see cref="StateVariableAttribute" />.
		/// </summary>
		/// <param name="handling"> The used state variable handling. </param>
		public StateVariableAttribute (StateVariableHandling handling)
		{
			this.Handling = handling;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used state variable handling.
		/// </summary>
		/// <value>
		///     The used state variable handling.
		/// </value>
		public StateVariableHandling Handling { get; }

		#endregion
	}
}
