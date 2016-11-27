using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using RI.Framework.Collections;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityView<T> : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
		where T : class
	{
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
		
		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

		void IDisposable.Dispose()
		{
			this.ReleaseEntities();
		}






		private ObservableCollection<EntityViewObject> _editedEntities;

		private ObservableCollection<EntityViewObject> _entities;

		private ObservableCollection<EntityViewObject> _selectedEntities;

		private int _totalCount;

		private int _pageCount;

		private IRepositorySet<T> _set;

		private object _filter;

		private int _pageNumber;

		private int _pageSize;

		public ObservableCollection<EntityViewObject> Entities
		{
			get
			{
				return this._entities;
			}
			private set
			{
				IList<EntityViewObject> oldValue = this._entities;
				this._entities = value;
				IList<EntityViewObject> newValue = this._entities;
				this.OnPropertyChanged(nameof(this.Entities));
				this.OnEntitiesChanged(oldValue ?? new List<EntityViewObject>(), newValue ?? new List<EntityViewObject>());
			}
		}

		public ObservableCollection<EntityViewObject> EditedEntities
		{
			get
			{
				return this._editedEntities;
			}
			private set
			{
				IList<EntityViewObject> oldValue = this._editedEntities;
				this._editedEntities = value;
				IList<EntityViewObject> newValue = this._editedEntities;
				this.OnPropertyChanged(nameof(this.EditedEntities));
				this.OnPropertyChanged(nameof(this.EditedEntity));
				this.OnEditedEntitiesChanged(oldValue ?? new List<EntityViewObject>(), newValue ?? new List<EntityViewObject>());
			}
		}

		public ObservableCollection<EntityViewObject> SelectedEntities
		{
			get
			{
				return this._selectedEntities;
			}
			private set
			{
				IList<EntityViewObject> oldValue = this._selectedEntities;
				this._selectedEntities = value;
				IList<EntityViewObject> newValue = this._selectedEntities;
				this.OnPropertyChanged(nameof(this.SelectedEntities));
				this.OnPropertyChanged(nameof(this.SelectedEntity));
				this.OnSelectedEntitiesChanged(oldValue ?? new List<EntityViewObject>(), newValue ?? new List<EntityViewObject>());
			}
		}

		public EntityViewObject EditedEntity
		{
			get
			{
				if (this.EditedEntities == null)
				{
					return null;
				}

				if (this.EditedEntities.Count == 0)
				{
					return null;
				}

				return this.EditedEntities[0];
			}
			set
			{
				if (this.EditedEntities == null)
				{
					return;
				}

				this.EditedEntities.RemoveWhere(x => !object.ReferenceEquals(x, value));

				if (!this.EditedEntities.Contains(value))
				{
					this.EditedEntities.Add(value);
				}
			}
		}

		public EntityViewObject SelectedEntity
		{
			get
			{
				if (this.SelectedEntities == null)
				{
					return null;
				}

				if (this.SelectedEntities.Count == 0)
				{
					return null;
				}

				return this.SelectedEntities[0];
			}
			set
			{
				if (this.SelectedEntities == null)
				{
					return;
				}

				this.SelectedEntities.RemoveWhere(x => !object.ReferenceEquals(x, value));

				if (!this.SelectedEntities.Contains(value))
				{
					this.SelectedEntities.Add(value);
				}
			}
		}

		public int TotalCount
		{
			get
			{
				return this._totalCount;
			}
			private set
			{
				this._totalCount = value;
				this.OnPropertyChanged(nameof(this.TotalCount));
			}
		}

		public int PageCount
		{
			get
			{
				return this._pageCount;
			}
			private set
			{
				this._pageCount = value;
				this.OnPropertyChanged(nameof(this.PageCount));
			}
		}

		public IRepositorySet<T> Set
		{
			get
			{
				return this._set;
			}
			private set
			{
				this._set = value;
				this.OnPropertyChanged(nameof(this.Set));
			}
		}
		
		public object Filter
		{
			get
			{
				return this._filter;
			}
			private set
			{
				this._filter = value;
				this.OnPropertyChanged(nameof(this.Filter));
			}
		}

		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			private set
			{
				this._pageNumber = value;
				this.OnPropertyChanged(nameof(this.PageNumber));
			}
		}

		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
			private set
			{
				this._pageSize = value;
				this.OnPropertyChanged(nameof(this.PageSize));
			}
		}
		
		private NotifyCollectionChangedEventHandler EditedEntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler EntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler SelectedEntitiesChangedHandler { get; set; }

		public EntityView ()
		{
			this.EntitiesChangedHandler = this.EntitiesChanged;
			this.EditedEntitiesChangedHandler = this.EditedEntitiesChanged;
			this.SelectedEntitiesChangedHandler = this.SelectedEntitiesChanged;

			this.Filter = null;
			this.PageNumber = 1;
			this.PageSize = 1000;

			this.Set = null;

			this.Entities = null;
			this.EditedEntities = null;
			this.SelectedEntities = null;

			this.TotalCount = 0;
			this.PageCount = 0;
		}







		public void SetFilter(string filter, bool immediate)
		{
			this.OnFilterChanging();

			this.FilterInternal = filter;

			this.OnPropertyChanged(nameof(this.Filter));

			this.OnFilterChanged();

			this.UpdatePageFilter(immediate);
		}

		public void SetPage(int pageNumber, int pageSize, bool immediate)
		{
			this.OnPageNumberChanging();
			this.OnPageSizeChanging();

			this.PageNumberInternal = pageNumber;
			this.PageSizeInternal = pageSize;

			this.OnPropertyChanged(nameof(this.PageNumber));
			this.OnPropertyChanged(nameof(this.PageSize));

			this.OnPageNumberChanged();
			this.OnPageSizeChanged();

			this.UpdatePageFilter(immediate);
		}

		protected EntityViewObject GetViewObjectForEntity(T entity)
		{
			return this.Entities == null ? null : (from x in this.Entities where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}

		protected void AcquireEntities(IRepositorySet<T> set)
		{
			this.ReleaseEntities();

			this.OnEntitiesAcquiring();

			int entityCount;
			int pageCount;

			IEnumerable<TEntity> entities = this.GetEntitiesFromSet(out entityCount, out pageCount);

			this.Entities = new ObservableCollection<EntityViewObject>(from x in entities
																	   select new EntityViewObject
																	   {
																		   ViewModel = this,
																		   Entity = x,
																		   IsNew = false
																	   });

			this.EditedEntities = new ObservableCollection<EntityViewObject>();
			this.SelectedEntities = new ObservableCollection<EntityViewObject>();

			this.Entities.CollectionChanged += this.EntitiesChangedHandler;
			this.EditedEntities.CollectionChanged += this.EditedEntitiesChangedHandler;
			this.SelectedEntities.CollectionChanged += this.SelectedEntitiesChangedHandler;

			this.TotalCount = entityCount;
			this.PageCount = pageCount;

			this.OnEntitiesAcquired();
		}

		protected void ReleaseEntities()
		{
			bool isReleasing = (this.Entities != null) || (this.EditedEntities != null) || (this.SelectedEntities != null);

			if (isReleasing)
			{
				this.OnEntitiesReleasing();
			}

			if (this.PageCount != 0)
			{
				this.PageCount = 0;
			}

			if (this.TotalCount != 0)
			{
				this.TotalCount = 0;
			}

			if (this.SelectedEntities != null)
			{
				this.SelectedEntities.CollectionChanged -= this.SelectedEntitiesChangedHandler;
				this.SelectedEntities = null;
			}

			if (this.EditedEntities != null)
			{
				this.EditedEntities.CollectionChanged -= this.EditedEntitiesChangedHandler;
				this.EditedEntities = null;
			}

			if (this.Entities != null)
			{
				this.Entities.CollectionChanged -= this.EntitiesChangedHandler;
				this.Entities = null;
			}

			if (isReleasing)
			{
				this.OnEntitiesReleased();
			}
		}

		private void EntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
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
						List<EntityViewObject> oldItems = new List<EntityViewObject>();
						List<EntityViewObject> newItems = new List<EntityViewObject>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<EntityViewObject>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<EntityViewObject>());
						}

						foreach (EntityViewObject viewObj in oldItems)
						{
							viewObj.Delete();
						}
						foreach (EntityViewObject viewObj in newItems)
						{
							this.EntityAdd(viewObj);
						}

						this.OnPropertyChanged(nameof(this.Entities));
						this.OnEntitiesChanged(oldItems, newItems);

						break;
					}
			}
		}

		private void EditedEntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
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
						List<EntityViewObject> oldItems = new List<EntityViewObject>();
						List<EntityViewObject> newItems = new List<EntityViewObject>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<EntityViewObject>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<EntityViewObject>());
						}

						foreach (EntityViewObject viewObj in oldItems)
						{
							viewObj.EndEdit();
						}
						foreach (EntityViewObject viewObj in newItems)
						{
							if (this.Entities.Contains(viewObj))
							{
								viewObj.BeginEdit();
							}
							else
							{
								throw new InvalidOperationException("The entity to edit is not in the collection of acquired entities.");
							}
						}

						this.OnPropertyChanged(nameof(this.EditedEntities));
						this.OnPropertyChanged(nameof(this.EditedEntity));
						this.OnEditedEntitiesChanged(oldItems, newItems);

						break;
					}
			}
		}

		private void SelectedEntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
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
						List<EntityViewObject> oldItems = new List<EntityViewObject>();
						List<EntityViewObject> newItems = new List<EntityViewObject>();

						if (e.OldItems != null)
						{
							oldItems.AddRange(e.OldItems.OfType<EntityViewObject>());
						}
						if (e.NewItems != null)
						{
							newItems.AddRange(e.NewItems.OfType<EntityViewObject>());
						}

						foreach (EntityViewObject viewObj in oldItems)
						{
							viewObj.Deselect();
						}
						foreach (EntityViewObject viewObj in newItems)
						{
							if (this.Entities.Contains(viewObj))
							{
								viewObj.Select();
							}
							else
							{
								throw new InvalidOperationException("The entity to select is not in the collection of acquired entities.");
							}
						}

						this.OnPropertyChanged(nameof(this.SelectedEntities));
						this.OnPropertyChanged(nameof(this.SelectedEntity));
						this.OnSelectedEntitiesChanged(oldItems, newItems);

						break;
					}
			}
		}

		private void EntityAdd(EntityViewObject viewObj)
		{
			if (viewObj.IsAdded)
			{
				return;
			}
			viewObj.IsAdded = true;

			viewObj.ViewModel = this;
			viewObj.Entity = viewObj.Entity ?? this.CreateEntityFromSet();
			viewObj.IsNew = true;

			this.OnEntityAdding(viewObj);
			this.Set.Add(viewObj.Entity);
			if (!this.Entities.Contains(viewObj))
			{
				this.Entities.Add(viewObj);
			}
			this.OnEntityAdded(viewObj);

			if (this.SelectOnAdd)
			{
				viewObj.Select();
			}

			if (this.EditOnAdd)
			{
				viewObj.BeginEdit();
			}

			if (this.ValidateOnAdd)
			{
				viewObj.Validate();
			}
		}

		private void EntityDeselect(EntityViewObject viewObj)
		{
			if (!viewObj.IsSelected)
			{
				return;
			}
			viewObj.IsSelected = false;

			this.SelectedEntities.Remove(viewObj);

			this.OnEntityDeselect(viewObj);
		}

		private void EntityEditBegin(EntityViewObject viewObj)
		{
			if (viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = true;

			if (!this.EditedEntities.Contains(viewObj))
			{
				this.EditedEntities.Add(viewObj);
			}

			viewObj.IsModified = false;
			viewObj.Errors = null;

			this.OnEntityEditBegin(viewObj);
		}

		private void EntityEditCancel(EntityViewObject viewObj)
		{
			if (!viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = false;

			if ((!viewObj.IsNew) && (!viewObj.IsDeleted))
			{
				this.Set.Reload(viewObj.Entity);
			}

			this.EditedEntities.Remove(viewObj);

			viewObj.Validate();

			this.OnEntityEditCancel(viewObj);

			if (viewObj.IsNew && this.DeleteNewOnCancelEdit)
			{
				viewObj.Delete();
			}
		}

		private void EntityEditEnd(EntityViewObject viewObj)
		{
			if (!viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = false;

			if (this.ExplicitModifyOnEndEdit)
			{
				this.Set.Modify(viewObj.Entity);
			}

			this.EditedEntities.Remove(viewObj);

			viewObj.Validate();

			this.OnEntityEditEnd(viewObj);
		}

		private void EntityDelete(EntityViewObject viewObj)
		{
			if (!viewObj.IsAdded)
			{
				return;
			}
			viewObj.IsAdded = false;

			viewObj.EndEdit();
			viewObj.Deselect();

			viewObj.IsDeleted = true;

			this.OnEntityDeleting(viewObj);
			this.Entities.Remove(viewObj);
			this.Set.Delete(viewObj.Entity);
			this.OnEntityDeleted(viewObj);
		}

		private void EntitySelect(EntityViewObject viewObj)
		{
			if (viewObj.IsSelected)
			{
				return;
			}
			viewObj.IsSelected = true;

			if (!this.SelectedEntities.Contains(viewObj))
			{
				this.SelectedEntities.Add(viewObj);
			}

			this.OnEntitySelect(viewObj);
		}

		private void EntityValidate(EntityViewObject viewObj)
		{
			this.OnEntityValidating(viewObj);

			viewObj.IsModified = this.Set.IsModified(viewObj.Entity);
			viewObj.Errors = this.Set.Validate(viewObj.Entity);

			this.OnEntityValidated(viewObj);
		}

		private void ResetNew()
		{
			if (this.Entities != null)
			{
				foreach (EntityViewObject viewObj in this.Entities)
				{
					viewObj.IsNew = false;
				}
			}
		}

		private void ResetValidation()
		{
			if (this.Entities != null)
			{
				foreach (EntityViewObject viewObj in this.Entities)
				{
					viewObj.IsModified = false;
					viewObj.Errors = null;
				}
			}
		}

		private void ResetEntities()
		{
			if (this.Entities != null)
			{
				this.AcquireEntities();
			}
		}

		protected virtual T CreateEntityFromSet()
		{
			return this.Set.Create();
		}

		protected virtual IEnumerable<T> GetEntitiesFromSet(out int entityCount, out int pageCount)
		{
			return this.Set.GetFiltered(this.Filter, x => x.Id, this.PageNumber - 1, this.PageSize, out entityCount, out pageCount);
		}















		protected virtual void OnEditedEntitiesChanged(IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		protected virtual void OnEntitiesChanged(IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		protected virtual void OnSelectedEntitiesChanged(IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		protected virtual void OnEntitiesAcquired()
		{
		}

		protected virtual void OnEntitiesAcquiring()
		{
		}

		protected virtual void OnEntitiesReleased()
		{
		}

		protected virtual void OnEntitiesReleasing()
		{
		}

		protected virtual void OnEntityAdded(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityAdding(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeselect(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditBegin(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditCancel(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditEnd(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeleted(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeleting(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntitySelect(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityValidated(EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityValidating(EntityViewObject viewObj)
		{
		}

		protected virtual void OnFilterChanged()
		{
		}

		protected virtual void OnFilterChanging()
		{
		}

		protected virtual void OnPageNumberChanged()
		{
		}

		protected virtual void OnPageNumberChanging()
		{
		}

		protected virtual void OnPageSizeChanged()
		{
		}

		protected virtual void OnPageSizeChanging()
		{
		}
	}
}