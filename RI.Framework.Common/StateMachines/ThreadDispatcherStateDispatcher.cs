using System;

using RI.Framework.Utilities.Threading;




namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Implements a state machine operation dispatcher which uses a <see cref="IThreadDispatcher"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IStateDispatcher"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class ThreadDispatcherStateDispatcher : IStateDispatcher
	{
		/// <summary>
		/// Gets the used dispatcher.
		/// </summary>
		/// <value>
		/// The used dispatcher.
		/// </value>
		public IThreadDispatcher ThreadDispatcher { get; private set; }

		/// <summary>
		/// Creates a new instance of <see cref="ThreadDispatcherStateDispatcher"/>.
		/// </summary>
		/// <param name="threadDispatcher">The used dispatcher.</param>
		/// <exception cref="ArgumentNullException"><paramref name="threadDispatcher"/> is null.</exception>
		public ThreadDispatcherStateDispatcher(IThreadDispatcher threadDispatcher)
		{
			if (threadDispatcher == null)
			{
				throw new ArgumentNullException(nameof(threadDispatcher));
			}

			this.ThreadDispatcher = threadDispatcher;
		}

		/// <inheritdoc />
		public void DispatchTransition(StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.ThreadDispatcher.Post(new Action<StateMachineTransientDelegate, StateTransientInfo>((x, y) => x(y)), transientDelegate, transientInfo);
		}

		/// <inheritdoc />
		public void DispatchSignal(StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			this.ThreadDispatcher.Post(new Action<StateMachineSignalDelegate, StateSignalInfo>((x, y) => x(y)), signalDelegate, signalInfo);
		}
	}
}
