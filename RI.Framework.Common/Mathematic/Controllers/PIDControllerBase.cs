using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Mathematic.Controllers
{
	/// <summary>
	/// Base class for PID controllers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// PID controllers compute an output (<see cref="Output"/>), which drives a process, based on a specified set point (<see cref="SetPoint"/>) and the feedback from the process (called process variable, <see cref="ProcessVariable"/>).
	/// The calculation uses the error (set point minus process variable) and several coefficients (proportional <see cref="KP"/>; integral <see cref="KI"/>, differential <see cref="KD"/>).
	/// To deactivate a coefficient, set its value to 0.0.
	/// </para>
	///     <para>
	/// See <see href="https://en.wikipedia.org/wiki/PID_controller"> https://en.wikipedia.org/wiki/PID_controller </see> for details about PID controllers and their theory of operation.
	/// </para>
	/// <para>
	/// The following values are used after initialization:
	/// <see cref="KP"/>, <see cref="KI"/>, <see cref="KD"/> are set to 1.0.
	/// <see cref="SetPoint"/>, <see cref="ProcessVariable"/>, <see cref="Output"/> are set to 0.0.
	/// <see cref="OutputMin"/> is set to <see cref="float.MinValue"/> and <see cref="OutputMax"/> is set to <see cref="float.MaxValue"/>.
	/// </para>
	/// <para>
	/// The following values are updated with each computation: <see cref="SetPoint"/>, <see cref="ProcessVariable"/>, <see cref="Output"/>.
	/// </para>
	/// <para>
	/// Classes which implement PID controllers are: <see cref="DiscretePIDController"/>, <see cref="ContinuousPIDController"/>.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public abstract class PIDControllerBase
	{
		internal PIDControllerBase ()
		{
			this.Reset();
		}

		public float KP;

		public float KI;

		public float KD;

		public float SetPoint;

		public float ProcessVariable;

		public float Output;

		public float OutputMin;

		public float OutputMax;

		public void Reset ()
		{
			this.KP = 1.0f;
			this.KI = 1.0f;
			this.KD = 1.0f;

			this.SetPoint = 0.0f;
			this.ProcessVariable = 0.0f;
			this.Output = 0.0f;

			this.OutputMin = float.MinValue;
			this.OutputMax = float.MaxValue;
		}
	}
}