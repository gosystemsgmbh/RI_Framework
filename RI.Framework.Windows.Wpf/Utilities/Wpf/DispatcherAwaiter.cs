using System;

using RI.Framework.Threading.Async;




namespace RI.Framework.Utilities.Wpf
{
	/// <summary>
	///     Implements an awaiter which continues on a specified <see cref="System.Windows.Threading.Dispatcher" />.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public sealed class DispatcherAwaiter : CustomAwaiter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DispatcherAwaiter" />.
		/// </summary>
		/// <param name="dispatcher"> The used <see cref="System.Windows.Threading.Dispatcher" />. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public DispatcherAwaiter (System.Windows.Threading.Dispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.Dispatcher = dispatcher;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used <see cref="System.Windows.Threading.Dispatcher" />.
		/// </summary>
		/// <value>
		///     The used <see cref="System.Windows.Threading.Dispatcher" />.
		/// </value>
		public System.Windows.Threading.Dispatcher Dispatcher { get; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override void OnCompleted (Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException(nameof(continuation));
			}

			this.Dispatcher.BeginInvoke(continuation);
		}

		#endregion
	}
}
