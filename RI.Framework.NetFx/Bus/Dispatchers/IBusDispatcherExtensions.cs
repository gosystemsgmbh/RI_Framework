using System;
using System.Threading.Tasks;

namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IBusDispatcher" /> type.
	/// </summary>
	public static class IBusDispatcherExtensions
	{
		/// <summary>
		/// Dispatches a delegate for execution and retuns a task which allows continuation after the delegate was executed.
		/// </summary>
		/// <param name="dispatcher">The bus dispatcher.</param>
		/// <param name="action">The delegate.</param>
		/// <param name="parameters">Optional parameters of the delegate.</param>
		/// <returns>
		/// The task which can be used to continue after the delegate was executed.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> or <paramref name="action"/> is null.</exception>
		public static Task<object> DispatchAsync (this IBusDispatcher dispatcher, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			parameters = parameters ?? new object[0];

			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

			dispatcher.Dispatch(new Action<TaskCompletionSource<object>,Delegate,object[]>((t,d,p) =>
			{
				object result = d.DynamicInvoke(p);
				t.TrySetResult(result);
			}), tcs, action, parameters);

			return tcs.Task;
		}
	}
}
