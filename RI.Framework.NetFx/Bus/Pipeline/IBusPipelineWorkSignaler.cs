using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Pipeline
{
	/// <summary>
	///     Defines the interface for a bus processing pipeline work signaler.
	/// </summary>
	/// <remarks>
	///     See <see cref="IBus" /> for more details about message busses.
	/// </remarks>
	public interface IBusPipelineWorkSignaler
	{
		/// <summary>
		///     Initializes the pipeline work signaler when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Signals the bus processing pipeline that new work is pending (e.g. a message has been received through a bus connection).
		/// </summary>
		void SignalWorkAvailable ();

		/// <summary>
		///     Unloads the pipeline work signaler when the bus stops.
		/// </summary>
		void Unload ();
	}
}
