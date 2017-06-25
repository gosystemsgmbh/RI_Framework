using System.Collections.Generic;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Used as a proxy in model-based importing (using <see cref="ImportAttribute" />) to hold multiple imported values in an AOT-compatible way.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ImportExtensions" /> must be used to access the actual imported values.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class Import : ISynchronizable
	{
		#region Instance Constructor/Destructor

		internal Import (object[] instances)
		{
			this.SyncRoot = new object();
			this.Instances = instances;
		}

		#endregion




		#region Instance Properties/Indexer

		internal object[] Instances { get; }

		#endregion




		#region Instance Methods

		internal List<object> GetInstancesSnapshot ()
		{
			lock (this.SyncRoot)
			{
				if (this.Instances == null)
				{
					return null;
				}

				return new List<object>(this.Instances);
			}
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion
	}
}
