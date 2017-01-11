﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;




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
	///         Therefore, an <see cref="EntityView{TEntity,TViewObject}" /> can be seen as an additional buffering layer on top of a <see cref="IRepositorySet{T}" /> which can be used to paginate, filter, and manage the items of the <see cref="IRepositorySet{T}" />.
	///     </para>
	///     <para>
	///         <see cref="EntityView{TEntity,TViewObject}" /> hides some of the complexity of <see cref="IRepositorySet{T}" /> by presenting the items through simple collections (<see cref="Entities" /> and <see cref="ViewObjects" />).
	///     </para>
	///     <para>
	///         <see cref="EntityView{TEntity,TViewObject}" /> either uses all of the associated <see cref="IRepositorySet{T}" />s items if <see cref="Source" /> is not set, or only uses the items provided by <see cref="Source" />, if set.
	///         This allows to control the final set of items an <see cref="EntityView{TEntity,TViewObject}" /> is using, as long as the items are associated with the specified <see cref="IRepositorySet{T}" />.
	///     </para>
	/// </remarks>
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
			this.PageSize = 0;
			this.PageNumber = 0;

			this.IsInitializing = false;

			this.Update(false);
		}

		#endregion




		#region Instance Fields

		private bool _allowEdit;

		private bool _allowSelect;

		private object _filter;

		private bool _observeSource;

		private int _pageNumber;

		private int _pageSize;

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
		///     Gets the total number of items which could be presented by this view, if no <see cref="Filter" /> would be set.
		/// </summary>
		/// <value>
		///     The total number of items which could be presented by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="EntityTotalCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int EntityTotalCount { get; private set; }

		public object Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
				this.OnPropertyChanged(nameof(this.Filter));

				this.Update(true);
			}
		}

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
		///     Gets the number of pages actually available by this view, after <see cref="Filter" /> is applied to the data source.
		/// </summary>
		/// <value>
		///     The number of pages actually available by this view.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="PageFilteredCount" /> is updated during <see cref="Update()" />.
		///         See <see cref="Update()" /> for more details.
		///     </para>
		/// </remarks>
		public int PageFilteredCount { get; private set; }

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
				this.OnPropertyChanged(nameof(this.PageNumber));

				this.Update(false);
			}
		}

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
				this.OnPropertyChanged(nameof(this.PageSize));

				this.Update(true);
			}
		}

		/// <summary>
		///     Gets the total number of pages which could be available by this view, if no <see cref="Filter" /> would be set.
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

		public IRepositorySet<TEntity> Set { get; private set; }

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

				this.OnPropertyChanged(nameof(this.Source));
				this.OnSourceChanged(oldItems, newItems);

				this.Update(true);
			}
		}

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

		protected bool EntityToAddIsAttached { get; private set; }

		protected bool IsInitializing { get; private set; }

		protected bool IsUpdating { get; private set; }

		protected bool SuppressEntitiesChangeHandling { get; private set; }

		protected bool SuppressSourceChangeHandling { get; private set; }

		protected bool SuppressViewObjectsChangeHandling { get; private set; }

		private NotifyCollectionChangedEventHandler EntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler SourceChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler ViewObjectsChangedHandler { get; set; }

		#endregion




		#region Instance Events

		public event EventHandler<EntityViewItemsEventArgs<TEntity>> EntitiesChanged;

		//TODO: On method
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityBeginEdit;

		//TODO: On method
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityCancelEdit;

		//TODO: On method
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityDeselected;

		//TODO: On method
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityEndEdit;

		//TODO: On method
		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntitySelected;

		public event EventHandler<EntityViewItemsEventArgs<TEntity>> SourceChanged;

		public event EventHandler<EntityViewUpdateEventArgs> Updated;

		public event EventHandler<EntityViewUpdateEventArgs> Updating;

		public event EventHandler<EntityViewItemsEventArgs<TViewObject>> ViewObjectsChanged;

		#endregion




		#region Instance Methods

		public TViewObject GetViewObjectForEntity (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return (from x in this.ViewObjects where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}

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
				this.IsUpdating = true;

				this.OnUpdating(resetPageNumber);

				if (resetPageNumber && (this.PageNumber > 1))
				{
					this.PageNumber = 1;
				}

				IList<TEntity> oldEntities = (IList<TEntity>)this.Entities ?? new List<TEntity>();
				IList<TViewObject> oldViewObjects = (IList<TViewObject>)this.ViewObjects ?? new List<TViewObject>();

				if (this.ViewObjects != null)
				{
					this.ViewObjects.CollectionChanged -= this.ViewObjectsChangedHandler;
					this.ViewObjects = null;
				}

				if (this.Entities != null)
				{
					this.Entities.CollectionChanged -= this.EntitiesChangedHandler;
					this.Entities = null;
				}

				int entityTotalCount = 0;
				int entityFilteredCount = 0;
				int pageFilteredCount = 0;

				List<TEntity> entities = this.PageNumber == 0 ? new List<TEntity>() : this.Set.GetFiltered(this.Source, this.Filter, this.PageNumber - 1, this.PageSize, out entityTotalCount, out entityFilteredCount, out pageFilteredCount).ToList();
				List<TViewObject> viewObjects = (from x in entities select this.PrepareViewObject(null, x)).ToList();

				int pageTotalCount = this.PageNumber == 0 ? 0 : (this.PageSize == 0 ? 1 : ((entityTotalCount / this.PageSize) + (((entityTotalCount % this.PageSize) == 0) ? 0 : 1)));

				this.Entities = new ObservableCollection<TEntity>(entities);
				this.Entities.CollectionChanged += this.EntitiesChangedHandler;

				this.ViewObjects = new ObservableCollection<TViewObject>(viewObjects);
				this.ViewObjects.CollectionChanged += this.ViewObjectsChangedHandler;

				this.EntityTotalCount = entityTotalCount;
				this.EntityFilteredCount = entityFilteredCount;
				this.PageTotalCount = pageTotalCount;
				this.PageFilteredCount = pageFilteredCount;

				this.OnPropertyChanged(nameof(this.Entities));
				this.OnPropertyChanged(nameof(this.ViewObjects));

				this.OnEntitiesChanged(oldEntities, this.Entities);
				this.OnViewObjectsChanged(oldViewObjects, this.ViewObjects);

				this.OnPropertyChanged(nameof(this.EntityTotalCount));
				this.OnPropertyChanged(nameof(this.EntityFilteredCount));
				this.OnPropertyChanged(nameof(this.PageTotalCount));
				this.OnPropertyChanged(nameof(this.PageFilteredCount));

				this.OnPropertyChanged(nameof(this.SelectedEntity));
				this.OnPropertyChanged(nameof(this.SelectedViewObject));

				this.OnPropertyChanged(nameof(this.EditedEntity));
				this.OnPropertyChanged(nameof(this.EditedViewObject));

				this.OnUpdated(resetPageNumber);
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

		public virtual bool CanNew ()
		{
			return this.EntityCanNew();
		}

		public virtual TViewObject New ()
		{
			TViewObject viewObj = this.PrepareViewObject(null, null);
			this.ViewObjects.Add(viewObj);
			return viewObj;
		}

		protected virtual TEntity CreateEntity ()
		{
			return this.Set.Create();
		}

		protected virtual TViewObject CreateViewObject ()
		{
			return new TViewObject();
		}

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

		protected virtual bool EntityCanAdd (TViewObject viewObject)
		{
			return this.Set.CanAdd(viewObject.Entity);
		}

		protected virtual bool EntityCanAttach (TViewObject viewObject)
		{
			return this.Set.CanAttach(viewObject.Entity);
		}

		protected virtual bool EntityCanDelete (TViewObject viewObject)
		{
			return this.Set.CanDelete(viewObject.Entity);
		}

		protected virtual bool EntityCanEdit (TViewObject viewObject)
		{
			return this.AllowEdit;
		}

		protected virtual bool EntityCanModify (TViewObject viewObject)
		{
			return this.Set.CanModify(viewObject.Entity);
		}

		protected virtual bool EntityCanNew ()
		{
			return this.Set.CanCreate();
		}

		protected virtual bool EntityCanReload (TViewObject viewObject)
		{
			return this.Set.CanReload(viewObject.Entity);
		}

		protected virtual bool EntityCanSelect (TViewObject viewObject)
		{
			return this.AllowSelect;
		}

		protected virtual bool EntityCanValidate (TViewObject viewObject)
		{
			return this.Set.CanValidate(viewObject.Entity);
		}

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

			this.EntityDeselected?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

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

			this.EntityBeginEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

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

			this.EntityCancelEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

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

			this.EntityEndEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		protected virtual void EntityModify (TViewObject viewObject)
		{
			this.Set.Modify(viewObject.Entity);

			viewObject.IsModified = true;

			viewObject.RaiseEntityChanged();
		}

		protected virtual void EntityReload (TViewObject viewObject)
		{
			this.Set.Reload(viewObject.Entity);

			viewObject.Errors = null;
			viewObject.IsModified = false;

			viewObject.RaiseEntityChanged();
		}

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

			this.EntitySelected?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		protected virtual void EntityValidate (TViewObject viewObject)
		{
			viewObject.Errors = this.Set.Validate(viewObject.Entity);
			viewObject.IsModified = this.Set.IsModified(viewObject.Entity);

			viewObject.RaiseEntityChanged();
		}

		protected virtual void OnEntitiesChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.EntitiesChanged?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
		}

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnSourceChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.SourceChanged?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
		}

		protected virtual void OnUpdated (bool resetPageNumber)
		{
			this.Updated?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		protected virtual void OnUpdating (bool resetPageNumber)
		{
			this.Updating?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		protected virtual void OnViewObjectsChanged (IList<TViewObject> oldItems, IList<TViewObject> newItems)
		{
			this.ViewObjectsChanged?.Invoke(this, new EntityViewItemsEventArgs<TViewObject>(oldItems, newItems));
		}

		#endregion




		#region Interface: IEntityViewCaller<TEntity>

		public void Add (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Entities.Add(entity);
		}

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

		public void BeginEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditBegin(viewObject);
		}

		public bool CanAdd (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAdd(viewObject);
		}

		public bool CanAttach (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAttach(viewObject);
		}

		public void CancelEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditCancel(viewObject);
		}

		public bool CanDelete (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanDelete(viewObject);
		}

		public bool CanEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanEdit(viewObject);
		}

		public bool CanModify (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanModify(viewObject);
		}

		public bool CanReload (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanReload(viewObject);
		}

		public bool CanSelect (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanSelect(viewObject);
		}

		public bool CanValidate (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanValidate(viewObject);
		}

		public void Delete (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Entities.Remove(entity);
		}

		public void Deselect (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityDeselect(viewObject);
		}

		public void EndEdit (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditEnd(viewObject);
		}

		public void Modify (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityModify(viewObject);
		}

		public void Reload (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityReload(viewObject);
		}

		public void Select (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntitySelect(viewObject);
		}

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

	public class EntityView <TEntity> : EntityView<TEntity, EntityViewObject<TEntity>>
		where TEntity : class, new()
	{
		#region Instance Constructor/Destructor

		public EntityView (IRepositorySet<TEntity> set)
			: base(set)
		{
		}

		public EntityView (IRepositorySet<TEntity> set, IEnumerable<TEntity> source)
			: base(set, source)
		{
		}

		#endregion
	}
}
