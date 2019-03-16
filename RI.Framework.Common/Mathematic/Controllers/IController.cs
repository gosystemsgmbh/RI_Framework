using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mathematic.Controllers
{
    /// <summary>
    ///     Defines the interface for mathematical controllers.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See the concrete controller implementation for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IController
    {
        /// <summary>
        ///     Gets the number of computations performed since the instance was created or the last time <see cref="Reset" /> was called.
        /// </summary>
        /// <value>
        ///     The number of computations performed since the instance was created or the last time <see cref="Reset" /> was called.
        /// </value>
        int Loops { get; }

        /// <summary>
        ///     Gets the current output (clamped by <see cref="OutputMin" /> and <see cref="OutputMax" />).
        /// </summary>
        /// <value>
        ///     The current output (clamped by <see cref="OutputMin" /> and <see cref="OutputMax" />).
        /// </value>
        float Output { get; }

        /// <summary>
        ///     Gets or sets the maximum allowed value of the output.
        /// </summary>
        /// <value>
        ///     Maximum allowed value of the output.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="OutputUnclamped"/> is not updated when changing the value of <see cref="OutputMax"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotFiniteArgumentException"><paramref name="value"/> is NaN or infinity.</exception>
        float OutputMax { get; set; }

        /// <summary>
        ///     Gets or sets the minimum allowed value of the output.
        /// </summary>
        /// <value>
        ///     Minimum allowed value of the output.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="OutputUnclamped"/> is not updated when changing the value of <see cref="OutputMin"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotFiniteArgumentException"><paramref name="value"/> is NaN or infinity.</exception>
        float OutputMin { get; set; }

        /// <summary>
        ///     Gets the current output (not clamped).
        /// </summary>
        /// <value>
        ///     The current output (not clamped).
        /// </value>
        float OutputUnclamped { get; }

        /// <summary>
        ///     Gets the last process variable.
        /// </summary>
        /// <value>
        ///     The last process variable.
        /// </value>
        float ProcessVariable { get; }

        /// <summary>
        ///     Gets the last set point.
        /// </summary>
        /// <value>
        ///     The last set point.
        /// </value>
        float SetPoint { get; }

        /// <summary>
        ///     Keeps the set point and computes the controller output using the process variable and a timestep of 1.0.
        /// </summary>
        /// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
        /// <returns>
        ///     The computed and clamped output (same as <see cref="Output" />.
        /// </returns>
        /// <exception cref="NotFiniteArgumentException"> <paramref name="processVariable" /> is NaN or infinity. </exception>
        float Compute (float processVariable);

        /// <summary>
        ///     Keeps the set point and computes the controller output using the process variable and a given timestep.
        /// </summary>
        /// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
        /// <param name="timestep"> The timestep for the current process value. </param>
        /// <returns>
        ///     The computed and clamped output (same as <see cref="Output" />.
        /// </returns>
        /// <exception cref="NotFiniteArgumentException"> <paramref name="processVariable" /> or <paramref name="timestep"/> is NaN or infinity. </exception>
        float Compute (float processVariable, float timestep);

        /// <summary>
        ///     Sets a new set point and computes the controller output using the process variable and a timestep of 1.0.
        /// </summary>
        /// <param name="setPoint"> The used set point (the desired value). </param>
        /// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
        /// <returns>
        ///     The computed and clamped output (same as <see cref="Output" />.
        /// </returns>
        /// <exception cref="NotFiniteArgumentException"> <paramref name="setPoint" /> or <paramref name="processVariable"/> is NaN or infinity. </exception>
        float ComputeWithNewSetPoint (float setPoint, float processVariable);

        /// <summary>
        ///     Sets a new set point and computes the controller output using the process variable and a given timestep.
        /// </summary>
        /// <param name="setPoint"> The used set point (the desired value). </param>
        /// <param name="processVariable"> The process variable (feedback or measurement from the controlled process). </param>
        /// <param name="timestep"> The timestep for the current process value. </param>
        /// <returns>
        ///     The computed and clamped output (same as <see cref="Output" />.
        /// </returns>
        /// <exception cref="NotFiniteArgumentException"> <paramref name="setPoint" />, <paramref name="processVariable"/>, or <paramref name="timestep"/> is NaN or infinity. </exception>
        float ComputeWithNewSetPoint (float setPoint, float processVariable, float timestep);

        /// <summary>
        ///     Resets the controller values to their initial values.
        /// </summary>
        void Reset ();
    }
}
