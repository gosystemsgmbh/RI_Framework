using System;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Describes the current shutdown mode of a <see cref="IThreadDispatcher" />.
	/// </summary>
	[Serializable]
	public enum ThreadDispatcherShutdownMode
	{
		/// <summary>
		///     The dispatcher is not running or not being shut down.
		/// </summary>
		None = 0,

		/// <summary>
		///     The dispatcher is being shut down and all already pending delegates are discarded.
		/// </summary>
		DiscardPending = 1,

		/// <summary>
		///     The dispatcher is being shut down and all already pending delegates are processed before the shutdown completes.
		/// </summary>
		FinishPending = 2,
	}
}
