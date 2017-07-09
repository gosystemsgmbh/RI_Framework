using System;

using RI.Framework.Services.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition
{
	public abstract class CompositionCreator : ILogSource, IDisposable, ISynchronizable
	{
		/// <summary>
		///     Creates a new instance of <see cref="CompositionCreator" />.
		/// </summary>
		protected CompositionCreator()
		{
			this.SyncRoot = new object();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="CompositionCreator" />.
		/// </summary>
		~CompositionCreator()
		{
			this.Dispose(false);
		}



		#region Interface: IDisposable


		/// <summary>
		///     Disposes this catalog and frees all resources.
		/// </summary>
		/// <param name="disposing"> true if called from <see cref="IDisposable.Dispose" />, false if called from the destructor. </param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <inheritdoc />
		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion




		internal protected abstract bool CanCreateInstance (Type type, Type compatibleType, string exportName);

		internal protected abstract object CreateInstance (Type type, Type compatibleType, string exportName);
	}
}
