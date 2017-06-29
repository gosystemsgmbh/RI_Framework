using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Threading
{
	/// <summary>
	/// Implements a <see cref="TaskScheduler" /> which uses a <see cref="IThreadDispatcher" /> for execution.
	/// </summary>
	public sealed class ThreadDispatcherTaskScheduler : TaskScheduler, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		/// Creates a new instance of <see cref="ThreadDispatcherTaskScheduler"/>.
		/// </summary>
		/// <param name="dispatcher">The used dispatcher.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> is null.</exception>
		public ThreadDispatcherTaskScheduler(IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.SyncRoot = new object();
			this.Dispatcher = dispatcher;
			this.Tasks = new Dictionary<Task, ThreadDispatcherOperation>();
			this.TaskExecutor = this.ExecuteTask;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher Dispatcher { get; }

		private Dictionary<Task, ThreadDispatcherOperation> Tasks { get; }

		private object SyncRoot { get; }

		private Func<Task, bool> TaskExecutor { get; }

		#endregion


		private bool ExecuteTask (Task task)
		{
			lock (this.SyncRoot)
			{
				this.Tasks.Remove(task);
			}

			return this.TryExecuteTask(task);
		}

		/// <inheritdoc />
		protected override void QueueTask (Task task)
		{
			lock (this.SyncRoot)
			{
				if (this.Tasks.ContainsKey(task))
				{
					return;
				}

				ThreadDispatcherOperation operation = this.Dispatcher.Post(this.TaskExecutor, task);
				this.Tasks.Add(task, operation);
			}
		}

		/// <inheritdoc />
		protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued) => false;

		/// <inheritdoc />
		protected override IEnumerable<Task> GetScheduledTasks () => this.Tasks.Keys.ToArray();

		/// <inheritdoc />
		protected override bool TryDequeue (Task task)
		{
			lock (this.SyncRoot)
			{
				if (!this.Tasks.ContainsKey(task))
				{
					return false;
				}

				ThreadDispatcherOperation operation = this.Tasks[task];
				this.Tasks.Remove(task);
				return operation.Cancel();
			}
		}

		/// <inheritdoc />
		public override int MaximumConcurrencyLevel => 1;

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;
	}
}
