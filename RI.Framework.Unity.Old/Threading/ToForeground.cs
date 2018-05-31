using UnityEngine;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Yield instruction to move a task to Unitys main/foreground thread.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ThreadMover" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class ToForeground : YieldInstruction
	{
	}
}
