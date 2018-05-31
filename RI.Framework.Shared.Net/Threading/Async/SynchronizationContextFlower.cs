using System;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
	/// <summary>
	///     Implements an awaiter which flows the current <see cref="SynchronizationContext" /> around the execution of a <see cref="Task" />.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public sealed class SynchronizationContextFlower : CustomFlower
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SynchronizationContextFlower" />.
		/// </summary>
		/// <param name="task"> The <see cref="Task" /> around which flows the current <see cref="SynchronizationContext" />. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public SynchronizationContextFlower (Task task)
			: base(task)
		{
			this.SynchronizationContext = null;
		}

		#endregion




		#region Instance Properties/Indexer

		private SynchronizationContext SynchronizationContext { get; set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Capture ()
		{
			this.SynchronizationContext = SynchronizationContext.Current;
		}

		/// <inheritdoc />
		protected override void Restore ()
		{
			SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
		}

		#endregion
	}
}
