using System;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Pipeline
{
	/// <summary>
	/// Defines the interface for a bus processing pipeline.
	/// </summary>
	/// <remarks>
	/// See <see cref="IBus"/> for more details about message busses.
	/// </remarks>
	public interface IBusPipeline
	{
		/// <summary>
		/// Initializes the pipeline when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which can be used to get instances of required types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		void Initialize(IDependencyResolver dependencyResolver);

		/// <summary>
		/// Unloads the pipeline when the bus stops.
		/// </summary>
		void Unload();

		/// <summary>
		/// Starts processing the bus pipeline.
		/// </summary>
		/// <param name="doWorkSignaler">The callback which can be used by the processing pipeline to trigger an immediate execution of <see cref="DoWork"/>.</param>
		/// <remarks>
		/// <note type="note">
		/// <see cref="StartProcessing"/> is supposed to be always called from the same thread as <see cref="DoWork"/> is called.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="doWorkSignaler"/> is null.</exception>
		void StartProcessing (Action doWorkSignaler);

		/// <summary>
		/// Stops processing the bus pipeline.
		/// </summary>
		/// <remarks>
		/// <note type="note">
		/// <see cref="StopProcessing"/> is supposed to be always called from the same thread as <see cref="DoWork"/> is called.
		/// </note>
		/// </remarks>
		void StopProcessing ();

		/// <summary>
		/// Processes all outstanding work on the bus processing pipeline.
		/// </summary>
		/// <remarks>
		/// <note type="note">
		/// <see cref="DoWork"/> is supposed to be called by the used implementation of <see cref="IBus"/> in the interval of <see cref="IBus.PollInterval"/> or immediately when the callback provided to <see cref="StartProcessing"/> is called.
		/// </note>
		/// <note type="note">
		/// <see cref="DoWork"/> is supposed to be always called from the same dedicated thread managed by the used implementation of <see cref="IBus"/>.
		/// </note>
		/// </remarks>
		void DoWork ();
	}
}
