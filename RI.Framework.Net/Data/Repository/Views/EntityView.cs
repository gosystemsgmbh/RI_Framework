using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using RI.Framework.Collections;
using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	//TODO: OnPage, etc. changed event
	//TODO: Comments
	public class EntityView <TEntity> : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
		where TEntity : class
	{
		#region Instance Constructor/Destructor

		public EntityView (IRepositorySet<TEntity> set, Expression<Func<TEntity, object>> sorter)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (sorter == null)
			{
				throw new ArgumentNullException(nameof(sorter));
			}

			this.Set = set;

			this.EntitiesChangedHandler = this.EntitiesChanged;
			this.EditedEntitiesChangedHandler = this.EditedEntitiesChanged;
			this.SelectedEntitiesChangedHandler = this.SelectedEntitiesChanged;

			this.Entities = null;
			this.EditedEntities = null;
			this.SelectedEntities = null;

			this.TotalCount = 0;
			this.PageCount = 0;

			this.Sequence = null;

			this.Filter = null;
			this.Sorter = sorter;
			this.PageNumber = 1;
			this.PageSize = 1000;
		}

		#endregion




		#region Instance Fields

		private object _filter;

		private int _pageNumber;

		private int _pageSize;

		private Expression<Func<TEntity, object>> _sorter;

		#endregion




		#region Instance Properties/Indexer

		public bool DeleteNewOnCancelEdit { get; set; }

		public ObservableCollection<EntityViewObject> EditedEntities { get; private set; }

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

		public bool EditOnAdd { get; set; }

		public ObservableCollection<EntityViewObject> Entities { get; private set; }

		public bool ExplicitModifyOnEndEdit { get; set; }

		public object Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				bool resetPageNumber = !object.ReferenceEquals(value, this._filter);

				this.OnPropertyChanging(nameof(this.Filter));
				this._filter = value;
				this.OnPropertyChanged(nameof(this.Filter));

				if (this.Entities != null)
				{
					this.AcquireEntities(this.Sequence, resetPageNumber);
				}
			}
		}

		public int PageCount { get; private set; }

		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			private set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this.OnPropertyChanging(nameof(this.PageNumber));
				this._pageNumber = value;
				this.OnPropertyChanged(nameof(this.PageNumber));

				if (this.Entities != null)
				{
					this.AcquireEntities(this.Sequence, false);
				}
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
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				bool resetPageNumber = value != this._pageSize;

				this.OnPropertyChanging(nameof(this.PageSize));
				this._pageSize = value;
				this.OnPropertyChanged(nameof(this.PageSize));

				if (this.Entities != null)
				{
					this.AcquireEntities(this.Sequence, resetPageNumber);
				}
			}
		}

		public ObservableCollection<EntityViewObject> SelectedEntities { get; private set; }

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

		public bool SelectOnAdd { get; set; }

		public List<TEntity> Sequence { get; private set; }

		public IRepositorySet<TEntity> Set { get; private set; }

		public Expression<Func<TEntity, object>> Sorter
		{
			get
			{
				return this._sorter;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				this.OnPropertyChanging(nameof(this.Sorter));
				this._sorter = value;
				this.OnPropertyChanged(nameof(this.Sorter));

				if (this.Entities != null)
				{
					this.AcquireEntities(this.Sequence, false);
				}
			}
		}

		public int TotalCount { get; private set; }

		public bool ValidateOnAdd { get; set; }

		private NotifyCollectionChangedEventHandler EditedEntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler EntitiesChangedHandler { get; set; }

		private NotifyCollectionChangedEventHandler SelectedEntitiesChangedHandler { get; set; }

		#endregion




		#region Instance Methods

		public void AcquireEntities ()
		{
			this.AcquireEntities(null, true);
		}

		public void AcquireEntities (IEnumerable<TEntity> sequence)
		{
			this.AcquireEntities(sequence, true);
		}

		public EntityViewObject GetViewObjectForEntity (TEntity entity)
		{
			return this.Entities == null ? null : (from x in this.Entities where object.ReferenceEquals(x.Entity, entity) select x).FirstOrDefault();
		}

		public void ReleaseEntities ()
		{
			IList<EntityViewObject> entities = this.Entities;
			IList<EntityViewObject> selectedEntities = this.SelectedEntities;
			IList<EntityViewObject> editedEntities = this.EditedEntities;

			bool isReleasing = (entities != null) || (selectedEntities != null) || (editedEntities != null);

			if (isReleasing)
			{
				this.OnEntitiesReleasing();

				this.OnPropertyChanging(nameof(this.Entities));
				this.OnPropertyChanging(nameof(this.SelectedEntities));
				this.OnPropertyChanging(nameof(this.EditedEntities));

				this.OnPropertyChanging(nameof(this.SelectedEntity));
				this.OnPropertyChanging(nameof(this.EditedEntity));

				this.OnPropertyChanging(nameof(this.TotalCount));
				this.OnPropertyChanging(nameof(this.PageCount));

				this.OnPropertyChanging(nameof(this.Sequence));
			}

			if (this.Sequence != null)
			{
				this.Sequence = null;
			}

			if (this.PageCount != 0)
			{
				this.PageCount = 0;
			}

			if (this.TotalCount != 0)
			{
				this.TotalCount = 0;
			}

			if (this.EditedEntities != null)
			{
				this.EditedEntities.CollectionChanged -= this.EditedEntitiesChangedHandler;
				this.EditedEntities = null;
			}

			if (this.SelectedEntities != null)
			{
				this.SelectedEntities.CollectionChanged -= this.SelectedEntitiesChangedHandler;
				this.SelectedEntities = null;
			}

			if (this.Entities != null)
			{
				this.Entities.CollectionChanged -= this.EntitiesChangedHandler;
				this.Entities = null;
			}

			if (isReleasing)
			{
				this.OnPropertyChanged(nameof(this.Entities));
				this.OnPropertyChanged(nameof(this.SelectedEntities));
				this.OnPropertyChanged(nameof(this.EditedEntities));

				this.OnPropertyChanged(nameof(this.SelectedEntity));
				this.OnPropertyChanged(nameof(this.EditedEntity));

				this.OnPropertyChanged(nameof(this.TotalCount));
				this.OnPropertyChanged(nameof(this.PageCount));

				this.OnPropertyChanged(nameof(this.Sequence));

				this.OnEntitiesChanged(entities ?? new List<EntityViewObject>(), new List<EntityViewObject>());
				this.OnSelectedEntitiesChanged(selectedEntities ?? new List<EntityViewObject>(), new List<EntityViewObject>());
				this.OnEditedEntitiesChanged(editedEntities ?? new List<EntityViewObject>(), new List<EntityViewObject>());

				this.OnEntitiesReleased();
			}
		}

		private void AcquireEntities (IEnumerable<TEntity> sequence, bool resetPageNumber)
		{
			this.ReleaseEntities();

			this.OnEntitiesAcquiring();

			this.OnPropertyChanging(nameof(this.Entities));
			this.OnPropertyChanging(nameof(this.SelectedEntities));
			this.OnPropertyChanging(nameof(this.EditedEntities));

			this.OnPropertyChanging(nameof(this.SelectedEntity));
			this.OnPropertyChanging(nameof(this.EditedEntity));

			this.OnPropertyChanging(nameof(this.TotalCount));
			this.OnPropertyChanging(nameof(this.PageCount));

			this.OnPropertyChanging(nameof(this.Sequence));

			List<TEntity> sequenceList = sequence?.ToList();

			int pageIndex = resetPageNumber ? 0 : (this.PageNumber - 1);

			int entityCount = 0;
			int pageCount = 0;
			IEnumerable<TEntity> filteredEntities = null;
			if (sequenceList == null)
			{
				filteredEntities = this.Set.GetFiltered(this.Filter, this.Sorter, pageIndex, this.PageSize, out entityCount, out pageCount);
			}
			else
			{
				filteredEntities = this.Set.GetFiltered(sequenceList, this.Filter, this.Sorter, pageIndex, this.PageSize, out entityCount, out pageCount);
			}

			IEnumerable<EntityViewObject> viewObjects = from x in filteredEntities
			                                            select new EntityViewObject
			                                            {
				                                            View = this,
				                                            Entity = x
			                                            };

			ObservableCollection<EntityViewObject> allEntities = new ObservableCollection<EntityViewObject>(viewObjects);
			ObservableCollection<EntityViewObject> selectedEntities = new ObservableCollection<EntityViewObject>();
			ObservableCollection<EntityViewObject> editedEntities = new ObservableCollection<EntityViewObject>();

			allEntities.CollectionChanged += this.EntitiesChangedHandler;
			selectedEntities.CollectionChanged += this.SelectedEntitiesChangedHandler;
			editedEntities.CollectionChanged += this.EditedEntitiesChangedHandler;

			this.Entities = allEntities;
			this.SelectedEntities = selectedEntities;
			this.EditedEntities = editedEntities;

			this.TotalCount = entityCount;
			this.PageCount = pageCount;

			this.Sequence = sequenceList;

			this.PageNumber = pageIndex + 1;

			this.OnPropertyChanged(nameof(this.Entities));
			this.OnPropertyChanged(nameof(this.SelectedEntities));
			this.OnPropertyChanged(nameof(this.EditedEntities));

			this.OnPropertyChanged(nameof(this.SelectedEntity));
			this.OnPropertyChanged(nameof(this.EditedEntity));

			this.OnPropertyChanged(nameof(this.TotalCount));
			this.OnPropertyChanged(nameof(this.PageCount));

			this.OnPropertyChanged(nameof(this.Sequence));

			this.OnEntitiesChanged(new List<EntityViewObject>(), this.Entities);
			this.OnSelectedEntitiesChanged(new List<EntityViewObject>(), this.SelectedEntities);
			this.OnEditedEntitiesChanged(new List<EntityViewObject>(), this.EditedEntities);

			this.OnEntitiesAcquired();
		}

		private void EditedEntitiesChanged (object sender, NotifyCollectionChangedEventArgs e)
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
						viewObj.Validate();
						if (viewObj.IsValid)
						{
							viewObj.EndEdit();
						}
						else
						{
							viewObj.CancelEdit();
						}
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

		private void EntitiesChanged (object sender, NotifyCollectionChangedEventArgs e)
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

		private void EntityAdd (EntityViewObject viewObj)
		{
			if (viewObj.IsAdded)
			{
				return;
			}
			viewObj.IsAdded = true;

			viewObj.View = this;
			viewObj.Entity = viewObj.Entity ?? this.CreateEntity();

			this.OnEntityAdding(viewObj);
			viewObj.IsNew = true;
			this.Set.Add(viewObj.Entity);
			if (!this.Entities.Contains(viewObj))
			{
				this.Entities.Add(viewObj);
			}
			this.OnEntityAdded(viewObj);

			if (this.ValidateOnAdd)
			{
				viewObj.Validate();
			}

			if (this.SelectOnAdd)
			{
				viewObj.Select();
			}

			if (this.EditOnAdd)
			{
				viewObj.BeginEdit();
			}
		}

		private void EntityDelete (EntityViewObject viewObj)
		{
			if (!viewObj.IsAdded)
			{
				return;
			}
			viewObj.IsAdded = false;

			viewObj.EndEdit();
			viewObj.Deselect();

			this.OnEntityDeleting(viewObj);
			viewObj.IsDeleted = true;
			this.Entities.Remove(viewObj);
			this.Set.Delete(viewObj.Entity);
			this.OnEntityDeleted(viewObj);
		}

		private void EntityDeselect (EntityViewObject viewObj)
		{
			if (!viewObj.IsSelected)
			{
				return;
			}
			viewObj.IsSelected = false;

			this.OnEntityDeselecting(viewObj);

			this.SelectedEntities.Remove(viewObj);

			this.OnEntityDeselected(viewObj);
		}

		private void EntityEditBegin (EntityViewObject viewObj)
		{
			if (viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = true;

			this.OnEntityEditBeginning(viewObj);

			if (!this.EditedEntities.Contains(viewObj))
			{
				this.EditedEntities.Add(viewObj);
			}

			viewObj.IsModified = false;
			viewObj.Errors = null;

			this.OnEntityEditBegun(viewObj);
		}

		private void EntityEditCancel (EntityViewObject viewObj)
		{
			if (!viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = false;

			this.OnEntityEditCanceling(viewObj);

			if ((!viewObj.IsNew) && (!viewObj.IsDeleted))
			{
				this.Set.Reload(viewObj.Entity);
			}

			this.EditedEntities.Remove(viewObj);

			viewObj.Validate();

			this.OnEntityEditCanceled(viewObj);

			if (this.DeleteNewOnCancelEdit && viewObj.IsNew && this.Set.CanDelete(viewObj.Entity))
			{
				viewObj.Delete();
			}
		}

		private void EntityEditEnd (EntityViewObject viewObj)
		{
			if (!viewObj.IsEditing)
			{
				return;
			}
			viewObj.IsEditing = false;

			this.OnEntityEditEnding(viewObj);

			if (this.ExplicitModifyOnEndEdit && (!viewObj.IsNew) && (!viewObj.IsDeleted))
			{
				this.Set.Modify(viewObj.Entity);
			}

			this.EditedEntities.Remove(viewObj);

			viewObj.Validate();

			this.OnEntityEditEnded(viewObj);
		}

		private void EntitySelect (EntityViewObject viewObj)
		{
			if (viewObj.IsSelected)
			{
				return;
			}
			viewObj.IsSelected = true;

			this.OnEntitySelecting(viewObj);

			if (!this.SelectedEntities.Contains(viewObj))
			{
				this.SelectedEntities.Add(viewObj);
			}

			this.OnEntitySelected(viewObj);
		}

		private void EntityValidate (EntityViewObject viewObj)
		{
			this.OnEntityValidating(viewObj);

			viewObj.IsModified = this.Set.IsModified(viewObj.Entity);
			viewObj.Errors = this.Set.Validate(viewObj.Entity);

			this.OnEntityValidated(viewObj);
		}

		private void SelectedEntitiesChanged (object sender, NotifyCollectionChangedEventArgs e)
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

		#endregion




		#region Virtuals

		protected virtual TEntity CreateEntity ()
		{
			return this.Set.Create();
		}

		protected virtual void OnEditedEntitiesChanged (IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		protected virtual void OnEntitiesAcquired ()
		{
		}

		protected virtual void OnEntitiesAcquiring ()
		{
		}

		protected virtual void OnEntitiesChanged (IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		protected virtual void OnEntitiesReleased ()
		{
		}

		protected virtual void OnEntitiesReleasing ()
		{
		}

		protected virtual void OnEntityAdded (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityAdding (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeleted (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeleting (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeselected (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityDeselecting (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditBeginning (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditBegun (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditCanceled (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditCanceling (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditEnded (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityEditEnding (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntitySelected (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntitySelecting (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityValidated (EntityViewObject viewObj)
		{
		}

		protected virtual void OnEntityValidating (EntityViewObject viewObj)
		{
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

		protected virtual void OnSelectedEntitiesChanged (IList<EntityViewObject> oldItems, IList<EntityViewObject> newItems)
		{
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.ReleaseEntities();
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




		#region Type: EntityViewObject

		//TODO: Handle notify property changed of entity
		public sealed class EntityViewObject : INotifyPropertyChanged, INotifyPropertyChanging, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
		{
			#region Instance Constructor/Destructor

			public EntityViewObject ()
			{
				this.View = null;
				this.Entity = null;

				this.IsNew = false;
				this.IsDeleted = false;
				this.IsAdded = false;
				this.IsSelected = false;
				this.IsEditing = false;
				this.IsModified = false;
				this.Errors = null;
			}

			#endregion




			#region Instance Fields

			private RepositorySetErrors _errors;

			private bool _isAdded;

			private bool _isDeleted;

			private bool _isEditing;

			private bool _isModified;

			private bool _isNew;

			private bool _isSelected;

			#endregion




			#region Instance Properties/Indexer

			public TEntity Entity { get; internal set; }

			public RepositorySetErrors Errors
			{
				get
				{
					return this._errors;
				}
				internal set
				{
					this.OnErrorsChanging();
					this._errors = value;
					this.OnErrorsChanged();
				}
			}

			public string ErrorsString
			{
				get
				{
					if (this.Errors == null)
					{
						return null;
					}

					StringBuilder sb = new StringBuilder();

					foreach (string error in this.Errors.EntityErrors)
					{
						sb.AppendLine(error);
					}

					foreach (KeyValuePair<string, HashSet<string>> propertyError in this.Errors.PropertyErrors)
					{
						foreach (string error in propertyError.Value)
						{
							sb.AppendLine(error);
						}
					}

					return sb.ToString().ToNullIfNullOrEmpty()?.Trim();
				}
			}

			public bool IsAdded
			{
				get
				{
					return this._isAdded;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsAdded));
					this._isAdded = value;
					this.OnPropertyChanged(nameof(this.IsAdded));
				}
			}

			public bool IsDeleted
			{
				get
				{
					return this._isDeleted;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsDeleted));
					this._isDeleted = value;
					this.OnPropertyChanged(nameof(this.IsDeleted));
				}
			}

			public bool IsEditing
			{
				get
				{
					return this._isEditing;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsEditing));
					this.OnPropertyChanging(nameof(IChangeTracking.IsChanged));
					this._isEditing = value;
					this.OnPropertyChanged(nameof(this.IsEditing));
					this.OnPropertyChanged(nameof(IChangeTracking.IsChanged));
				}
			}

			public bool IsModified
			{
				get
				{
					return this._isModified;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsModified));
					this._isModified = value;
					this.OnPropertyChanged(nameof(this.IsModified));
				}
			}

			public bool IsNew
			{
				get
				{
					return this._isNew;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsNew));
					this._isNew = value;
					this.OnPropertyChanged(nameof(this.IsNew));
				}
			}

			public bool IsSelected
			{
				get
				{
					return this._isSelected;
				}
				internal set
				{
					this.OnPropertyChanging(nameof(this.IsSelected));
					this._isSelected = value;
					this.OnPropertyChanged(nameof(this.IsSelected));
				}
			}

			public bool IsValid => this.Errors == null;

			public EntityView<TEntity> View { get; internal set; }

			#endregion




			#region Instance Methods

			public void Delete ()
			{
				this.View.EntityDelete(this);
			}

			public void Deselect ()
			{
				this.View.EntityDeselect(this);
			}

			public void Select ()
			{
				this.View.EntitySelect(this);
			}

			public void Validate ()
			{
				this.View.EntityValidate(this);
			}

			private void OnErrorsChanged ()
			{
				this.OnPropertyChanged(nameof(this.Errors));
				this.OnPropertyChanged(nameof(this.ErrorsString));
				this.OnPropertyChanged(nameof(this.IsValid));
				this.OnPropertyChanged(nameof(string.Empty));
				this.OnPropertyChanged(nameof(IDataErrorInfo.Error));
				this.OnPropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));

				this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
			}

			private void OnErrorsChanging ()
			{
				this.OnPropertyChanging(nameof(this.Errors));
				this.OnPropertyChanging(nameof(this.ErrorsString));
				this.OnPropertyChanging(nameof(this.IsValid));
				this.OnPropertyChanging(nameof(string.Empty));
				this.OnPropertyChanging(nameof(IDataErrorInfo.Error));
				this.OnPropertyChanging(nameof(INotifyDataErrorInfo.HasErrors));
			}

			/// <summary>
			///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
			/// </summary>
			/// <param name="propertyName"> The name of the property which has changed. </param>
			private void OnPropertyChanged (string propertyName)
			{
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}


			/// <summary>
			///     Handles the change of a property value by raising the <see cref="PropertyChanging" /> event.
			/// </summary>
			/// <param name="propertyName"> The name of the property which is about to be changed. </param>
			private void OnPropertyChanging (string propertyName)
			{
				this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
			}

			#endregion




			#region Interface: IChangeTracking

			bool IChangeTracking.IsChanged => this.IsEditing;

			void IChangeTracking.AcceptChanges ()
			{
				this.EndEdit();
			}

			#endregion




			#region Interface: IDataErrorInfo

			string IDataErrorInfo.Error => this.Errors?.EntityErrors?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();

			string IDataErrorInfo.this [string columnName]
			{
				get
				{
					if (this.Errors == null)
					{
						return null;
					}

					if (columnName.IsNullOrEmpty())
					{
						return ((IDataErrorInfo)this).Error;
					}

					if (!this.Errors.PropertyErrors.ContainsKey(columnName))
					{
						return null;
					}

					return this.Errors?.PropertyErrors?[columnName]?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();
				}
			}

			#endregion




			#region Interface: IEditableObject

			public void BeginEdit ()
			{
				this.View.EntityEditBegin(this);
			}

			public void CancelEdit ()
			{
				this.View.EntityEditCancel(this);
			}

			public void EndEdit ()
			{
				this.View.EntityEditEnd(this);
			}

			#endregion




			#region Interface: INotifyDataErrorInfo

			bool INotifyDataErrorInfo.HasErrors => this.Errors != null;

			public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

			IEnumerable INotifyDataErrorInfo.GetErrors (string propertyName)
			{
				if (this.Errors == null)
				{
					return new string[0];
				}

				if (propertyName.IsNullOrEmpty())
				{
					return this.Errors?.EntityErrors?.ToArray() ?? new string[0];
				}

				if (!this.Errors.PropertyErrors.ContainsKey(propertyName))
				{
					return new string[0];
				}

				return this.Errors?.PropertyErrors?[propertyName]?.ToArray() ?? new string[0];
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




			#region Interface: IRevertibleChangeTracking

			void IRevertibleChangeTracking.RejectChanges ()
			{
				this.CancelEdit();
			}

			#endregion
		}

		#endregion
	}
}
