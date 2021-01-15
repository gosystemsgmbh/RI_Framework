using UnityEngine;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Yield instruction to move a coroutine to Unitys main/foreground thread.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ThreadMover" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class ToForeground : YieldInstruction
    {
    }
}
