using System.Threading;

using UnityEngine;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Yield instruction to move a task to a <see cref="ThreadPool"/> thread.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="ThreadMover"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class ToBackground : YieldInstruction
	{
	}
}