using System;
using System.Threading;

namespace RI.Framework.Threading
{
	/// <summary>
	/// Describes execution options for enqueued delegates.
	/// </summary>
	[Serializable]
	[Flags]
	public enum ThreadDispatcherOptions
	{
		/// <summary>
		/// No options specified. That is: Nothing flows.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// <see cref="Thread.CurrentCulture"/> flows.
		/// </summary>
		FlowCurrentCulture = 0x01,

		/// <summary>
		/// <see cref="Thread.CurrentUICulture"/> flows.
		/// </summary>
		FlowCurrentUICulture = 0x02,

		/// <summary>
		/// <see cref="SynchronizationContext.Current"/> flows.
		/// </summary>
		FlowSynchronizationContext = 0x04,

		/// <summary>
		/// <see cref="ExecutionContext"/> flows.
		/// </summary>
		FlowExecutionContext = 0x08,

		/// <summary>
		/// <see cref="Thread.CurrentCulture"/> and <see cref="Thread.CurrentUICulture"/> flows.
		/// </summary>
		FlowCulture = ThreadDispatcherOptions.FlowCurrentCulture | ThreadDispatcherOptions.FlowCurrentUICulture,

		/// <summary>
		/// Everything flows.
		/// </summary>
		FlowAll = ThreadDispatcherOptions.FlowCulture | ThreadDispatcherOptions.FlowSynchronizationContext | ThreadDispatcherOptions.FlowExecutionContext,
	}
}
