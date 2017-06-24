using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Implements the base class for composition catalogs.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Composition catalogs contain exports (types or objects) which can be used by a <see cref="CompositionContainer" /> for composition.
	///     </para>
	///     <para>
	///         The exports managed by a composition catalog are encapsulated using <see cref="CompositionCatalogItem" />.
	///     </para>
	///     <para>
	///         Composition catalogs are used with model-based exporting.
	///         This means that types or objects managed by a composition catalog are exported under the names as defined by the <see cref="ExportAttribute" /> they are decorated with.
	///     </para>
	///     <para>
	///         A composition catalog can be added to multiple <see cref="CompositionContainer" />s.
	///     </para>
	///     <para>
	///         See <see cref="CompositionContainer" /> for more details about composition.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class CompositionCatalog : ILogSource, IDisposable, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionCatalog" />.
		/// </summary>
		protected CompositionCatalog ()
		{
			this.SyncRoot = new object();
			this.Items = new Dictionary<string, List<CompositionCatalogItem>>(CompositionContainer.NameComparer);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="CompositionCatalog" />.
		/// </summary>
		~CompositionCatalog ()
		{
			this.Dispose();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the dictionary with the exports managed by this composition catalog.
		/// </summary>
		/// <value>
		///     The dictionary with the exports managed by this composition catalog.
		/// </value>
		/// <remarks>
		///     <para>
		///         A dictionary key corresponds to the name under which an export is exported.
		///         A dictionary value is a list of all exports which are exported under the associated name.
		///     </para>
		///     <note type="important">
		///         In order to maintain its internal data of available exports in a <see cref="CompositionContainer" />, this property is used every time the composition needs to be updated (e.g. when a composition catalog is added to or removed from a <see cref="CompositionContainer" />, a <see cref="CompositionCatalog" /> explicitly requests an update through <see cref="RequestRecompose" />, etc.).
		///         This means that the update of this property needs to be controlled by the <see cref="CompositionContainer" />.
		///         Therefore, this property should only ever be updated by the <see cref="CompositionCatalog" /> either at construction time of the <see cref="CompositionCatalog" /> (for catalogs where the exports never change and therefore are defined at construction time) or when <see cref="UpdateItems" /> is called (for catalogs which can change their exports during runtime).
		///         Furthermore, if the <see cref="CompositionCatalog" /> itself detects a change of available exports, it must not update this property directly but instead request a proper recomposition from the <see cref="CompositionContainer" /> by calling <see cref="RequestRecompose" /> (which in turn will call <see cref="UpdateItems" />).
		///     </note>
		/// </remarks>
		protected internal Dictionary<string, List<CompositionCatalogItem>> Items { get; private set; }

		/// <summary>
		/// Gets a thread-safe snapshot of the current exports in <see cref="Items"/>.
		/// </summary>
		/// <returns>
		/// The thread-safe snapshot of the current exports in <see cref="Items"/>.
		/// If no exports are available, an empty dictionary is returned.
		/// </returns>
		protected internal Dictionary<string, List<CompositionCatalogItem>> GetItemsSnapshot ()
		{
			lock (this.SyncRoot)
			{
				return new Dictionary<string, List<CompositionCatalogItem>>(this.Items, this.Items.Comparer);
			}
		}

		#endregion




		#region Instance Events

		internal event EventHandler RecomposeRequested;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Requests recomposition from all <see cref="CompositionContainer" />s this <see cref="CompositionCatalog" /> is added to when it detects the change of its exports.
		/// </summary>
		/// <remarks>
		///     <para>
		///         See <see cref="Items" /> and <see cref="UpdateItems" /> for more details.
		///     </para>
		/// </remarks>
		protected void RequestRecompose ()
		{
			this.RecomposeRequested?.Invoke(this, EventArgs.Empty);
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called by the <see cref="CompositionContainer" /> when it is required to have <see cref="Items" /> up-to-date.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="UpdateItems" /> is called independently for each <see cref="CompositionContainer" /> this <see cref="CompositionCatalog" /> is added to.
		///     </para>
		///     <note type="important">
		///         <see cref="UpdateItems" /> can be repeatedly called, depending on the operations of the <see cref="CompositionContainer" />.
		///         Therefore, overloading <see cref="UpdateItems" /> shall not do anything in cases the exports have not changed.
		///         This requires proper determination on each <see cref="UpdateItems" /> call whether <see cref="Items" /> is up-to-date or not.
		///     </note>
		/// </remarks>
		protected internal virtual void UpdateItems ()
		{
		}

		/// <inheritdoc cref="IDisposable.Dispose"/>
		protected virtual void Dispose ()
		{
		}

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Dispose();
		}

		#endregion




		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }
	}
}
