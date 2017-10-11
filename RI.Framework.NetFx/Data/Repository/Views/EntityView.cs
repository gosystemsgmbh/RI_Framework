using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using RI.Framework.Collections;




namespace RI.Framework.Data.Repository.Views
{
	/// <summary>
	///     Provides a view onto data which is associated with an <see cref="IRepositorySet{T}" />.
	/// </summary>
	/// <typeparam name="TEntity"> The type of entities this view presents. </typeparam>
	/// <typeparam name="TViewObject"> The type of <see cref="EntityViewObject{TEntity}" /> this view uses to wrap its entity items. </typeparam>
	/// <remarks>
	///     <para>
	///         An <see cref="EntityView{TEntity,TViewObject}" /> can be used to provide a workable view onto data which is delivered using a <see cref="IRepositorySet{T}" />.
	///         The items of a <see cref="IRepositorySet{T}" /> are processed and wrapped in an <see cref="EntityViewObject{TEntity}" /> instance to provide additional view-related functionality for each item.
	///         Therefore, an <see cref="EntityView{TEntity,TViewObject}" /> can be seen as an additional layer on top of a <see cref="IRepositorySet{T}" /> which can be used to paginate, filter, and manage the items of the <see cref="IRepositorySet{T}" />.
	///     </para>
	///     <para>
	///         <see cref="EntityView{TEntity,TViewObject}" /> hides some of the complexity of <see cref="IRepositorySet{T}" /> by presenting the items through simple collections (<see cref="Entities" /> and <see cref="ViewObjects" />).
	///     </para>
	///     <para>
	///         <see cref="EntityView{TEntity,TViewObject}" /> either uses all of the associated <see cref="IRepositorySet{T}" />s items if <see cref="Source" /> is not set, or only uses the items provided by <see cref="Source" />, if set.
	///         This allows to control the final set of items an <see cref="EntityView{TEntity,TViewObject}" /> is using, as long as the items are associated with the specified <see cref="IRepositorySet{T}" />.
	///     </para>
	/// </remarks>
	/// TODO: View interfaces for WF and WPF
	public class EntityView <TEntity, TViewObject> : INotifyPropertyChanged, IEntityViewCaller<TEntity>
		where TEntity : class, new()
		where TViewObject : EntityViewObject<TEntity>, new()
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityView{TEntity,TViewObject}" />.
		/// </summary>
		/// <param name="set"> The <see cref="IRepositorySet{T}" /> this view is associated with. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="set" /> is null. </exception>
		public EntityView (IRepositorySet<TEntity> set)
			: this(set, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="EntityView{TEntity,TViewObject}" />.
		/// </summary>
		/// <param name="set"> The <see cref="IRepositorySet{T}" /> this view is associated with. </param>
		/// <param name="source"> Specifies a value for <see cref="Source" /> (can be null). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="set" /> is null. </exception>
		public EntityView (IRepositorySet<TEntity> set, IEnumerable<TEntity> source)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.IsInitializing = true;

			this.Set = set;
			this.Source = source;

			this.EntitiesChangedHandler = this.EntitiesChangedMethod;
			this.ViewObjectsChangedHandler = this.ViewObjectsChangedMethod;
			this.SourceChangedHandler = this.SourceChangedMethod;

			this.IsUpdating = false;
			this.SuppressEntitiesChangeHandling = false;
			this.SuppressViewObjectsChangeHandling = false;
			this.SuppressSourceChangeHandling = false;
			this.EntityToAddIsAttached = false;

			this.CollectionUpdateStrategy = EntityViewCollectionUpdateStrategy.Recreate;

			this.AllowEdit = true;
			this.AllowSelect = true;
			this.ObserveSource = true;
			this.UpdateSource = true;

			this.Entities = null;
			this.ViewObjects = null;

			this.EntityTotalCount = 0;
			this.EntityFilteredCount = 0;
			this.PageTotalCount = 0;
			this.PageFilteredCount = 0;

			this.Filter = null;
			this.Sort = null;
			this.PageSize = 0;
			this.PageNumber = 0;

			this.IsInitializing = false;

			this.Update(false);
		}

		#endregion




		#region Instance Fields

		private bool _allowEdit;

		private bool _allowSelect;

		private EntityViewCollectionUpdateStrategy _collectionUpdateStrategy;

		private object _filter;

		private bool _isUpdating;

		private bool _observeSource;

		private int _pageNumber;

		private int _pageSize;

		private object _sort;

		private IEnumerable<TEntity> _source;

		private bool _updateSource;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether the items managed by this view can be edited or not.
		/// </summary>
		/// <value>
		///     true if the items can be edited, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool AllowEdit
		{
			get
			{
				return this._allowEdit;
			}
			set
			{
				this._allowEdit = value;
				this.OnPropertyChanged(nameof(this.AllowEdit));
			}
		}

		/// <summary>
		///     Gets or sets whether the items managed by this view can be selected or not.
		/// </summary>
		/// <value>
		///     true if the items can be selected, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool AllowSelect
		{
			get
			{
				return this._allowSelect;
			}
			set
			{
				this._allowSelect = value;
				this.OnPropertyChanged(nameof(this.AllowSelect));
			}
		}

		/// <summary>
		///     Gets or sets the collection update strategy for <see cref="Entities" /> and <see cref="ViewObjects" />.
		/// </summary>
		/// <value>
		///     The collection update strategy for <see cref="Entities" /> and <see cref="ViewObjects" />.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="EntityViewCollectionUpdateStrategy.Recreate" />.
		///     </para>
		/// </remarks>
		public EntityViewCollectionUpdateStrategy CollectionUpdateStrategy
		{
			get
			{
				return this._collectionUpdateStrategy;
			}
			set
			{
				this._collectionUpdateStrategy = value;
				this.Update(false);
			}
		}

