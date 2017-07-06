using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Tasks
{
	/// <summary>
	///     Implements a base class to implement custom flowers.
	/// </summary>
	/// <remarks>
	///     <note type="important">
	///         Some virtual methods are called from within locks to <see cref="CustomAwaiter.SyncRoot" />.
	///         Be caruful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class CustomFlower : CustomAwaiter, ICriticalNotifyCompletion
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CustomFlower" />.
		/// </summary>
		/// <param name="task"> The <see cref="Task" /> to flow around. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		protected CustomFlower (Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			this.Task = task;
			this.Awaiter = task.GetAwaiter();

			this.IsCaptured = false;
		}

		#endregion




		#region Instance Fields

		private bool _isCaptured;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether the flow was captured.
		/// </summary>
		/// <value>
		///     true if the flow was captured, false otherwise.
		/// </value>
		public bool IsCaptured
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isCaptured;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._isCaptured = value;
				}
			}
		}

		/// <summary>
		///     Gets the <see cref="Task" /> to flow around.
		/// </summary>
		/// <value>
		///     The <see cref="Task" /> to flow around.
		/// </value>
		public Task Task { get; }

		private TaskAwaiter Awaiter { get; }

		#endregion




		#region Abstracts

		/// <summary>
		///     Called to capture what needs to flow around the task.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         This method is called inside a lock to <see cref="CustomAwaiter.SyncRoot" />.
		///     </note>
		/// </remarks>
		protected abstract void Capture ();

		/// <summary>
		///     Called to restore what needs to flow around the task.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         This method is called inside a lock to <see cref="CustomAwaiter.SyncRoot" />.
		///     </note>
		/// </remarks>
		protected abstract void Restore ();

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override bool IsCompleted => this.Awaiter.IsCompleted;

		/// <inheritdoc />
		public override void GetResult ()
		{
			lock (this.SyncRoot)
			{
				this.Capture();

				this.IsCaptured = true;
			}

			this.Awaiter.GetResult();
		}

		#endregion




		#region Interface: ICriticalNotifyCompletion

		/// <inheritdoc />
		public override void OnCompleted (Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException(nameof(continuation));
			}

			lock (this.SyncRoot)
			{
				if (this.IsCaptured)
				{
					this.Restore();
				}
			}

			this.Awaiter.OnCompleted(continuation);
		}

		/// <inheritdoc cref="CustomAwaiter.OnCompleted" />
		public void UnsafeOnCompleted (Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException(nameof(continuation));
			}

			lock (this.SyncRoot)
			{
				if (this.IsCaptured)
				{
					this.Restore();
				}
			}

			this.Awaiter.UnsafeOnCompleted(continuation);
		}

		#endregion
	}
}
