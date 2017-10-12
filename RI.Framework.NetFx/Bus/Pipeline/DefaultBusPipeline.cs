using System;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Pipeline
{
	/// <summary>
	/// Implements a default bus processing pipeline which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	/// See <see cref="IBusPipeline"/> for more details.
	/// </remarks>
	public sealed class DefaultBusPipeline : IBusPipeline
	{
		/// <inheritdoc />
		public void Initialize (IDependencyResolver dependencyResolver)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void Unload ()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void StartProcessing (Action doWorkSignaler)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void StopProcessing ()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void DoWork ()
		{
			throw new NotImplementedException();
		}
	}
}