		/// <summary>
		///     Gets or sets the entity which is currently being edited.
		/// </summary>
		/// <value>
		///     The entity which is currently being edited or null if no entity is to be edited.
		/// </value>
		/// <remarks>
		///     <para>
		///         Multiple entities can be edited at the same time.
		///         When reading, this property only returns the first edited entity found.
		///         When setting, this property calls <see cref="EntityViewObject{TEntity}.EndEdit" /> for all currently edited entities and <see cref="EntityViewObject{TEntity}.BeginEdit" /> for the entity to be edited (if any).
		///     </para>
		/// </remarks>
		public TEntity EditedEntity
		{
			get
			{
				return (from x in this.ViewObjects where x.IsEdited select x.Entity).FirstOrDefault();
			}
			set
			{
				foreach (TViewObject viewObj in from x in this.ViewObjects where x.IsEdited && (!object.ReferenceEquals(x.Entity, value)) select x)
				{
					viewObj.EndEdit();
				}
				if (value != null)
				{
					this.BeginEdit(value);
				}
			}
		}

		/// <summary>
		///     Gets or sets the view object which is currently being edited.
		/// </summary>
		/// <value>
		///     The view object which is currently being edited or null if no view object is to be edited.
		/// </value>
		/// <remarks>
		///     <para>
		///         Multiple view objects can be edited at the same time.
		///         When reading, this property only returns the first edited view object found.
		///         When setting, this property calls <see cref="EntityViewObject{TEntity}.EndEdit" /> for all currently edited view objects and <see cref="EntityViewObject{TEntity}.BeginEdit" /> for the view object to be edited (if any).
		///     </para>
		/// </remarks>
		public TViewObject EditedViewObject
		{
			get
			{
				return (from x in this.ViewObjects where x.IsEdited select x).FirstOrDefault();
			}
			set
			{
				foreach (TViewObject viewObj in from x in this.ViewObjects where x.IsEdited && (!object.ReferenceEquals(x, value)) select x)
				{
					viewObj.EndEdit();
				}
				if (value != null)
				{
					this.BeginEdit(value.Entity);
				}
			}
		}

		/// <summary>
		///     Gets the observable collection of entities presented by this view.
		/// </summary>
		/// <value>
		///     The observable collection of entities presented by this view.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         <see cref="Entities" /> gets a new instance of <see cref="ObservableCollection{T}" /> each time <see cref="Update()" /> is called.
		///         See <see cref="Update()" /> for more details.
		///     </note>
		/// </remarks>
		public ObservableCollection<TEntity> Entities { get; private set; }

		/// <summary>
		///     Gets the number of items actually presented by this view, after <see cref="Filter" /> is applied to the data source.
		/// </summary>
		/// <value>
		///     The number of items actually presented by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="EntityFilteredCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int EntityFilteredCount { get; private set; }

		/// <summary>
		///     Gets the total number of items presented by this view, before <see cref="Filter" /> is applied to the data source.
		/// </summary>
		/// <value>
		///     The total number of items presented by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="EntityTotalCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int EntityTotalCount { get; private set; }

