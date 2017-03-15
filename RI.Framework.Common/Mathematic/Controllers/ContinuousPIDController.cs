using System;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Mathematic.Controllers
{
	/// <summary>
	/// A continuous PID controller.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="PIDControllerBase"/> for more information.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class ContinuousPIDController : PIDControllerBase
	{
		public float Compute(float processVariable, float timestep)
		{
			return this.Compute(this.SetPoint, processVariable, timestep);
		}

		public float Compute(float setPoint, float processVariable, float timestep)
		{
			this.SetPoint = setPoint;
			this.ProcessVariable = processVariable;

			//TODO: calculate

			this.Output = Math.Max(this.OutputMin, this.Output);
			this.Output = Math.Min(this.OutputMax, this.Output);

			return this.Output;
		}
	}
}