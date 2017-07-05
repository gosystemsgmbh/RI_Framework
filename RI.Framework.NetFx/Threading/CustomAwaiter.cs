using System;
using System.Runtime.CompilerServices;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements a base class to implement custom awaiters.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public abstract class CustomAwaiter : INotifyCompletion, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CustomAwaiter" />.
		/// </summary>
		protected CustomAwaiter ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Gets whether the continuation action has already completed.
		/// </summary>
		/// <value>
		///     true if the continuation action has already completed and does not need to be scheduled, false otherwise.
		/// </value>
		public virtual bool IsCompleted => false;

		/// <summary>
		///     Gets the awaiter, as used by the compiler with the async/await pattern.
		/// </summary>
		/// <returns>
		///     The awaiter, which is nothing else than this instance itself.
		/// </returns>
		public virtual CustomAwaiter GetAwaiter () => this;

		/// <summary>
		///     Gets the result of the scheduled continuation action.
		/// </summary>
		public virtual void GetResult ()
		{
		}

		#endregion




		#region Interface: INotifyCompletion

		/// <summary>
		///     Schedules a continuation action.
		/// </summary>
		/// <param name="continuation"> The continuation action to schedule. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="continuation" /> is null. </exception>
		public abstract void OnCompleted (Action continuation);

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		public bool IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion
	}
}