		/// <summary>
		///     Gets or sets the entity filter.
		/// </summary>
		/// <value>
		///     The entity filter.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Changing <see cref="Filter" /> calls <see cref="Update()" /> and resets <see cref="PageNumber" /> to 1 (if current page number is higher than 1).
		///     </note>
		///     <para>
		///         The entity filter is an arbitrary filter object which is used to filter the data source before its entities are presented by the view.
		///         Note that the filter is applied before pagination is performed.
		///     </para>
		///     <para>
		///         The filter object itself is used by <see cref="IRepositorySet{T}.GetFiltered(object, object, int, int, out int, out int, out int)" /> and therefore depends on its implementation.
		///         The filter object itself is not directly used by the view.
		///     </para>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public object Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
				this.Update(true);
			}
		}

		/// <summary>
		///     Gets whether the view is currently being updated.
		/// </summary>
		/// <value>
		///     true if the view is being updated, false otherwise.
		/// </value>
		public bool IsUpdating
		{
			get
			{
				return this._isUpdating;
			}
			private set
			{
				this._isUpdating = value;
				this.OnPropertyChanged(nameof(this.IsUpdating));
			}
		}

		/// <summary>
		///     Gets or sets whether changes in <see cref="Source" /> should be observed and replicated into <see cref="Entities" /> and <see cref="ViewObjects" />.
		/// </summary>
		/// <value>
		///     true if changes in <see cref="Source" /> should be replicated, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		///     <para>
		///         In order to be observable, the source must implement <see cref="INotifyCollectionChanged" />.
		///     </para>
		/// </remarks>
		public bool ObserveSource
		{
			get
			{
				return this._observeSource;
			}
			set
			{
				this._observeSource = value;
				this.OnPropertyChanged(nameof(this.ObserveSource));
			}
		}

		/// <summary>
		///     Gets the number of pages actually available by this view, before <see cref="Filter" /> is applied to the data source.
		/// </summary>
		/// <value>
		///     The total number of pages available by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="PageFilteredCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int PageFilteredCount { get; private set; }

		/// <summary>
		///     Gets or sets the current page number.
		/// </summary>
		/// <value>
		///     The current page number.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Changing <see cref="PageNumber" /> calls <see cref="Update()" />.
		///     </note>
		///     <note type="important">
		///         <see cref="PageNumber" /> is one-indexed, so the first page has the number 1 and not 0.
		///     </note>
		///     <para>
		///         The page number 0 is the &quot;empty&quot; page which always exists and is always empty, regardless of the actual number of items available from the data source.
		///     </para>
		///     <para>
		///         The page number 1 is the first page and always exists, even if the data source provides no entities (in which case the first page is empty).
		///     </para>
		///     <para>
		///         The page number is reset to 1 (if it is not already 0 or 1), when one of the following properties is changed: <see cref="Filter" />, <see cref="PageSize" />, <see cref="Source" />.
		///     </para>
		///     <para>
		///         The default value is 0.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero, is higher than <see cref="PageTotalCount" /> or is higher than one when no pages are available. </exception>
		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				if ((value > 1) && (value > this.PageTotalCount))
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this._pageNumber = value;
				this.Update(false);
			}
		}

		/// <summary>
		///     Gets or sets the current page size.
		/// </summary>
		/// <value>
		///     The current page size.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Changing <see cref="PageSize" /> calls <see cref="Update()" /> and resets <see cref="PageNumber" /> to 1 (if current page number is higher than 1).
		///     </note>
		///     <para>
		///         A page size of 0 will set the page size to infinite so that all entities available by the data source fit in one page (so there will be exactly one page available).
		///     </para>
		///     <para>
		///         The default value is 0.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this._pageSize = value;
				this.Update(true);
			}
		}

		/// <summary>
		///     Gets the total number of pages available by this view, if no <see cref="Filter" /> would be set.
		/// </summary>
		/// <value>
		///     The total number of pages which could be available by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="PageTotalCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int PageTotalCount { get; private set; }

		/// <summary>
		///     Gets or sets the entity which is currently selected.
		/// </summary>
		/// <value>
		///     The entity which is currently selected or null if no entity is to be selected.
		/// </value>
		/// <remarks>
		///     <para>
		///         Multiple entities can be selected at the same time.
		///         When reading, this property only returns the first selected entity found.
		///         When setting, this property calls <see cref="EntityViewObject{TEntity}.Deselect" /> for all currently selected entities and <see cref="EntityViewObject{TEntity}.Select" /> for the entity to be selected (if any).
		///     </para>
		/// </remarks>
		public TEntity SelectedEntity
		{
			get
			{
				return (from x in this.ViewObjects where x.IsSelected select x.Entity).FirstOrDefault();
			}
			set
			{
				foreach (TViewObject viewObj in from x in this.ViewObjects where x.IsSelected && (!object.ReferenceEquals(x.Entity, value)) select x)
				{
					viewObj.Deselect();
				}
				if (value != null)
				{
					this.Select(value);
				}
			}
		}

		/// <summary>
		///     Gets or sets the view object which is currently selected.
		/// </summary>
		/// <value>
		///     The view object which is currently selected or null if no view object is to be selected.
		/// </value>
		/// <remarks>
		///     <para>
		///         Multiple view objects can be selected at the same time.
		///         When reading, this property only returns the first selected view object found.
		///         When setting, this property calls <see cref="EntityViewObject{TEntity}.Deselect" /> for all currently selected view objects and <see cref="EntityViewObject{TEntity}.Select" /> for the view object to be selected (if any).
		///     </para>
		/// </remarks>
		public TViewObject SelectedViewObject
		{
			get
			{
				return (from x in this.ViewObjects where x.IsSelected select x).FirstOrDefault();
			}
			set
			{
				foreach (TViewObject viewObj in from x in this.ViewObjects where x.IsSelected && (!object.ReferenceEquals(x, value)) select x)
				{
					viewObj.Deselect();
				}
				if (value != null)
				{
					this.Select(value.Entity);
				}
			}
		}

		/// <summary>
		///     Gets the <see cref="IRepositorySet{T}" /> this view is associated with.
		/// </summary>
		/// <value>
		///     The <see cref="IRepositorySet{T}" /> this view is associated with.
		/// </value>
		public IRepositorySet<TEntity> Set { get; private set; }

		/// <summary>
		///     Gets or sets the entity sorting.
		/// </summary>
		/// <value>
		///     The entity sorting.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Changing <see cref="Sort" /> calls <see cref="Update()" /> and resets <see cref="PageNumber" /> to 1 (if current page number is higher than 1).
		///     </note>
		///     <para>
		///         The entity sorting is an arbitrary sorting object which is used to sort the data source before its entities are presented by the view.
		///         Note that the sorting is applied before pagination is performed.
		///     </para>
		///     <para>
		///         The sorting object itself is used by <see cref="IRepositorySet{T}.GetFiltered(object, object, int, int, out int, out int, out int)" /> and therefore depends on its implementation.
		///         The sorting object itself is not directly used by the view.
		///     </para>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public object Sort
		{
			get
			{
				return this._sort;
			}
			set
			{
				this._sort = value;
				this.Update(true);
			}
		}

		/// <summary>
		///     Gets or sets the data source presented by this view.
		/// </summary>
		/// <value>
		///     The data source presented by this view.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Changing <see cref="Source" /> calls <see cref="Update()" /> and resets <see cref="PageNumber" /> to 1 (if current page number is higher than 1).
		///     </note>
		///     <para>
		///         The entities of <see cref="Source" /> must be managed by the repository set specified by <see cref="Set" />.
		///     </para>
		///     <para>
		///         If no data source is specified (<see cref="Source" /> is set to null), all entities managed by <see cref="Set" /> are presented by this view.
		///     </para>
		///     <para>
		///         For change replication between <see cref="Source" /> and this view, see <see cref="ObserveSource" /> and <see cref="UpdateSource" />.
		///     </para>
		/// </remarks>
		public IEnumerable<TEntity> Source
		{
			get
			{
				return this._source;
			}
			set
			{
				if (this._source is INotifyCollectionChanged)
				{
					((INotifyCollectionChanged)this._source).CollectionChanged -= this.SourceChangedHandler;
				}

				List<TEntity> oldItems = this._source?.ToList() ?? new List<TEntity>();

				this._source = value;

				List<TEntity> newItems = this._source?.ToList() ?? new List<TEntity>();

				if (this._source is INotifyCollectionChanged)
				{
					((INotifyCollectionChanged)this._source).CollectionChanged += this.SourceChangedHandler;
				}

				this.OnSourceChanged(oldItems, newItems);

				this.Update(true);
			}
		}

		/// <summary>
		///     Gets or sets whether changes in <see cref="Entities" /> and <see cref="ViewObjects" /> should be replicated into <see cref="Source" />.
		/// </summary>
		/// <value>
		///     true if changes in <see cref="Entities" /> and <see cref="ViewObjects" /> should be replicated, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		///     <para>
		///         In order to be updateable, the source must implement <see cref="ICollection{TEntity}" />.
		///     </para>
		/// </remarks>
		public bool UpdateSource
		{
			get
			{
				return this._updateSource;
			}
			set
			{
				this._updateSource = value;
				this.OnPropertyChanged(nameof(this.UpdateSource));
			}
		}

		/// <summary>
		///     Gets the observable collection of view objects presented by this view.
		/// </summary>
		/// <value>
		///     The observable collection of view objects presented by this view.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         <see cref="ViewObjects" /> gets a new instance of <see cref="ObservableCollection{T}" /> each time <see cref="Update()" /> is called.
		///         See <see cref="Update()" /> for more details.
		///     </note>
		/// </remarks>
		public ObservableCollection<TViewObject> ViewObjects { get; private set; }

		/// <summary>
		///     Gets whether the current entity being added to <see cref="Entities" /> or <see cref="ViewObjects" /> is attached instead of added (from the <see cref="Set" />s point-of-view).
		/// </summary>
		/// <value>
		///     true if the entity is attaced, false if it is added.
		/// </value>
		protected bool EntityToAddIsAttached { get; private set; }

		/// <summary>
		///     Gets whether the view is currently being initialized.
		/// </summary>
		/// <value>
		///     true if the view is being initialized, false otherwise.
		/// </value>
		protected bool IsInitializing { get; private set; }

		/// <summary>
		///     Gets whether change handling of <see cref="Entities" /> is currently suppressed.
		/// </summary>
		/// <value>
		///     true if change handling of <see cref="Entities" /> is currently suppressed, false otherwise.
		/// </value>
		protected bool SuppressEntitiesChangeHandling { get; private set; }

		/// <summary>
		///     Gets whether change handling of <see cref="Source" /> is currently suppressed.
		/// </summary>
		/// <value>
		///     true if change handling of <see cref="Source" /> is currently suppressed, false otherwise.
		/// </value>
		protected bool SuppressSourceChangeHandling { get; private set; }

		/// <summary>
		///     Gets whether change handling of <see cref="ViewObjects" /> is currently suppressed.
		/// </summary>
		/// <value>
		///     true if change handling of <see cref="ViewObjects" /> is currently suppressed, false otherwise.
		/// </value>
		protected bool SuppressViewObjectsChangeHandling { get; private set; }

		private NotifyCollectionChangedEventHandler EntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler SourceChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler ViewObjectsChangedHandler { get; set; }

		#endregion




		#region Instance Events

		/// <summary>
		///     Raised after <see cref="Entities" /> has changed.
		/// </summary>
		public event EventHandler<EntityViewItemsEventArgs<TEntity>> EntitiesChanged;

		/// <summary>
		///     Raised after an entity has started being edited.
		/// </summary>
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityBeginEdit;

		/// <summary>
		///     Raised after an entity has canceled its editing.
		/// </summary>
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityCancelEdit;

		/// <summary>
		///     Raised after an entity was deselected.
		/// </summary>
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityDeselected;

		/// <summary>
		///     Raised after an entity has finished its editing.
		/// </summary>
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityEndEdit;

		/// <summary>
		///     Raised after an entity was selected.
		/// </summary>
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntitySelected;

		/// <summary>
		///     Raised after <see cref="Source" /> has changed.
		/// </summary>
		public event EventHandler<EntityViewItemsEventArgs<TEntity>> SourceChanged;

		/// <summary>
		///     Raised after <see cref="Update()" /> finished all processing.
		/// </summary>
		public event EventHandler<EntityViewUpdateEventArgs> Updated;

		/// <summary>
		///     Raised before <see cref="Update()" /> starts any processing.
		/// </summary>
		public event EventHandler<EntityViewUpdateEventArgs> Updating;

		/// <summary>
		///     Raised after <see cref="ViewObjects" /> has changed.
		/// </summary>
		public event EventHandler<EntityViewItemsEventArgs<TViewObject>> ViewObjectsChanged;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets the view object for the specified entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     The view object which is used to wrap the specified entity or null if the entity is currently not presented by the view.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public TViewObject GetViewObjectForEntity (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return (from x in this.ViewObjects where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}

		/// <summary>
		///     Rebuilds and updates the view.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="Update()" /> should only be called if fundamental characteristics of the view have changed and which are not automatically handled by the view itself.
		///     </para>
		///     <para>
		///         <see cref="Update()" /> is automatically called by changing the following properties: <see cref="Filter" />, <see cref="Sort" />, <see cref="PageNumber" />, <see cref="PageSize" />, <see cref="Source" />.
		///     </para>
		/// </remarks>
		public void Update ()
		{
			this.Update(false);
		}

		private void EntitiesChangedMethod (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if (this.SuppressEntitiesChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressEntitiesChangeHandling = true;

				switch (e.Action)
				{
					default:
					{
						throw new NotSupportedException(e.Action + " not supported.");
					}

					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Replace:
					{
						List<TEntity> oldItems = new List<TEntity>();
						List<TEntity> newItems = new List<TEntity>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<TEntity>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<TEntity>());
						}

						foreach (TEntity entity in oldItems)
						{
							TViewObject viewObj = this.GetViewObjectForEntity(entity);

							if (!this.SuppressViewObjectsChangeHandling)
							{
								this.ViewObjects.Remove(viewObj);
							}
						}
						foreach (TEntity entity in newItems)
						{
							TViewObject viewObject = this.PrepareViewObject(null, entity);

							if (!this.SuppressViewObjectsChangeHandling)
							{
								this.ViewObjects.Add(viewObject);
							}
						}

						this.OnPropertyChanged(nameof(this.Entities));
						this.OnEntitiesChanged(oldItems, newItems);

						break;
					}
				}
			}
			finally
			{
				this.SuppressEntitiesChangeHandling = false;
			}
		}

		private TViewObject PrepareViewObject (TViewObject viewObj, TEntity entityCandidate)
		{
			viewObj = viewObj ?? this.CreateViewObject();

			viewObj.Entity = (viewObj.Entity ?? entityCandidate) ?? this.CreateEntity();
			viewObj.ViewCaller = this;

			return viewObj;
		}

		private void SourceChangedMethod (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!this.ObserveSource)
			{
				return;
			}

			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if (this.SuppressSourceChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressSourceChangeHandling = true;

				switch (e.Action)
				{
					default:
					{
						throw new NotSupportedException(e.Action + " not supported.");
					}

					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Replace:
					{
						List<TEntity> oldItems = new List<TEntity>();
						List<TEntity> newItems = new List<TEntity>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<TEntity>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<TEntity>());
						}

						foreach (TEntity entity in oldItems)
						{
							TViewObject viewObj = this.GetViewObjectForEntity(entity);

							if (!this.SuppressViewObjectsChangeHandling)
							{
								if (viewObj != null)
								{
									this.ViewObjects.Remove(viewObj);
								}
							}
						}
						foreach (TEntity entity in newItems)
						{
							TViewObject viewObject = this.PrepareViewObject(null, entity);

							if (!this.SuppressViewObjectsChangeHandling)
							{
								this.ViewObjects.Add(viewObject);
							}
						}

						this.OnPropertyChanged(nameof(this.Source));
						this.OnSourceChanged(oldItems, newItems);

						break;
					}
				}
			}
			finally
			{
				this.SuppressSourceChangeHandling = false;
			}
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private void Update (bool resetPageNumber)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			try
			{
				bool pageNumberReallyReset = resetPageNumber && (this.PageNumber > 1);

				this.IsUpdating = true;

				this.OnUpdating(pageNumberReallyReset);

				if (pageNumberReallyReset)
				{
					this.PageNumber = 1;
				}

				IList<TEntity> oldEntities = (IList<TEntity>)this.Entities ?? new List<TEntity>();
				IList<TViewObject> oldViewObjects = (IList<TViewObject>)this.ViewObjects ?? new List<TViewObject>();

				if (this.ViewObjects != null)
				{
					this.ViewObjects.CollectionChanged -= this.ViewObjectsChangedHandler;
				}

				if (this.Entities != null)
				{
					this.Entities.CollectionChanged -= this.EntitiesChangedHandler;
				}

				if (this.CollectionUpdateStrategy == EntityViewCollectionUpdateStrategy.Recreate)
				{
					this.ViewObjects = null;
					this.Entities = null;
				}
				else
				{
					this.ViewObjects = this.ViewObjects ?? new ObservableCollection<TViewObject>();
					this.Entities = this.Entities ?? new ObservableCollection<TEntity>();

					this.ViewObjects?.Clear();
					this.Entities?.Clear();
				}

				int entityTotalCount = 0;
				int entityFilteredCount = 0;
				int pageFilteredCount = 0;

				List<TEntity> entities = this.PageNumber == 0 ? new List<TEntity>() : this.Set.GetFiltered(this.Source, this.Filter, this.Sort, this.PageNumber - 1, this.PageSize, out entityTotalCount, out entityFilteredCount, out pageFilteredCount).ToList();
				List<TViewObject> viewObjects = (from x in entities select this.PrepareViewObject(null, x)).ToList();

				int pageTotalCount = this.PageNumber == 0 ? 0 : (this.PageSize == 0 ? 1 : ((entityTotalCount / this.PageSize) + (((entityTotalCount % this.PageSize) == 0) ? 0 : 1)));
				pageTotalCount = Math.Max(pageTotalCount, 1);

				if (this.CollectionUpdateStrategy == EntityViewCollectionUpdateStrategy.Recreate)
				{
					this.Entities = new ObservableCollection<TEntity>(entities);
					this.ViewObjects = new ObservableCollection<TViewObject>(viewObjects);
				}
				else
				{
					this.Entities.AddRange(entities);
					this.ViewObjects.AddRange(viewObjects);
				}

				this.Entities.CollectionChanged += this.EntitiesChangedHandler;
				this.ViewObjects.CollectionChanged += this.ViewObjectsChangedHandler;

				this.EntityTotalCount = entityTotalCount;
				this.EntityFilteredCount = entityFilteredCount;
				this.PageTotalCount = pageTotalCount;
				this.PageFilteredCount = pageFilteredCount;

				this.OnEntitiesChanged(oldEntities, this.Entities);
				this.OnViewObjectsChanged(oldViewObjects, this.ViewObjects);

				this.OnPropertyChanged(nameof(this.Entities));
				this.OnPropertyChanged(nameof(this.ViewObjects));
				this.OnPropertyChanged(nameof(this.Source));

				this.OnPropertyChanged(nameof(this.EntityTotalCount));
				this.OnPropertyChanged(nameof(this.EntityFilteredCount));
				this.OnPropertyChanged(nameof(this.PageTotalCount));
				this.OnPropertyChanged(nameof(this.PageFilteredCount));

				this.OnPropertyChanged(nameof(this.SelectedEntity));
				this.OnPropertyChanged(nameof(this.SelectedViewObject));

				this.OnPropertyChanged(nameof(this.EditedEntity));
				this.OnPropertyChanged(nameof(this.EditedViewObject));

				this.OnPropertyChanged(nameof(this.Filter));
				this.OnPropertyChanged(nameof(this.Sort));
				this.OnPropertyChanged(nameof(this.PageSize));
				this.OnPropertyChanged(nameof(this.PageNumber));

				this.OnPropertyChanged(nameof(this.CollectionUpdateStrategy));

				this.OnUpdated(pageNumberReallyReset);
			}
			finally
			{
				this.IsUpdating = false;
			}
		}

		private void ViewObjectsChangedMethod (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if (this.SuppressViewObjectsChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressViewObjectsChangeHandling = true;

				ICollection<TEntity> source = this.Source as ICollection<TEntity>;

				switch (e.Action)
				{
					default:
					{
						throw new NotSupportedException(e.Action + " not supported.");
					}

					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Replace:
					{
						List<TViewObject> oldItems = new List<TViewObject>();
						List<TViewObject> newItems = new List<TViewObject>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<TViewObject>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<TViewObject>());
						}

						foreach (TViewObject viewObj in oldItems)
						{
							if (object.ReferenceEquals(viewObj, this.SelectedViewObject))
							{
								this.SelectedViewObject = null;
							}

							this.EntityDelete(viewObj);

							if (!this.SuppressEntitiesChangeHandling)
							{
								this.Entities.Remove(viewObj.Entity);
							}

							if ((!this.SuppressSourceChangeHandling) && this.UpdateSource)
							{
								source?.Remove(viewObj.Entity);
							}
						}
						foreach (TViewObject viewObj in newItems)
						{
							this.PrepareViewObject(viewObj, null);

							if ((!this.SuppressSourceChangeHandling) && this.UpdateSource)
							{
								source?.Add(viewObj.Entity);
							}

							if (!this.SuppressEntitiesChangeHandling)
							{
								this.Entities.Add(viewObj.Entity);
							}

							if (this.EntityToAddIsAttached)
							{
								this.EntityAttach(viewObj);
							}
							else
							{
								this.EntityAdd(viewObj);
							}
						}

						this.OnPropertyChanged(nameof(this.ViewObjects));
						this.OnViewObjectsChanged(oldItems, newItems);

						this.OnPropertyChanged(nameof(this.SelectedEntity));
						this.OnPropertyChanged(nameof(this.SelectedViewObject));

						this.OnPropertyChanged(nameof(this.EditedEntity));
						this.OnPropertyChanged(nameof(this.EditedViewObject));

						break;
					}
				}
			}
			finally
			{
				this.SuppressViewObjectsChangeHandling = false;
			}
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports creating new entities.
		/// </summary>
		/// <returns>
		///     true if new entities can be created, false otherwise.
		/// </returns>
		public virtual bool CanNew ()
		{
			return this.EntityCanNew();
		}

		/// <summary>
		///     Creates and adds a new entity.
		/// </summary>
		/// <returns>
		///     The new entity.
		/// </returns>
		public virtual TViewObject New ()
		{
			TViewObject viewObj = this.PrepareViewObject(null, null);
			this.ViewObjects.Add(viewObj);
			return viewObj;
		}

		/// <summary>
		///     Called when a new entity is to be created.
		/// </summary>
		/// <returns>
		///     The newly created entity.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The default implementation uses <see cref="IRepositorySet{T}.Create" /> to create a new instance of the entities type.
		///     </para>
		/// </remarks>
		protected virtual TEntity CreateEntity ()
		{
			return this.Set.Create();
		}

		/// <summary>
		///     Called when a new view object is to be created before it is assigned to wrap an entity.
		/// </summary>
		/// <returns>
		///     The newly created view object.
		/// </returns>
		protected virtual TViewObject CreateViewObject ()
		{
			return new TViewObject();
		}

		/// <summary>
		///     Called when an entities view object is to be added.
		/// </summary>
		/// <param name="viewObject"> The view object to add. </param>
		protected virtual void EntityAdd (TViewObject viewObject)
		{
			if (viewObject.IsAddedOrAttached)
			{
				return;
			}
			viewObject.IsAdded = true;

			viewObject.IsAttached = false;
			viewObject.IsDeleted = false;

			viewObject.RaiseEntityChanged();

			this.Set.Add(viewObject.Entity);
		}

		/// <summary>
		///     Called when an entities view object is to be attached.
		/// </summary>
		/// <param name="viewObject"> The view object to attach. </param>
		protected virtual void EntityAttach (TViewObject viewObject)
		{
			if (viewObject.IsAddedOrAttached)
			{
				return;
			}
			viewObject.IsAttached = true;

			viewObject.IsAdded = false;
			viewObject.IsDeleted = false;

			viewObject.RaiseEntityChanged();

			this.Set.Attach(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether an entities view object can be added.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be added, false otherwise.
		/// </returns>
		protected virtual bool EntityCanAdd (TViewObject viewObject)
		{
			return this.Set.CanAdd(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether an entities view object can be attached.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be attached, false otherwise.
		/// </returns>
		protected virtual bool EntityCanAttach (TViewObject viewObject)
		{
			return this.Set.CanAttach(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether an entities view object can be deleted.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be deleted, false otherwise.
		/// </returns>
		protected virtual bool EntityCanDelete (TViewObject viewObject)
		{
			return this.Set.CanDelete(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether an entities view object can be edited.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be edited, false otherwise.
		/// </returns>
		protected virtual bool EntityCanEdit (TViewObject viewObject)
		{
			return this.AllowEdit;
		}

		/// <summary>
		///     Called to determine whether an entities view object can be modified.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be modified, false otherwise.
		/// </returns>
		protected virtual bool EntityCanModify (TViewObject viewObject)
		{
			return this.Set.CanModify(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether a new entity or view object can be created.
		/// </summary>
		/// <returns>
		///     true if a new entity or view object can be created, false otherwise.
		/// </returns>
		protected virtual bool EntityCanNew ()
		{
			return this.Set.CanCreate();
		}

		/// <summary>
		///     Called to determine whether an entities view object can be reloaded.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be reloaded, false otherwise.
		/// </returns>
		protected virtual bool EntityCanReload (TViewObject viewObject)
		{
			return this.Set.CanReload(viewObject.Entity);
		}

		/// <summary>
		///     Called to determine whether an entities view object can be selected.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be selected, false otherwise.
		/// </returns>
		protected virtual bool EntityCanSelect (TViewObject viewObject)
		{
			return this.AllowSelect;
		}

		/// <summary>
		///     Called to determine whether an entities view object can be validated.
		/// </summary>
		/// <param name="viewObject"> The view object. </param>
		/// <returns>
		///     true if the view object can be validated, false otherwise.
		/// </returns>
		protected virtual bool EntityCanValidate (TViewObject viewObject)
		{
			return this.Set.CanValidate(viewObject.Entity);
		}

		/// <summary>
		///     Called when an entities view object is to be deleted.
		/// </summary>
		/// <param name="viewObject"> The view object to delete. </param>
		protected virtual void EntityDelete (TViewObject viewObject)
		{
			if (viewObject.IsDeleted)
			{
				return;
			}
			viewObject.IsDeleted = true;

			viewObject.IsAdded = false;
			viewObject.IsAttached = false;

			viewObject.RaiseEntityChanged();

			this.Set.Delete(viewObject.Entity);
		}

		/// <summary>
		///     Called when an entities view object is to be deselected.
		/// </summary>
		/// <param name="viewObject"> The view object to deselect. </param>
		protected virtual void EntityDeselect (TViewObject viewObject)
		{
			if (!viewObject.IsSelected)
			{
				return;
			}
			viewObject.IsSelected = false;

			viewObject.RaiseEntityChanged();

			this.OnPropertyChanged(nameof(this.SelectedEntity));
			this.OnPropertyChanged(nameof(this.SelectedViewObject));

			this.OnEntityDeselected(viewObject);
		}

		/// <summary>
		///     Called when an entities view object is to start editing.
		/// </summary>
		/// <param name="viewObject"> The view object to start editing. </param>
		protected virtual void EntityEditBegin (TViewObject viewObject)
		{
			if (viewObject.IsEdited)
			{
				return;
			}
			viewObject.IsEdited = true;

			viewObject.RaiseEntityChanged();

			this.OnPropertyChanged(nameof(this.EditedEntity));
			this.OnPropertyChanged(nameof(this.EditedViewObject));

			this.OnEntityBeginEdit(viewObject);
		}

		/// <summary>
		///     Called when an entities view object is to cancel editing.
		/// </summary>
		/// <param name="viewObject"> The vie object to cancel editing. </param>
		protected virtual void EntityEditCancel (TViewObject viewObject)
		{
			if (!viewObject.IsEdited)
			{
				return;
			}
			viewObject.IsEdited = false;

			viewObject.RaiseEntityChanged();

			this.OnPropertyChanged(nameof(this.EditedEntity));
			this.OnPropertyChanged(nameof(this.EditedViewObject));

			this.OnEntityCancelEdit(viewObject);
		}

		/// <summary>
		///     Called when an entities view object is to finish editing.
		/// </summary>
		/// <param name="viewObject"> The view object to finish editing. </param>
		protected virtual void EntityEditEnd (TViewObject viewObject)
		{
			if (!viewObject.IsEdited)
			{
				return;
			}
			viewObject.IsEdited = false;

			viewObject.RaiseEntityChanged();

			this.OnPropertyChanged(nameof(this.EditedEntity));
			this.OnPropertyChanged(nameof(this.EditedViewObject));

			this.OnEntityEndEdit(viewObject);
		}

		/// <summary>
		///     Called when an entities view object is to be marked as modified.
		/// </summary>
		/// <param name="viewObject"> The view object to mark as modified. </param>
		protected virtual void EntityModify (TViewObject viewObject)
		{
			this.Set.Modify(viewObject.Entity);

			viewObject.IsModified = true;

			viewObject.RaiseEntityChanged();
		}

		/// <summary>
		///     Called when an entities view object is to be reloaded.
		/// </summary>
		/// <param name="viewObject"> The view object to reload. </param>
		protected virtual void EntityReload (TViewObject viewObject)
		{
			this.Set.Reload(viewObject.Entity);

			viewObject.Errors = null;
			viewObject.IsModified = false;

			viewObject.RaiseEntityChanged();
		}

		/// <summary>
		///     Called when an entities view object is to be selected.
		/// </summary>
		/// <param name="viewObject"> The view object to select. </param>
		protected virtual void EntitySelect (TViewObject viewObject)
		{
			if (viewObject.IsSelected)
			{
				return;
			}
			viewObject.IsSelected = true;

			viewObject.RaiseEntityChanged();

			this.OnPropertyChanged(nameof(this.SelectedEntity));
			this.OnPropertyChanged(nameof(this.SelectedViewObject));

			this.OnEntitySelected(viewObject);
		}

		/// <summary>
		///     Called when an entities view object is to be validated.
		/// </summary>
		/// <param name="viewObject"> The view object to validate. </param>
		protected virtual void EntityValidate (TViewObject viewObject)
		{
			viewObject.Errors = this.Set.Validate(viewObject.Entity);
			viewObject.IsModified = this.Set.IsModified(viewObject.Entity);

			viewObject.RaiseEntityChanged();
		}

		/// <summary>
		///     Raises <see cref="EntitiesChanged" />.
		/// </summary>
		/// <param name="oldItems"> The entities which were removed. </param>
		/// <param name="newItems"> The entities which were added. </param>
		protected virtual void OnEntitiesChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.EntitiesChanged?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
		}

		/// <summary>
		///     Raises <see cref="EntityBeginEdit" />.
		/// </summary>
		/// <param name="viewObject"> The entities view object. </param>
		protected virtual void OnEntityBeginEdit (TViewObject viewObject)
		{
			this.EntityBeginEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		/// <summary>
		///     Raises <see cref="EntityCancelEdit" />.
		/// </summary>
		/// <param name="viewObject"> The entities view object. </param>
		protected virtual void OnEntityCancelEdit (TViewObject viewObject)
		{
			this.EntityCancelEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		/// <summary>
		///     Raises <see cref="EntityDeselected" />.
		/// </summary>
		/// <param name="viewObject"> The entities view object. </param>
		protected virtual void OnEntityDeselected (TViewObject viewObject)
		{
			this.EntityDeselected?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		/// <summary>
		///     Raises <see cref="EntityEndEdit" />.
		/// </summary>
		/// <param name="viewObject"> The entities view object. </param>
		protected virtual void OnEntityEndEdit (TViewObject viewObject)
		{
			this.EntityEndEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		/// <summary>
		///     Raises <see cref="EntitySelected" />.
		/// </summary>
		/// <param name="viewObject"> The entities view object. </param>
		protected virtual void OnEntitySelected (TViewObject viewObject)
		{
			this.EntitySelected?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///     Raises <see cref="SourceChanged" />.
		/// </summary>
		/// <param name="oldItems"> The entities which were removed. </param>
		/// <param name="newItems"> The entities which were added. </param>
		protected virtual void OnSourceChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.SourceChanged?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
		}

		/// <summary>
		///     Raises <see cref="Updated" />.
		/// </summary>
		/// <param name="resetPageNumber"> Specifies whether the page number was reset to 1. </param>
		protected virtual void OnUpdated (bool resetPageNumber)
		{
			this.Updated?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		/// <summary>
		///     Raises <see cref="Updating" />.
		/// </summary>
		/// <param name="resetPageNumber"> Specifies whether the page number was reset to 1. </param>
		protected virtual void OnUpdating (bool resetPageNumber)
		{
			this.Updating?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		/// <summary>
		///     Raises <see cref="ViewObjectsChanged" />.
		/// </summary>
		/// <param name="oldItems"> The view objects which were removed. </param>
		/// <param name="newItems"> The view objects which were added. </param>
		protected virtual void OnViewObjectsChanged (IList<TViewObject> oldItems, IList<TViewObject> newItems)
		{
			this.ViewObjectsChanged?.Invoke(this, new EntityViewItemsEventArgs<TViewObject>(oldItems, newItems));
		}

		#endregion




		#region Interface: IEntityViewCaller<TEntity>

		/// <summary>
		///     Adds a new entity.
		/// </summary>
		/// <param name="entity"> The entity to add. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Add (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Entities.Add(entity);
		}

		/// <summary>
		///     Attaches an exiting entity.
		/// </summary>
		/// <param name="entity"> The entity to attach. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Attach (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			try
			{
				this.EntityToAddIsAttached = true;
				this.Entities.Add(entity);
			}
			finally
			{
				this.EntityToAddIsAttached = false;
			}
		}

		/// <summary>
		///     Starts editing of an entity.
		/// </summary>
		/// <param name="entity"> The entity to edit. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void BeginEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditBegin(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports adding of the specified new entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the new entity can be added, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanAdd (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAdd(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports attaching of the specified existing entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the existing entity can be attached, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanAttach (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAttach(viewObject);
		}

		/// <summary>
		///     Finishes editing of an entity without applying the changes made during edit.
		/// </summary>
		/// <param name="entity"> The entity to finish editing of. </param>
		/// <remarks>
		///     <note type="note">
		///         <see cref="CancelEdit" /> does not automatically reload the entity or does not discard changes made since <see cref="BeginEdit" /> was called respectively.
		///         This cannot be done as the higher context is not known by the <see cref="EntityView{TEntity,TViewObject}" />.
		///         However, you can do your own reload logic by listening to the <see cref="EntityCancelEdit" /> event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void CancelEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditCancel(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports deleting of the specified entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanDelete (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanDelete(viewObject);
		}

		/// <summary>
		///     Determines whether this <see cref="EntityView{TEntity,TViewObject}" /> supports editing of the specified entity (that is, if <see cref="AllowEdit" /> is true).
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be edited, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanEdit(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports modification of the specified entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanModify (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanModify(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports reloading of the specified entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanReload (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanReload(viewObject);
		}

		/// <summary>
		///     Determines whether this <see cref="EntityView{TEntity,TViewObject}" /> supports selection of the specified entity (that is, if <see cref="AllowSelect" /> is true).
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be selected, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanSelect (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanSelect(viewObject);
		}

		/// <summary>
		///     Determines whether the underlying <see cref="IRepositorySet{T}" /> supports checking for changes and validation of the specified entity.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be checked for changes and validated, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public bool CanValidate (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanValidate(viewObject);
		}

		/// <summary>
		///     Deletes an entity.
		/// </summary>
		/// <param name="entity"> The entity to delete. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Delete (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Entities.Remove(entity);
		}

		/// <summary>
		///     Deselects an entity.
		/// </summary>
		/// <param name="entity"> The entity to deselect. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Deselect (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityDeselect(viewObject);
		}

		/// <summary>
		///     Finished editing of an entity and applies the changes made during edit.
		/// </summary>
		/// <param name="entity"> The entity to finish editing of. </param>
		/// <remarks>
		///     <note type="note">
		///         <see cref="EndEdit" /> does not automatically save the entity or does not save changes made since <see cref="BeginEdit" /> was called respectively.
		///         This cannot be done as the higher context is not known by the <see cref="EntityView{TEntity,TViewObject}" />.
		///         However, you can do your own reload logic by listening to the <see cref="EntityEndEdit" /> event.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void EndEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditEnd(viewObject);
		}

		/// <summary>
		///     Explicitly marks an entity as modified, regardless whether actual changes were made.
		/// </summary>
		/// <param name="entity"> The entity to mark as modify. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Modify (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityModify(viewObject);
		}

		/// <summary>
		///     Relaods an entity from the store used by the underlying <see cref="IRepositoryContext" /> to which the associated <see cref="IRepositorySet{T}" /> belongs.
		/// </summary>
		/// <param name="entity"> The entity to reload. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Reload (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityReload(viewObject);
		}

		/// <summary>
		///     Selects an entity.
		/// </summary>
		/// <param name="entity"> The entity to select. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Select (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntitySelect(viewObject);
		}

		/// <summary>
		///     Checks an entity for changes and validates the entity.
		/// </summary>
		/// <param name="entity"> The entity to check for changes and validate. </param>
		/// <remarks>
		///     <para>
		///         The information whether the entity has changes and is valid can be retrieved through its associated <see cref="EntityViewObject{TEntity}" /> (<see cref="EntityViewObject{TEntity}.IsModified" />, <see cref="EntityViewObject{TEntity}.IsValid" />, <see cref="EntityViewObject{TEntity}.Errors" />).
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		public void Validate (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityValidate(viewObject);
		}

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}

	/// <inheritdoc cref="EntityView{TEntity,TViewObject}" />
	public class EntityView <TEntity> : EntityView<TEntity, EntityViewObject<TEntity>>
		where TEntity : class, new()
	{
		#region Instance Constructor/Destructor

		/// <inheritdoc cref="EntityView{TEntity,TViewObject}(IRepositorySet{TEntity})" />
		/// .
		public EntityView (IRepositorySet<TEntity> set)
			: base(set)
		{
		}

		/// <inheritdoc cref="EntityView{TEntity,TViewObject}(IRepositorySet{TEntity}, IEnumerable{TEntity})" />
		/// .
		public EntityView (IRepositorySet<TEntity> set, IEnumerable<TEntity> source)
			: base(set, source)
		{
		}

		#endregion
	}
}
