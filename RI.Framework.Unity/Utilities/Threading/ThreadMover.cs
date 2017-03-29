using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using UnityEngine;

using Object = UnityEngine.Object;
using ThreadPriority = System.Threading.ThreadPriority;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Provides functionality to move execution of tasks (or coroutines respectively) between threads and dispatchers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// 
	/// </para>
	/// </remarks>
	public static class ThreadMover
	{
		/// <summary>
		/// Begins execution of a task.
		/// </summary>
		/// <param name="task">The task to execute.</param>
		/// <remarks>
		/// <para>
		/// See the description of the <see cref="ThreadMover"/> class for more details.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="task"/> is null.</exception>
		public static void BeginTask (IEnumerator task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			if (ThreadMover.SyncRoot == null)
			{
				ThreadMover.SyncRoot = new object();

				ThreadMover.MoverObject = new GameObject(typeof(ThreadMover).Name);
				ThreadMover.MoverObject.SetActive(true);
				Object.DontDestroyOnLoad(ThreadMover.MoverObject);

				ThreadMover.MoverHandler = ThreadMover.MoverObject.AddComponent<ThreadMoverHandler>();
				ThreadMover.MoverHandler.StartCoroutine(ThreadMover.MoverHandler.DispatchForeground());

				ThreadMover.TasksToMoveToForeground = new List<MoveableTask>();
			}

			lock (ThreadMover.SyncRoot)
			{
				ThreadMover.TasksToMoveToForeground.Add(new MoveableTask(task));
			}
		}

		private static object SyncRoot;
		private static GameObject MoverObject;
		private static ThreadMoverHandler MoverHandler;
		private static List<MoveableTask> TasksToMoveToForeground;

		private sealed class ThreadMoverHandler : MonoBehaviour
		{
			[SuppressMessage("ReSharper", "IteratorNeverReturns")]
			public IEnumerator DispatchForeground ()
			{
				while (true)
				{
					yield return null;

					MoveableTask[] tasksToMove = null;
					lock (ThreadMover.SyncRoot)
					{
						if (ThreadMover.TasksToMoveToForeground.Count > 0)
						{
							tasksToMove = ThreadMover.TasksToMoveToForeground.ToArray();
							ThreadMover.TasksToMoveToForeground.Clear();
						}
					}

					if (tasksToMove != null)
					{
						foreach (MoveableTask taskToMove in tasksToMove)
						{
							this.StartCoroutine(taskToMove);
						}
					}
				}
			}
		}

		private sealed class MoveableTask : IEnumerator
		{
			public MoveableTask(IEnumerator task)
			{
				if (task == null)
				{
					throw new ArgumentNullException(nameof(task));
				}

				this.Task = task;
				this.Current = null;
				this.HasInjectedYield = false;
				this.InjectedYield = null;
			}

			public IEnumerator Task { get; private set; }
			public object Current { get; private set; }
			public bool HasInjectedYield { get; private set; }
			public object InjectedYield { get; private set; }

			public bool MoveNext ()
			{
				if (this.HasInjectedYield)
				{
					this.HasInjectedYield = false;
					this.Current = this.InjectedYield;
					return true;
				}

				if (!this.Task.MoveNext())
				{
					return false;
				}

				this.Current = this.Task.Current;

				ToDispatcher toDispatcher = this.Current as ToDispatcher;
				ToHeavyThread toHeavyThread = this.Current as ToHeavyThread;
				ToBackground toBackground = this.Current as ToBackground;
				ToForeground toForeground = this.Current as ToForeground;

				if (toDispatcher != null)
				{
					if (toDispatcher.DispatcherService != null)
					{
						toDispatcher.DispatcherService.Dispatch(toDispatcher.Priority, this.HandleBackground);
					}
					else
					{
						toDispatcher.ThreadDispatcher.Post(new Action<MoveableTask>(x => x.HandleBackground()), this);
					}
					return false;
				}

				if (toHeavyThread != null)
				{
					HeavyThreadExecutor heavyThread = new HeavyThreadExecutor(this, toHeavyThread.Priority);
					heavyThread.Start();
					return false;
				}

				if (toBackground != null)
				{
					ThreadPool.QueueUserWorkItem(x => ((MoveableTask)x).HandleBackground(), this);
					return false;
				}

				if (toForeground != null)
				{
					this.Current = null;
				}

				return true;
			}

			public void HandleBackground ()
			{
				if (!this.Task.MoveNext())
				{
					return;
				}

				this.Current = this.Task.Current;

				ToDispatcher toDispatcher = this.Current as ToDispatcher;
				ToHeavyThread toHeavyThread = this.Current as ToHeavyThread;
				ToBackground toBackground = this.Current as ToBackground;
				ToForeground toForeground = this.Current as ToForeground;

				if (toDispatcher != null)
				{
					if (toDispatcher.DispatcherService != null)
					{
						toDispatcher.DispatcherService.Dispatch(toDispatcher.Priority, this.HandleBackground);
					}
					else
					{
						toDispatcher.ThreadDispatcher.Post(new Action<MoveableTask>(x => x.HandleBackground()), this);
					}
					return;
				}

				if (toHeavyThread != null)
				{
					HeavyThreadExecutor heavyThread = new HeavyThreadExecutor(this, toHeavyThread.Priority);
					heavyThread.Start();
					return;
				}

				if (toBackground != null)
				{
					ThreadPool.QueueUserWorkItem(x => ((MoveableTask)x).HandleBackground(), this);
					return;
				}

				lock (ThreadMover.SyncRoot)
				{
					ThreadMover.TasksToMoveToForeground.Add(this);
				}

				if (toForeground == null)
				{
					this.HasInjectedYield = true;
					this.InjectedYield = this.Current;
				}
			}

			public void Reset()
			{
			}
		}

		private sealed class HeavyThreadExecutor : HeavyThread
		{
			public HeavyThreadExecutor (MoveableTask task, ThreadPriority priority)
			{
				if (task == null)
				{
					throw new ArgumentNullException(nameof(task));
				}

				this.Task = task;
				this.Priority = priority;
			}

			public MoveableTask Task { get; }
			public ThreadPriority Priority { get; }

			protected override void OnStart ()
			{
				base.OnStart();

				this.Thread.IsBackground = true;
				this.Thread.Priority = this.Priority;
			}

			protected override void OnRun ()
			{
				base.OnRun();

				this.Task.HandleBackground();

				ThreadPool.QueueUserWorkItem(x => ((HeavyThreadExecutor)x).Stop(), this);
			}
		}
	}
}