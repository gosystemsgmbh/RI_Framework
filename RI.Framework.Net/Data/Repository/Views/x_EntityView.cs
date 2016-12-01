using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using RI.Framework.Data.Repository;




namespace ConsoleApplication1
{
	//TODO: OnXXX for Source, Filter, PageSize, PageNumber
	//TODO: Delete, CanDelete
	//TODO: Edit, CanEdit

	public class EntityView<TEntity, TViewObject> : INotifyPropertyChanged, INotifyPropertyChanging, IEntityViewCaller
		where TEntity : class, new()
		where TViewObject : EntityViewObject<TEntity>, new()
	{
		private IEnumerable<TEntity> _source;

		private object _filter;

		private int _pageSize;

		private int _pageNumber;







		public EntityView(IRepositorySet<TEntity> set)
			: this(set, null)
		{
		}

		public EntityView(IRepositorySet<TEntity> set, IEnumerable<TEntity> source)
		{
			if(set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.IsInitializing = true;

			this.Set = set;
			this.Source = source;

			this.ItemsChangedHandler = this.ItemsChanged;
			this.ViewObjectsChangedHandler = this.ViewObjectsChanged;

			this.IsUpdating = false;
			this.SuppressItemsChangeHandling = false;
			this.SuppressViewObjectChangeHandling = false;

			this.Items = new ObservableCollection<TEntity>();
			this.Items.CollectionChanged += this.ItemsChangedHandler;

			this.ViewObjects = new ObservableCollection<TViewObject>();
			this.ViewObjects.CollectionChanged += this.ViewObjectsChangedHandler;

			this.Filter = null;
			this.PageSize = 100;
			this.PageNumber = 1;

			this.TotalCount = 0;
			this.FilteredCount = 0;
			this.PageCount = 0;

			this.IsInitializing = false;

			this.UpdateItems(false);
		}

		public IRepositorySet<TEntity> Set { get; private set; }

		protected bool IsInitializing { get; private set;}

		protected bool IsUpdating { get; private set; }

		protected bool SuppressViewObjectChangeHandling { get; private set; }

		protected bool SuppressItemsChangeHandling { get; private set; }

		private NotifyCollectionChangedEventHandler ItemsChangedHandler { get; set; }
		
		private NotifyCollectionChangedEventHandler ViewObjectsChangedHandler { get; set; }







		public IEnumerable<TEntity> Source
		{
			get
			{
				return this._source;
			}
			set
			{
				this.OnPropertyChanging(nameof(this.Source));
				this._source = value;
				this.OnPropertyChanged(nameof(this.Source));

				this.UpdateItems(true);
			}
		}

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

		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
			set
			{
				if(value < 1)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this.OnPropertyChanging(nameof(this.PageSize));
				this._pageSize = value;
				this.OnPropertyChanged(nameof(this.PageSize));

				this.UpdateItems(true);
			}
		}

		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				//TODO: check for max page number

				this.OnPropertyChanging(nameof(this.PageNumber));
				this._pageNumber = value;
				this.OnPropertyChanged(nameof(this.PageNumber));

				this.UpdateItems(false);
			}
		}

		public int TotalCount { get; private set; }

		public int FilteredCount { get; private set; }

		public int PageCount { get; private set; }





		public ObservableCollection<TEntity> Items { get; private set; }

		public ObservableCollection<TViewObject> ViewObjects { get; private set; }

		private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
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
								//TODO: implement (set delete)
							}
							foreach (TEntity entity in newItems)
							{
								//TODO: implement (set add)
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

		private void ViewObjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInitializing)
			{
				return;
			}

			if (this.IsUpdating)
			{
				return;
			}

			if(this.SuppressViewObjectChangeHandling)
			{
				return;
			}

			try
			{
				this.SuppressViewObjectChangeHandling = true;

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
								this.Items.Remove(viewObj.Entity);
							}
							foreach (TViewObject viewObj in newItems)
							{
								viewObj.Entity = viewObj.Entity ?? this.CreateEntity();
								viewObj.ViewCaller = this;

								this.Items.Add(viewObj.Entity);
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

		public void UpdateItems()
		{
			this.UpdateItems(false);
		}

		private void UpdateItems(bool resetPageNumber)
		{
			if(this.IsInitializing)
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

				if(resetPageNumber)
				{
					this.PageNumber = 1;
				}

				this.OnUpdatingItems(resetPageNumber);

				this.ViewObjects.CollectionChanged -= this.ViewObjectsChangedHandler;
				this.ViewObjects = null;

				this.Items.CollectionChanged -= this.ItemsChangedHandler;
				this.Items = null;

				int totalCount = this.Set.GetCount();
				int entityCount = 0;
				int pageCount = 0;

				IEnumerable<TEntity> entities = this.Set.GetFiltered(this.Source, this.Filter, this.PageNumber - 1, this.PageSize, out entityCount, out pageCount);
				IEnumerable<TViewObject> viewObjects = from x in entities select new TViewObject { Entity = x, ViewCaller = this };

				this.Items = new ObservableCollection<TEntity>(entities);
				this.Items.CollectionChanged += this.ItemsChangedHandler;

				this.ViewObjects = new ObservableCollection<TViewObject>(viewObjects);
				this.ViewObjects.CollectionChanged += this.ViewObjectsChangedHandler;

				this.TotalCount = totalCount;
				this.FilteredCount = entityCount;
				this.PageCount = pageCount;

				//TODO: invoke, build difference
				this.OnPropertyChanged(nameof(this.Items));
				//this.OnItemsChanged(oldItems, newItems);

				//TODO: invoke, build difference
				this.OnPropertyChanged(nameof(this.ViewObjects));
				//this.OnViewObjectsChanged(oldItems, newItems);

				this.OnPropertyChanged(nameof(this.TotalCount));
				this.OnPropertyChanged(nameof(this.FilteredCount));
				this.OnPropertyChanged(nameof(this.PageCount));

				this.OnUpdatedItems(resetPageNumber);
			}
			finally
			{
				this.IsUpdating = false;
			}
		}

		protected virtual void OnUpdatingItems(bool resetPageNumber)
		{
			//TODO: event
		}

		protected virtual void OnUpdatedItems(bool resetPageNumber)
		{
			//TODO: event
		}

		protected virtual void OnItemsChanged(IList<TEntity> oldItems, IList<TEntity> newItems)
		{
			//TODO: event
		}

		protected virtual void OnViewObjectsChanged(IList<TViewObject> oldItems, IList<TViewObject> newItems)
		{
			//TODO: event
		}

		public TViewObject GetViewObjectForEntity(TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return (from x in this.ViewObjects where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}










		protected virtual TEntity CreateEntity()
		{
			return this.Set.Create();
		}

		public virtual bool CanNew()
		{
			return this.Set.CanCreate();
		}

		public TViewObject New()
		{
			TViewObject viewObj = new TViewObject();
			this.ViewObjects.Add(viewObj);
			return viewObj;
		}










		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanging" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which is about to be changed. </param>
		protected virtual void OnPropertyChanging(string propertyName)
		{
			this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}
	}

	internal interface IEntityViewCaller
	{

	}
}
