namespace RI.Framework.Utilities.ObjectModel
{
	/// <summary>
	///     Defines an interface to implement detection of synchronization/thread-safety.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public interface ISynchronizable
	{
		/// <summary>
		///     Gets a value indicating whether access to the implementing object is synchronized (thread safe).
		/// </summary>
		/// <value>
		///     true if the access to the implementing object is synchronized (thread safe), false otherwise.
		/// </value>
		bool IsSynchronized { get; }

		/// <summary>
		///     Gets an object that can be used to synchronize access to the implementing object.
		/// </summary>
		/// <value>
		///     An object that can be used to synchronize access to the implementing object.
		/// </value>
		object SyncRoot { get; }
	}
}
