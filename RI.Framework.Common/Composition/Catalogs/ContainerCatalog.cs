using System;

using RI.Framework.Collections;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which wraps around a composition container, dynamically exposing its exports as a composition catalog.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ContainerCatalog" /> allows for a more dynamic linking between multiple <see cref="CompositionContainer" />s than with parent/child containers because a catalog (an thus also <see cref="ContainerCatalog" />) can be added and removed dynamically.
	///     </para>
	///     <note type="important">
	///         <see cref="ContainerCatalog" /> does not support circular linking, e.g. wrapping two <see cref="CompositionContainer" />s in a <see cref="ContainerCatalog" /> and add them to each other.
	///         Doing so ends up in undefined behaviour and most likely in a <see cref="StackOverflowException" />.
	///     </note>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ContainerCatalog : CompositionCatalog
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ContainerCatalog" />.
		/// </summary>
		/// <param name="container"> The composition container to wrap. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public ContainerCatalog (CompositionContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.ContainerChangedHandler = this.ContainerChanged;

			this.Container = container;
			this.Container.CompositionChanged += this.ContainerChangedHandler;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the wrapped composition container.
		/// </summary>
		/// <value>
		///     The wrapped composition container.
		/// </value>
		public CompositionContainer Container { get; private set; }

		private EventHandler ContainerChangedHandler { get; }

		#endregion




		#region Instance Methods

		private void ContainerChanged (object sender, EventArgs args)
		{
			this.RequestRecompose();
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				if (this.Container != null)
				{
					this.Container.CompositionChanged -= this.ContainerChangedHandler;
					this.Container = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <inheritdoc />
		protected internal override void UpdateItems ()
		{
			base.UpdateItems();

			lock (this.SyncRoot)
			{
				this.Items.Clear();

				if (this.Container == null)
				{
					return;
				}

				this.Items.AddRange(this.Container.GetCompositionSnapshot());
			}
		}

		#endregion
	}
}
