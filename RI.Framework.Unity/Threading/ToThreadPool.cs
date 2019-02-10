using System.Threading;

using UnityEngine;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Yield instruction to move a coroutine to a <see cref="ThreadPool" /> thread.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ThreadMover" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class ToThreadPool : YieldInstruction
    {
    }
}
