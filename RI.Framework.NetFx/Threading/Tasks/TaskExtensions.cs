using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Tasks
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Task" /> type.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public static class TaskExtensions
	{
		#region Static Methods

		/// <summary>
		///     Creates an awaitable which flows the current <see cref="CultureInfo" /> around the execution of a <see cref="Task" />.
		/// </summary>
		/// <param name="task"> The task. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public static CultureFlower WithCurrentCulture (this Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new CultureFlower(task);
		}

		/// <summary>
		///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around the execution of a <see cref="Task" />.
		/// </summary>
		/// <param name="task"> The task. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public static SynchronizationContextFlower WithCurrentSynchronizationContext (this Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new SynchronizationContextFlower(task);
		}

		#endregion
	}
}
