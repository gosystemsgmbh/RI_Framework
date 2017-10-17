using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Pipeline
{
	/// <summary>
	///     Defines the interface for a bus processing pipeline.
	/// </summary>
	/// <remarks>
	///     See <see cref="IBus" /> for more details about message busses.
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IBusPipeline : ISynchronizable
	{
		/// <summary>
		///     Processes all pending work on the bus processing pipeline.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="DoWork" /> is also called in the interval of <see cref="IBus.PollInterval" /> when no work is available.
		///     </para>
		/// </remarks>
		void DoWork ();

		/// <summary>
		///     Initializes the pipeline when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Starts processing the bus pipeline.
		/// </summary>
		void StartProcessing ();

		/// <summary>
		///     Stops processing the bus pipeline.
		/// </summary>
		void StopProcessing ();

		/// <summary>
		///     Unloads the pipeline when the bus stops.
		/// </summary>
		void Unload ();
	}
}
