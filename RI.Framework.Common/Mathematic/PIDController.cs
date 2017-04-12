using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Implements a PID controller.
	/// </summary>
	/// <remarks>
	/// <para>
	/// PID controllers compute an output (<see cref="Output"/>) which acts as a control value to control or drive a process.
	/// The output is computed based on a specified set point (<see cref="SetPoint"/>) and the feedback or measurement from the process (called process variable, <see cref="ProcessVariable"/>).
	/// The calculation uses the error (set point minus process variable) and several coefficients (proportional <see cref="KP"/>; integral <see cref="KI"/>; differential <see cref="KD"/>).
	/// To deactivate a coefficient, its value can be set to 0.0f.
	/// </para>
	///     <para>
	/// See <see href="https://en.wikipedia.org/wiki/PID_controller"> https://en.wikipedia.org/wiki/PID_controller </see> for details about PID controllers and their theory of operation.
	/// </para>
	/// <para>
	/// When using PID controller, be aware of steady-state-error, integral windup, noise sensitivity, and instability/oscillation.
	/// To increase stability, the output can be clamped using <see cref="OutputMin"/> and <see cref="OutputMax"/>.
	/// Especially the differential component is sensitive to noise in the process variable, therefore the differential component is disabled by default (<see cref="KD"/> is 0.0).
	/// </para>
	/// <para>
	/// <see cref="PIDController"/> can be used for discrete or continuous control.
	/// Their usage can be mixed, using <see cref="ComputeNewSetPoint"/> or <see cref="ComputeNewSetPoint(float,float,float)"/> with a timestep of 1.0f for discrete control or <see cref="ComputeNewSetPoint(float,float,float)"/> for continuous control.
	/// </para>
	/// <para>
	/// The following values are used after a new instance of <see cref="PIDController"/> is created or the controller has been reset using <see cref="Reset"/>:
	/// <see cref="KP"/>, <see cref="KI"/> are set to 1.0.
	/// <see cref="KD"/> is set to 0.0.
	/// <see cref="SetPoint"/>, <see cref="ProcessVariable"/>, <see cref="Output"/>, <see cref="OutputUnclamped"/> are set to 0.0.
	/// <see cref="OutputMin"/> is set to <see cref="float.MinValue"/> and <see cref="OutputMax"/> is set to <see cref="float.MaxValue"/>, effectively disable clamping of the output.
	/// <see cref="Error"/>, <see cref="Integral"/> are set to 0.0.
	/// <see cref="Loops"/> is set to 0.
	/// </para>
	/// <para>
	/// The following values are updated with each computation: <see cref="SetPoint"/>, <see cref="ProcessVariable"/>, <see cref="Output"/>, <see cref="OutputUnclamped"/>, <see cref="Error"/>, <see cref="Integral"/>, <see cref="Loops"/>.
	/// </para>
	/// </remarks>
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // create the controller
	/// PIDController controller = new PIDController();
	/// 
	/// // set parameters
	/// controller.KP = 0.5f;
	/// controller.KI = 0.22f;
	/// controller.KD = 0.0f;
	/// 
	/// // ...
	/// 
	/// // update the controller once per frame
	/// // Time.deltaTime is used as timestep to ensure that the controller behaves framerate-independent
	/// float newSetting = controller.ComputeNewSetPoint(aim, feedback, Time.deltaTime);
	/// ]]>
	/// </code>
	/// </example>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class PIDController
	{
		/// <summary>
		/// Creates a new instance of <see cref="PIDController"/>.
		/// </summary>
		public PIDController()
		{
			this.Reset();
		}

		/// <summary>
		/// The number of computations performed since the instance was created or the last time <see cref="Reset"/> was called.
		/// </summary>
		public int Loops;

		/// <summary>
		/// The proportional coefficient.
		/// </summary>
		public float KP;

		/// <summary>
		/// The integral coefficient.
		/// </summary>
		public float KI;

		/// <summary>
		/// The differential coefficient.
		/// </summary>
		public float KD;

		/// <summary>
		/// The last set point.
		/// </summary>
		public float SetPoint;

		/// <summary>
		/// The last process variable.
		/// </summary>
		public float ProcessVariable;

		/// <summary>
		/// The current output (clamped by <see cref="OutputMin"/> and <see cref="OutputMax"/>).
		/// </summary>
		public float Output;

		/// <summary>
		/// The current output (not clamped).
		/// </summary>
		public float OutputUnclamped;

		/// <summary>
		/// Minimum allowed value of the output.
		/// </summary>
		public float OutputMin;

		/// <summary>
		/// Maximum allowed value of the output.
		/// </summary>
		public float OutputMax;

		/// <summary>
		/// The current error (set point minus process variable).
		/// </summary>
		public float Error;

		/// <summary>
		/// The current integral value used for the integral component.
		/// </summary>
		public float Integral;

		/// <summary>
		/// Resets the controller values to their initial values.
		/// </summary>
		public void Reset ()
		{
			this.Loops = 0;

			this.KP = 1.0f;
			this.KI = 1.0f;
			this.KD = 0.0f;

			this.SetPoint = 0.0f;
			this.ProcessVariable = 0.0f;
			this.Output = 0.0f;
			this.OutputUnclamped = 0.0f;

			this.OutputMin = float.MinValue;
			this.OutputMax = float.MaxValue;

			this.Error = 0.0f;
			this.Integral = 0.0f;
		}

		/// <summary>
		/// Keeps the set point and computes the controller output using a timestep of 1.0.
		/// </summary>
		/// <param name="processVariable">The process variable (feedback or measurement from the controlled process).</param>
		/// <returns>
		/// The computet and clamped output (same as <see cref="Output"/>.
		/// </returns>
		public float ComputeKeepSetPoint(float processVariable)
		{
			return this.ComputeNewSetPoint(this.SetPoint, processVariable, 1.0f);
		}

		/// <summary>
		///  Keeps the set point and computes the controller output using a given timestep.
		/// </summary>
		/// <param name="processVariable">The process variable (feedback or measurement from the controlled process).</param>
		/// <param name="timestep">The timestep for the current process value.</param>
		/// <returns>
		/// The computet and clamped output (same as <see cref="Output"/>.
		/// </returns>
		public float ComputeKeepSetPoint(float processVariable, float timestep)
		{
			return this.ComputeNewSetPoint(this.SetPoint, processVariable, timestep);
		}

		/// <summary>
		/// Sets a new set point and computes the controller output using a timestep of 1.0.
		/// </summary>
		/// <param name="setPoint">The used set point (the desired value).</param>
		/// <param name="processVariable">The process variable (feedback or measurement from the controlled process).</param>
		/// <returns>
		/// The computet and clamped output (same as <see cref="Output"/>.
		/// </returns>
		public float ComputeNewSetPoint(float setPoint, float processVariable)
		{
			return this.ComputeNewSetPoint(setPoint, processVariable, 1.0f);
		}

		/// <summary>
		/// Sets a new set point and computes the controller output using a given timestep.
		/// </summary>
		/// <param name="setPoint">The used set point (the desired value).</param>
		/// <param name="processVariable">The process variable (feedback or measurement from the controlled process).</param>
		/// <param name="timestep">The timestep for the current process value.</param>
		/// <returns>
		/// The computet and clamped output (same as <see cref="Output"/>.
		/// </returns>
		public float ComputeNewSetPoint(float setPoint, float processVariable, float timestep)
		{
			this.Loops++;

			float error = setPoint - processVariable;
			float integral = this.Integral + (error * timestep);
			float differential = (error - this.Error) / timestep;

			float p = this.KP * error;
			float i = this.KI * integral;
			float d = this.KD * differential;

			float outputUnclamped = p + i + d;
			float output = outputUnclamped;
			if (output < this.OutputMin)
			{
				output = this.OutputMin;
			}
			if (output > this.OutputMax)
			{
				output = this.OutputMax;
			}

			this.SetPoint = setPoint;
			this.ProcessVariable = processVariable;
			this.Output = output;
			this.OutputUnclamped = outputUnclamped;
			this.Error = error;
			this.Integral = integral;

			return output;
		}
	}
}