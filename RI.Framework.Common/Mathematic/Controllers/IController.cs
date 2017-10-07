namespace RI.Framework.Mathematic.Controllers
{
	/// <summary>
	///     Defines the interface for controllers.
	/// </summary>
	public interface IController
	{
		/// <summary>
		///     Gets or sets the number of computations performed since the instance was created or the last time <see cref="Reset" /> was called.
		/// </summary>
		/// <value>
		///     The number of computations performed since the instance was created or the last time <see cref="Reset" /> was called.
		/// </value>
		int Loops { get; set; }

		/// <summary>
		///     Gets or sets the current output (clamped by <see cref="OutputMin" /> and <see cref="OutputMax" />).
		/// </summary>
		/// <value>
		///     The current output (clamped by <see cref="OutputMin" /> and <see cref="OutputMax" />).
		/// </value>
		float Output { get; set; }

		/// <summary>
		///     Gets or sets the maximum allowed value of the output.
		/// </summary>
		/// <value>
		///     Maximum allowed value of the output.
		/// </value>
		float OutputMax { get; set; }

		/// <summary>
		///     Gets or sets the minimum allowed value of the output.
		/// </summary>
		/// <value>
		///     Minimum allowed value of the output.
		/// </value>
		float OutputMin { get; set; }

		/// <summary>
		///     Gets or sets the current output (not clamped).
		/// </summary>
		/// <value>
		///     The current output (not clamped).
		/// </value>
		float OutputUnclamped { get; set; }

		/// <summary>
		///     Gets or sets the last process variable.
		/// </summary>
		/// <value>
		///     The last process variable.
		/// </value>
		float ProcessVariable { get; set; }

		/// <summary>
		///     Gets or sets the last set point.
		/// </summary>
		/// <value>
		///     The last set point.
		/// </value>
		float SetPoint { get; set; }

		/// <summary>
		///     Keeps the set point and computes the controller output using a timestep of 1.0.
		/// </summary>
		/// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
		/// <returns>
		///     The computet and clamped output (same as <see cref="Output" />.
		/// </returns>
		float ComputeKeepSetPoint (float processVariable);

		/// <summary>
		///     Keeps the set point and computes the controller output using a given timestep.
		/// </summary>
		/// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
		/// <param name="timestep"> The timestep for the current process value. </param>
		/// <returns>
		///     The computet and clamped output (same as <see cref="Output" />.
		/// </returns>
		float ComputeKeepSetPoint (float processVariable, float timestep);

		/// <summary>
		///     Sets a new set point and computes the controller output using a timestep of 1.0.
		/// </summary>
		/// <param name="setPoint"> The used set point (the desired value). </param>
		/// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
		/// <returns>
		///     The computet and clamped output (same as <see cref="Output" />.
		/// </returns>
		float ComputeWithNewSetPoint (float setPoint, float processVariable);

		/// <summary>
		///     Sets a new set point and computes the controller output using a given timestep.
		/// </summary>
		/// <param name="setPoint"> The used set point (the desired value). </param>
		/// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
		/// <param name="timestep"> The timestep for the current process value. </param>
		/// <returns>
		///     The computet and clamped output (same as <see cref="Output" />.
		/// </returns>
		float ComputeWithNewSetPoint (float setPoint, float processVariable, float timestep);

		/// <summary>
		///     Resets the controller values to their initial values.
		/// </summary>
		void Reset ();
	}
}
