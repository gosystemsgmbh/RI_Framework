using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Implements a default dispatcher service which is suitable for most scenarios.
	/// </summary>
	public sealed class DispatcherService : IDispatcherService
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DispatcherService" />.
		/// </summary>
		public DispatcherService ()
		{
			this.NowTicks = DateTime.UtcNow.Ticks;
			this.FramesWithoutOperations = 0;

			this.PendingOperations = new List<LinkedList<DispatcherOperation>>();
			while (this.PendingOperations.Count <= (int)DispatcherPriority.Lowest)
			{
				this.PendingOperations.Add(new LinkedList<DispatcherOperation>());
			}

			GameObject dispatcherServiceObject = new GameObject(this.GetType().Name);
			dispatcherServiceObject.SetActive(true);
			Object.DontDestroyOnLoad(dispatcherServiceObject);

			DispatcherListener dispatcherServiceListener = dispatcherServiceObject.AddComponent<DispatcherListener>();
			dispatcherServiceListener.DispatcherService = this;

			this.RegisterReceiver<DispatcherBroadcast>(this.HandleDispatcherBroadcast);
		}

		#endregion




		#region Instance Fields

		private int FramesWithoutOperations;

		private long NowTicks;

		private readonly List<LinkedList<DispatcherOperation>> PendingOperations;

		#endregion




		#region Instance Methods

		private DispatcherOperation BroadcastInternal <T> (DispatcherPriority priority, T broadcast) where T : class
		{
			DispatcherOperation operation = new DispatcherOperation();
			operation.Dispatcher = this;
			operation.Invoker = DispatcherSlots<T>.Invoker;
			operation.Priority = priority;
			operation.Broadcast = broadcast;
			operation.StatusInternal = DispatcherStatus.Queued;
			operation.ResultInternal = null;
			operation.TickTrigger = null;

			if (operation.Priority == DispatcherPriority.Now)
			{
				this.Invoke(operation);
				this.FramesWithoutOperations = 0;
			}
			else
			{
				this.PendingOperations[(int)operation.Priority].AddLast(operation);
			}

			return operation;
		}

		private bool Cancel (DispatcherOperation operation)
		{
			if (operation.StatusInternal != DispatcherStatus.Queued)
			{
				return false;
			}

			operation.StatusInternal = DispatcherStatus.Canceled;

			return true;
		}

		private void HandleDispatcherBroadcast (DispatcherBroadcast broadcast)
		{
			broadcast.Invoke();
		}

		private bool Invoke (DispatcherOperation operation)
		{
			if (operation.StatusInternal != DispatcherStatus.Queued)
			{
				return false;
			}

			operation.StatusInternal = DispatcherStatus.Processing;
			operation.Invoker(operation.Broadcast);
			operation.StatusInternal = DispatcherStatus.Processed;

			return true;
		}

		private void InvokePendingOperations ()
		{
			this.NowTicks = DateTime.UtcNow.Ticks;

			bool operationsDispatched = false;

			for (int i1 = (int)DispatcherPriority.Highest; i1 <= (int)DispatcherPriority.Lowest; i1++)
			{
				if (( ( (DispatcherPriority)i1 ) >= DispatcherPriority.Idle ) && ( this.FramesWithoutOperations < 1 ))
				{
					break;
				}

				LinkedList<DispatcherOperation> operations = this.PendingOperations[i1];

				LinkedListNode<DispatcherOperation> currentNode = operations.First;
				LinkedListNode<DispatcherOperation> nextNode = currentNode;

				while (nextNode != null)
				{
					currentNode = nextNode;
					nextNode = currentNode.Next;

					DispatcherOperation operation = currentNode.Value;

					bool isProcessable = true;
					if (operation.TickTrigger.HasValue)
					{
						isProcessable = this.NowTicks >= operation.TickTrigger.Value;
					}

					if (isProcessable)
					{
						if (this.Invoke(operation))
						{
							operationsDispatched = true;
						}
						operations.Remove(currentNode);
					}
				}

				if (operationsDispatched)
				{
					break;
				}
			}

			if (operationsDispatched)
			{
				this.FramesWithoutOperations = 0;
			}
			else
			{
				this.FramesWithoutOperations++;
			}
		}

		private bool Reschedule (DispatcherOperation operation, int millisecondsFromNow)
		{
			if (operation.StatusInternal != DispatcherStatus.Queued)
			{
				return false;
			}

			operation.TickTrigger = this.NowTicks + ( millisecondsFromNow * 10000L );

			return true;
		}

		private bool Reschedule (DispatcherOperation operation, DateTime timestamp)
		{
			return this.Reschedule(operation, Math.Max((int)( ( timestamp.ToUniversalTime().Ticks - this.NowTicks ) / 10000 ), 0));
		}

		#endregion




		#region Interface: IDispatcherService

		/// <inheritdoc />
		public IDispatcherOperation Broadcast <T> (DispatcherPriority priority, T broadcast) where T : class
		{
			if (broadcast == null)
			{
				throw new ArgumentNullException(nameof(broadcast));
			}

			return this.BroadcastInternal(priority, broadcast);
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch (DispatcherPriority priority, Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DispatcherBroadcast broadcast = new DispatcherAction
			{
				Action = action
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T> (DispatcherPriority priority, Action<T> action, T arg)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DispatcherBroadcast broadcast = new DispatcherAction<T>
			{
				Action = action,
				Arg = arg
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2> (DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DispatcherBroadcast broadcast = new DispatcherAction<T1, T2>
			{
				Action = action,
				Arg1 = arg1,
				Arg2 = arg2
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2, T3> (DispatcherPriority priority, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DispatcherBroadcast broadcast = new DispatcherAction<T1, T2, T3>
			{
				Action = action,
				Arg1 = arg1,
				Arg2 = arg2,
				Arg3 = arg3
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2, T3, T4> (DispatcherPriority priority, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DispatcherBroadcast broadcast = new DispatcherAction<T1, T2, T3, T4>
			{
				Action = action,
				Arg1 = arg1,
				Arg2 = arg2,
				Arg3 = arg3,
				Arg4 = arg4
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <TResult> (DispatcherPriority priority, Func<TResult> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			DispatcherBroadcast broadcast = new DispatcherFunc<TResult>
			{
				Func = func
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T, TResult> (DispatcherPriority priority, Func<T, TResult> func, T arg)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			DispatcherBroadcast broadcast = new DispatcherFunc<T, TResult>
			{
				Func = func,
				Arg = arg
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2, TResult> (DispatcherPriority priority, Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			DispatcherBroadcast broadcast = new DispatcherFunc<T1, T2, TResult>
			{
				Func = func,
				Arg1 = arg1,
				Arg2 = arg2
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2, T3, TResult> (DispatcherPriority priority, Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			DispatcherBroadcast broadcast = new DispatcherFunc<T1, T2, T3, TResult>
			{
				Func = func,
				Arg1 = arg1,
				Arg2 = arg2,
				Arg3 = arg3
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public IDispatcherOperation Dispatch <T1, T2, T3, T4, TResult> (DispatcherPriority priority, Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			DispatcherBroadcast broadcast = new DispatcherFunc<T1, T2, T3, T4, TResult>
			{
				Func = func,
				Arg1 = arg1,
				Arg2 = arg2,
				Arg3 = arg3,
				Arg4 = arg4
			};

			DispatcherOperation operation = this.BroadcastInternal(priority, broadcast);
			broadcast.Operation = operation;
			return operation;
		}

		/// <inheritdoc />
		public void RegisterReceiver <T> (Action<T> receiver) where T : class
		{
			if (receiver == null)
			{
				throw new ArgumentNullException(nameof(receiver));
			}

			DispatcherSlots<T>.RegisterReceiver(receiver);
		}

		/// <inheritdoc />
		public void UnregisterReceiver <T> (Action<T> receiver) where T : class
		{
			if (receiver == null)
			{
				throw new ArgumentNullException(nameof(receiver));
			}

			DispatcherSlots<T>.UnregisterReceiver(receiver);
		}

		#endregion




		#region Type: DispatcherAction

		private sealed class DispatcherAction : DispatcherBroadcast
		{
			#region Instance Fields

			public Action Action;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Action();
			}

			#endregion
		}




		private sealed class DispatcherAction <T> : DispatcherBroadcast
		{
			#region Instance Fields

			public Action<T> Action;

			public T Arg;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Action(this.Arg);
			}

			#endregion
		}




		private sealed class DispatcherAction <T1, T2> : DispatcherBroadcast
		{
			#region Instance Fields

			public Action<T1, T2> Action;

			public T1 Arg1;

			public T2 Arg2;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Action(this.Arg1, this.Arg2);
			}

			#endregion
		}




		private sealed class DispatcherAction <T1, T2, T3> : DispatcherBroadcast
		{
			#region Instance Fields

			public Action<T1, T2, T3> Action;

			public T1 Arg1;

			public T2 Arg2;

			public T3 Arg3;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Action(this.Arg1, this.Arg2, this.Arg3);
			}

			#endregion
		}




		private sealed class DispatcherAction <T1, T2, T3, T4> : DispatcherBroadcast
		{
			#region Instance Fields

			public Action<T1, T2, T3, T4> Action;

			public T1 Arg1;

			public T2 Arg2;

			public T3 Arg3;

			public T4 Arg4;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Action(this.Arg1, this.Arg2, this.Arg3, this.Arg4);
			}

			#endregion
		}

		#endregion




		#region Type: DispatcherBroadcast

		private abstract class DispatcherBroadcast
		{
			#region Instance Fields

			public DispatcherOperation Operation;

			#endregion




			#region Abstracts

			public abstract void Invoke ();

			#endregion
		}

		#endregion




		#region Type: DispatcherFunc

		private sealed class DispatcherFunc <TResult> : DispatcherBroadcast
		{
			#region Instance Fields

			public Func<TResult> Func;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Operation.ResultInternal = this.Func();
			}

			#endregion
		}




		private sealed class DispatcherFunc <T, TResult> : DispatcherBroadcast
		{
			#region Instance Fields

			public T Arg;

			public Func<T, TResult> Func;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Operation.ResultInternal = this.Func(this.Arg);
			}

			#endregion
		}




		private sealed class DispatcherFunc <T1, T2, TResult> : DispatcherBroadcast
		{
			#region Instance Fields

			public T1 Arg1;

			public T2 Arg2;

			public Func<T1, T2, TResult> Func;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Operation.ResultInternal = this.Func(this.Arg1, this.Arg2);
			}

			#endregion
		}




		private sealed class DispatcherFunc <T1, T2, T3, TResult> : DispatcherBroadcast
		{
			#region Instance Fields

			public T1 Arg1;

			public T2 Arg2;

			public T3 Arg3;

			public Func<T1, T2, T3, TResult> Func;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Operation.ResultInternal = this.Func(this.Arg1, this.Arg2, this.Arg3);
			}

			#endregion
		}




		private sealed class DispatcherFunc <T1, T2, T3, T4, TResult> : DispatcherBroadcast
		{
			#region Instance Fields

			public T1 Arg1;

			public T2 Arg2;

			public T3 Arg3;

			public T4 Arg4;

			public Func<T1, T2, T3, T4, TResult> Func;

			#endregion




			#region Overrides

			public override void Invoke ()
			{
				this.Operation.ResultInternal = this.Func(this.Arg1, this.Arg2, this.Arg3, this.Arg4);
			}

			#endregion
		}

		#endregion




		#region Type: DispatcherListener

		private sealed class DispatcherListener : MonoBehaviour
		{
			#region Instance Fields

			public DispatcherService DispatcherService;

			#endregion




			#region Instance Methods

			public void LateUpdate ()
			{
				this.StartCoroutine(this.HandleFrameEnd());
			}

			private IEnumerator HandleFrameEnd ()
			{
				yield return new WaitForEndOfFrame();
				this.DispatcherService.InvokePendingOperations();
			}

			#endregion
		}

		#endregion




		#region Type: DispatcherOperation

		private sealed class DispatcherOperation : IDispatcherOperation
		{
			#region Instance Fields

			public object Broadcast;

			public DispatcherService Dispatcher;

			public Action<object> Invoker;

			public DispatcherPriority Priority = DispatcherPriority.Now;

			public object ResultInternal;

			public DispatcherStatus StatusInternal = DispatcherStatus.Queued;

			public long? TickTrigger;

			#endregion




			#region Interface: IDispatcherOperation

			object IDispatcherOperation.Result
			{
				get
				{
					return this.ResultInternal;
				}
			}

			DispatcherStatus IDispatcherOperation.Status
			{
				get
				{
					return this.StatusInternal;
				}
			}

			bool IDispatcherOperation.Cancel ()
			{
				return this.Dispatcher.Cancel(this);
			}

			bool IDispatcherOperation.Reschedule (int millisecondsFromNow)
			{
				return this.Dispatcher.Reschedule(this, millisecondsFromNow);
			}

			bool IDispatcherOperation.Reschedule (DateTime timestamp)
			{
				return this.Dispatcher.Reschedule(this, timestamp);
			}

			#endregion
		}

		#endregion




		#region Type: DispatcherSlots

		private static class DispatcherSlots <T>
				where T : class
		{
			#region Constants

			private static List<Action<T>> Receivers;

			private static Action<T>[] ReceiversSafe;

			public static readonly Action<object> Invoker = DispatcherSlots<T>.Invoke;

			#endregion




			#region Static Fields

			#endregion




			#region Static Methods

			public static void RegisterReceiver (Action<T> receiver)
			{
				if (DispatcherSlots<T>.Receivers == null)
				{
					DispatcherSlots<T>.Receivers = new List<Action<T>>();
				}

				if (DispatcherSlots<T>.Receivers.Contains(receiver))
				{
					return;
				}

				DispatcherSlots<T>.Receivers.Add(receiver);
				DispatcherSlots<T>.ReceiversSafe = null;
			}

			public static void UnregisterReceiver (Action<T> receiver)
			{
				if (DispatcherSlots<T>.Receivers == null)
				{
					return;
				}

				if (!DispatcherSlots<T>.Receivers.Contains(receiver))
				{
					return;
				}

				DispatcherSlots<T>.Receivers.Remove(receiver);
				DispatcherSlots<T>.ReceiversSafe = null;
			}

			private static void Invoke (object broadcast)
			{
				if (DispatcherSlots<T>.ReceiversSafe == null)
				{
					DispatcherSlots<T>.ReceiversSafe = DispatcherSlots<T>.Receivers?.ToArray();
				}

				if (DispatcherSlots<T>.ReceiversSafe == null)
				{
					return;
				}

				T realBroadcast = broadcast as T;

				Action<T>[] receivers = DispatcherSlots<T>.ReceiversSafe;
				for (int i1 = 0; i1 < receivers.Length; i1++)
				{
					Action<T> receiver = receivers[i1];
					receiver(realBroadcast);
				}
			}

			#endregion
		}

		#endregion
	}
}
