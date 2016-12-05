using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityView <TEntity, TViewObject> : INotifyPropertyChanged, INotifyPropertyChanging, IEntityViewCaller<TEntity>
		where TEntity : class, new()
		where TViewObject : EntityViewObject<TEntity>, new()
	{
		#region Instance Constructor/Destructor

		public EntityView (IRepositorySet<TEntity> set)
			: this(set, null)
		{
		}

		public EntityView (IRepositorySet<TEntity> set, IEnumerable<TEntity> source)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.IsInitializing = true;

			this.Set = set;
			this.Source = source;

			this.ItemsChangedHandler = this.ItemsChanged;
			this.ViewObjectsChangedHandler = this.ViewObjectsChanged;
			this.SourceChangedHandler = this.SourceChanged;

			this.IsUpdating = false;
			this.SuppressItemsChangeHandling = false;
			this.SuppressViewObjectChangeHandling = false;
			this.SuppressSourceChangeHandling = false;
			this.ItemToAddIsAttached = false;

			this.AllowEdit = true;
			this.ObserveSource = true;
			this.UpdateSource = true;

			this.Items = null;
			this.ViewObjects = null;

			this.EntityTotalCount = 0;
			this.EntityFilteredCount = 0;
			this.PageTotalCount = 0;
			this.PageFilteredCount = 0;

			this.Filter = null;
			this.PageSize = 0;
			this.PageNumber = 0;

			this.IsInitializing = false;

			this.UpdateItems(false);
		}

		#endregion




		#region Instance Fields

		private bool _allowEdit;

		private object _filter;

		private bool _observeSource;

		private int _pageNumber;

		private int _pageSize;

		private IEnumerable<TEntity> _source;

		private bool _updateSource;

		#endregion




		#region Instance Properties/Indexer

		public bool AllowEdit
		{
			get
			{
				return this._allowEdit;
			}
			set
			{
				this.OnPropertyChanging(nameof(this.AllowEdit));
				this._allowEdit = value;
				this.OnPropertyChanged(nameof(this.AllowEdit));
			}
		}

		public int EntityFilteredCount { get; private set; }

		public int EntityTotalCount { get; private set; }

		public object Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this.OnPropertyChanging(nameof(this.Filter));
				this._filter = value;
				this.OnPropertyChanged(nameof(this.Filter));

				this.UpdateItems(true);
			}
		}

		public ObservableCollection<TEntity> Items { get; private set; }

		public bool ObserveSource
		{
			get
			{
				return this._observeSource;
			}
			set
			{
				this.OnPropertyChanging(nameof(this.ObserveSource));
				this._observeSource = value;
				this.OnPropertyChanged(nameof(this.ObserveSource));
			}
		}

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

				this.OnPropertyChanging(nameof(this.PageNumber));
				this._pageNumber = value;
				this.OnPropertyChanged(nameof(this.PageNumber));

				this.UpdateItems(false);
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

				this.OnPropertyChanging(nameof(this.PageSize));
				this._pageSize = value;
				this.OnPropertyChanged(nameof(this.PageSize));

				this.UpdateItems(true);
			}
		}

		public int PageTotalCount { get; private set; }

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

				this.OnPropertyChanging(nameof(this.Source));
				this._source = value;
				this.OnPropertyChanged(nameof(this.Source));

				if (this._source is INotifyCollectionChanged)
				{
					((INotifyCollectionChanged)this._source).CollectionChanged += this.SourceChangedHandler;
				}

				this.UpdateItems(true);
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
				this.OnPropertyChanging(nameof(this.UpdateSource));
				this._updateSource = value;
				this.OnPropertyChanged(nameof(this.UpdateSource));
			}
		}

		public ObservableCollection<TViewObject> ViewObjects { get; private set; }

		protected bool IsInitializing { get; private set; }

		protected bool IsUpdating { get; private set; }

		protected bool ItemToAddIsAttached { get; private set; }

		protected bool SuppressItemsChangeHandling { get; private set; }

		protected bool SuppressSourceChangeHandling { get; private set; }

		protected bool SuppressViewObjectChangeHandling { get; private set; }

		private NotifyCollectionChangedEventHandler ItemsChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler SourceChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler ViewObjectsChangedHandler { get; set; }

		#endregion




		#region Instance Events

		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityBeginEdit;

		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityCancelEdit;

		public event EventHandler<EntityViewItemEventArgs<TViewObject>> EntityEndEdit;

		public event EventHandler<EntityViewItemsEventArgs<TEntity>> ItemsChange;

		public event EventHandler<EntityViewItemsEventArgs<TEntity>> SourceChange;

		public event EventHandler<EntityViewUpdateEventArgs> Updated;

		public event EventHandler<EntityViewUpdateEventArgs> Updating;

		public event EventHandler<EntityViewItemsEventArgs<TViewObject>> ViewObjectsChange;

		#endregion




		#region Instance Methods

		public bool CanNew ()
		{
			return this.EntityCanNew();
		}

		public TViewObject GetViewObjectForEntity (TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return (from x in this.ViewObjects where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}

		public TViewObject New ()
		{
			TViewObject viewObj = this.PrepareViewObject(null, null);
			this.ViewObjects.Add(viewObj);
			return viewObj;
		}

		public void UpdateItems ()
		{
			this.UpdateItems(false);
		}

		private void ItemsChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if (this.SuppressItemsChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressItemsChangeHandling = true;

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

							if (!this.SuppressViewObjectChangeHandling)
							{
								this.ViewObjects.Remove(viewObj);
							}
						}
						foreach (TEntity entity in newItems)
						{
							TViewObject viewObject = this.PrepareViewObject(null, entity);

							if (!this.SuppressViewObjectChangeHandling)
							{
								this.ViewObjects.Add(viewObject);
							}
						}

						this.OnPropertyChanged(nameof(this.Items));
						this.OnItemsChanged(oldItems, newItems);

						break;
					}
				}
			}
			finally
			{
				this.SuppressItemsChangeHandling = false;
			}
		}

		private TViewObject PrepareViewObject (TViewObject viewObj, TEntity entityCandidate)
		{
			viewObj = viewObj ?? this.CreateViewObject();

			viewObj.Entity = (viewObj.Entity ?? entityCandidate) ?? this.CreateEntity();
			viewObj.ViewCaller = this;

			return viewObj;
		}

		private void SourceChanged (object sender, NotifyCollectionChangedEventArgs e)
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

							if (!this.SuppressViewObjectChangeHandling)
							{
								this.ViewObjects.Remove(viewObj);
							}
						}
						foreach (TEntity entity in newItems)
						{
							TViewObject viewObject = this.PrepareViewObject(null, entity);

							if (!this.SuppressViewObjectChangeHandling)
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

		private void UpdateItems (bool resetPageNumber)
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

				if (resetPageNumber && (this.PageNumber > 1))
				{
					this.PageNumber = 1;
				}

				this.OnUpdatingItems(resetPageNumber);

				IList<TEntity> oldItems = (IList<TEntity>)this.Items ?? new List<TEntity>();
				IList<TViewObject> oldViewObjects = (IList<TViewObject>)this.ViewObjects ?? new List<TViewObject>();

				if (this.ViewObjects != null)
				{
					this.ViewObjects.CollectionChanged -= this.ViewObjectsChangedHandler;
					this.ViewObjects = null;
				}

				if (this.Items != null)
				{
					this.Items.CollectionChanged -= this.ItemsChangedHandler;
					this.Items = null;
				}

				int entityTotalCount = 0;
				int entityFilteredCount = 0;
				int pageFilteredCount = 0;

				List<TEntity> entities = this.PageNumber == 0 ? new List<TEntity>() : this.Set.GetFiltered(this.Source, this.Filter, this.PageNumber - 1, this.PageSize, out entityTotalCount, out entityFilteredCount, out pageFilteredCount).ToList();
				List<TViewObject> viewObjects = (from x in entities select this.PrepareViewObject(null, x)).ToList();

				int pageTotalCount = this.PageNumber == 0 ? 0 : (this.PageSize == 0 ? 1 : ((entityTotalCount / this.PageSize) + (((entityTotalCount % this.PageSize) == 0) ? 0 : 1)));

				this.Items = new ObservableCollection<TEntity>(entities);
				this.Items.CollectionChanged += this.ItemsChangedHandler;

				this.ViewObjects = new ObservableCollection<TViewObject>(viewObjects);
				this.ViewObjects.CollectionChanged += this.ViewObjectsChangedHandler;

				this.EntityTotalCount = entityTotalCount;
				this.EntityFilteredCount = entityFilteredCount;
				this.PageTotalCount = pageTotalCount;
				this.PageFilteredCount = pageFilteredCount;

				this.OnPropertyChanged(nameof(this.Items));
				this.OnItemsChanged(oldItems, this.Items);

				this.OnPropertyChanged(nameof(this.ViewObjects));
				this.OnViewObjectsChanged(oldViewObjects, this.ViewObjects);

				this.OnPropertyChanged(nameof(this.EntityTotalCount));
				this.OnPropertyChanged(nameof(this.EntityFilteredCount));
				this.OnPropertyChanged(nameof(this.PageTotalCount));
				this.OnPropertyChanged(nameof(this.PageFilteredCount));

				this.OnUpdatedItems(resetPageNumber);
			}
			finally
			{
				this.IsUpdating = false;
			}
		}

		private void ViewObjectsChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if (this.SuppressViewObjectChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressViewObjectChangeHandling = true;

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
							this.EntityDelete(viewObj);

							if (!this.SuppressItemsChangeHandling)
							{
								this.Items.Remove(viewObj.Entity);
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

							if (!this.SuppressItemsChangeHandling)
							{
								this.Items.Add(viewObj.Entity);
							}

							if (this.ItemToAddIsAttached)
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

						break;
					}
				}
			}
			finally
			{
				this.SuppressViewObjectChangeHandling = false;
			}
		}

		#endregion




		#region Virtuals

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

			this.Set.Delete(viewObject.Entity);
		}

		protected virtual void EntityEditBegin (TViewObject viewObject)
		{
			if (viewObject.IsEdited)
			{
				return;
			}
			viewObject.IsEdited = true;

			viewObject.RaiseEntityChanged();

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

			this.EntityEndEdit?.Invoke(this, new EntityViewItemEventArgs<TViewObject>(viewObject));
		}

		protected virtual void EntityModify (TViewObject viewObject)
		{
			this.Set.Modify(viewObject.Entity);

			viewObject.RaiseEntityChanged();

			viewObject.IsModified = true;
		}

		protected virtual void EntityReload (TViewObject viewObject)
		{
			this.Set.Reload(viewObject.Entity);

			viewObject.RaiseEntityChanged();

			viewObject.Errors = null;
			viewObject.IsModified = false;
		}

		protected virtual void EntityValidate (TViewObject viewObject)
		{
			viewObject.Errors = this.Set.Validate(viewObject.Entity);
			viewObject.IsModified = this.Set.IsModified(viewObject.Entity);

			viewObject.RaiseEntityChanged();
		}

		protected virtual void OnItemsChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.ItemsChange?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
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
		///     Handles the change of a property value by raising the <see cref="PropertyChanging" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which is about to be changed. </param>
		protected virtual void OnPropertyChanging (string propertyName)
		{
			this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		protected virtual void OnSourceChanged (IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			this.SourceChange?.Invoke(this, new EntityViewItemsEventArgs<TEntity>(oldItems, newItems));
		}

		protected virtual void OnUpdatedItems (bool resetPageNumber)
		{
			this.Updated?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		protected virtual void OnUpdatingItems (bool resetPageNumber)
		{
			this.Updating?.Invoke(this, new EntityViewUpdateEventArgs(resetPageNumber));
		}

		protected virtual void OnViewObjectsChanged (IList<TViewObject> oldItems, IList<TViewObject> newItems)
		{
			this.ViewObjectsChange?.Invoke(this, new EntityViewItemsEventArgs<TViewObject>(oldItems, newItems));
		}

		#endregion




		#region Interface: IEntityViewCaller<TEntity>

		public void Add (TEntity entity)
		{
			this.Items.Add(entity);
		}

		public void Attach (TEntity entity)
		{
			try
			{
				this.ItemToAddIsAttached = true;
				this.Items.Add(entity);
			}
			finally
			{
				this.ItemToAddIsAttached = false;
			}
		}

		public void BeginEdit (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditBegin(viewObject);
		}

		public bool CanAdd (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAdd(viewObject);
		}

		public bool CanAttach (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanAttach(viewObject);
		}

		public void CancelEdit (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditCancel(viewObject);
		}

		public bool CanDelete (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanDelete(viewObject);
		}

		public bool CanEdit (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanEdit(viewObject);
		}

		public bool CanModify (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanModify(viewObject);
		}

		public bool CanReload (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanReload(viewObject);
		}

		public bool CanValidate (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			return this.EntityCanValidate(viewObject);
		}

		public void Delete (TEntity entity)
		{
			this.Items.Remove(entity);
		}

		public void EndEdit (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityEditEnd(viewObject);
		}

		public void Modify (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityModify(viewObject);
		}

		public void Reload (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityReload(viewObject);
		}

		public void Validate (TEntity entity)
		{
			TViewObject viewObject = this.GetViewObjectForEntity(entity);
			this.EntityValidate(viewObject);
		}

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion




		#region Interface: INotifyPropertyChanging

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

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
