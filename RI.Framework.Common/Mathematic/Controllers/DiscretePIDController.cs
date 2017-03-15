using System;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Mathematic.Controllers
{
	/// <summary>
	/// A discrete PID controller.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="PIDControllerBase"/> for more information.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class DiscretePIDController : PIDControllerBase
	{
		public float Compute(float processVariable)
		{
			return this.Compute(this.SetPoint, processVariable);
		}

		public float Compute (float setPoint, float processVariable)
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