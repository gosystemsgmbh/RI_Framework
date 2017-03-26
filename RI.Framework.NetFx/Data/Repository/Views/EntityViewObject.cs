using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	/// <summary>
	/// Used by <see cref="EntityView{TEntity,TViewObject}"/> to wrap entities.
	/// </summary>
	/// <remarks>
	/// See <see cref="EntityView{TEntity,TViewObject}"/> for details.
	/// </remarks>
	/// <typeparam name="TEntity">The type of entities wrapped by <see cref="EntityView{TEntity,TViewObject}"/>.</typeparam>
	public class EntityViewObject <TEntity> : INotifyPropertyChanged, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
		where TEntity : class
	{
		#region Instance Constructor/Destructor

		/// <summary>
		/// Creates a new instance of <see cref="EntityViewObject{TEntity}"/>.
		/// </summary>
		public EntityViewObject ()
		{
			this.EntityChangedHandler = this.EntityChangedMethod;

			this.ViewCaller = null;

			this.IsAdded = false;
			this.IsAttached = false;
			this.IsDeleted = false;
			this.IsEdited = false;
			this.IsModified = false;
			this.IsSelected = false;

			this.Entity = null;
			this.Errors = null;
		}

		#endregion




		#region Instance Fields

		private TEntity _entity;

		private RepositorySetErrors _errors;

		private bool _isAdded;

		private bool _isAttached;

		private bool _isDeleted;

		private bool _isEdited;

		private bool _isModified;

		private bool _isSelected;

		#endregion




		#region Instance Properties/Indexer

		private PropertyChangedEventHandler EntityChangedHandler { get; set; }

		private void EntityChangedMethod (object sender, PropertyChangedEventArgs e)
		{
			this.RaiseEntityChanged();
		}

		/// <summary>
		/// Gets the wrapped entity.
		/// </summary>
		/// <value>
		/// The wrapped entity.
		/// </value>
		public TEntity Entity
		{
			get
			{
				return this._entity;
			}
			internal set
			{
				if (this._entity is INotifyPropertyChanged)
				{
					((INotifyPropertyChanged)this._entity).PropertyChanged -= this.EntityChangedHandler;
				}

				this._entity = value;

				if (this._entity is INotifyPropertyChanged)
				{
					((INotifyPropertyChanged)this._entity).PropertyChanged += this.EntityChangedHandler;
				}

				this.RaiseEntityChanged();
			}
		}

		/// <summary>
		/// Gets all validation errors of the wrapped entity as a string, separated by line feeds.
		/// </summary>
		/// <value>
		/// All validation errors of the wrapped entity as a string, separated by line feeds.
		/// </value>
		public string ErrorLines => this.Errors?.ToErrorString(Environment.NewLine);

		/// <summary>
		/// Gets all validation errors of the wrapped entity.
		/// </summary>
		/// <value>
		/// All validation errors of the wrapped entity.
		/// </value>
		public RepositorySetErrors Errors
		{
			get
			{
				return this._errors;
			}
			internal set
			{
				this._errors = value;
				this.OnErrorsChanged();
			}
		}

		/// <summary>
		/// Gets all validation errors of the wrapped entity as a string, separated by space characters.
		/// </summary>
		/// <value>
		/// All validation errors of the wrapped entity as a string, separated by space characters.
		/// </value>
		public string ErrorStrings => this.Errors?.ToErrorString(" ");

		/// <summary>
		/// Gets whether the wrapped entity was added.
		/// </summary>
		/// <value>
		/// true if the wrapped entity was added, false otherwise.
		/// </value>
		public bool IsAdded
		{
			get
			{
				return this._isAdded;
			}
			internal set
			{
				this._isAdded = value;
				this.OnPropertyChanged(nameof(this.IsAdded));
				this.OnPropertyChanged(nameof(this.IsAddedOrAttached));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity was added or attached.
		/// </summary>
		/// <value>
		/// true if the wrapped entity was added or attached, false otherwise.
		/// </value>
		public bool IsAddedOrAttached => this.IsAdded || this.IsAttached;

		/// <summary>
		/// Gets whether the wrapped entity was attached.
		/// </summary>
		/// <value>
		/// true if the wrapped entity was attached, false otherwise.
		/// </value>
		public bool IsAttached
		{
			get
			{
				return this._isAttached;
			}
			internal set
			{
				this._isAttached = value;
				this.OnPropertyChanged(nameof(this.IsAttached));
				this.OnPropertyChanged(nameof(this.IsAddedOrAttached));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity was deleted.
		/// </summary>
		/// <value>
		/// true if the wrapped entity was deleted, false otherwise.
		/// </value>
		public bool IsDeleted
		{
			get
			{
				return this._isDeleted;
			}
			internal set
			{
				this._isDeleted = value;
				this.OnPropertyChanged(nameof(this.IsDeleted));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity is currently being edited.
		/// </summary>
		/// <value>
		/// true if the wrapped entity is currently being edited, false otherwise.
		/// </value>
		public bool IsEdited
		{
			get
			{
				return this._isEdited;
			}
			internal set
			{
				this._isEdited = value;
				this.OnPropertyChanged(nameof(this.IsEdited));
				this.OnPropertyChanged(nameof(IChangeTracking.IsChanged));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity was modified.
		/// </summary>
		/// <value>
		/// true if the wrapped entity was modified, false otherwise.
		/// </value>
		public bool IsModified
		{
			get
			{
				return this._isModified;
			}
			internal set
			{
				this._isModified = value;
				this.OnPropertyChanged(nameof(this.IsModified));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity is currently selected.
		/// </summary>
		/// <value>
		/// true if the wrapped entity is currently selected, false otherwise.
		/// </value>
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			internal set
			{
				this._isSelected = value;
				this.OnPropertyChanged(nameof(this.IsSelected));
			}
		}

		/// <summary>
		/// Gets whether the wrapped entity is valid.
		/// </summary>
		/// <value>
		/// true if the wrapped entity is valid, false otherwise.
		/// </value>
		public bool IsValid => this.Errors == null;

		internal IEntityViewCaller<TEntity> ViewCaller { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be deleted.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be deleted, false otherwise.
		/// </returns>
		public bool CanDelete ()
		{
			return this.ViewCaller.CanDelete(this.Entity);
		}

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be edited.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be edited, false otherwise.
		/// </returns>
		public bool CanEdit ()
		{
			return this.ViewCaller.CanEdit(this.Entity);
		}

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be modified.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be modified, false otherwise.
		/// </returns>
		public bool CanModify ()
		{
			return this.ViewCaller.CanModify(this.Entity);
		}

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be reloaded.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be reloaded, false otherwise.
		/// </returns>
		public bool CanReload ()
		{
			return this.ViewCaller.CanReload(this.Entity);
		}

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be selected.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be selected, false otherwise.
		/// </returns>
		public bool CanSelect ()
		{
			return this.ViewCaller.CanSelect(this.Entity);
		}

		/// <summary>
		/// Determines whether the entity wrapped by this view object can be validated.
		/// </summary>
		/// <returns>
		/// true if the entity wrapped by this view object can be validated, false otherwise.
		/// </returns>
		public bool CanValidate ()
		{
			return this.ViewCaller.CanValidate(this.Entity);
		}

		/// <summary>
		/// Deletes the entity wrapped by this view object.
		/// </summary>
		public void Delete ()
		{
			this.ViewCaller.Delete(this.Entity);
		}

		/// <summary>
		/// Deselects the entity wrapped by this view object.
		/// </summary>
		public void Deselect ()
		{
			this.ViewCaller.Deselect(this.Entity);
		}

		/// <summary>
		/// Modifies the entity wrapped by this view object.
		/// </summary>
		public void Modify ()
		{
			this.ViewCaller.Modify(this.Entity);
		}

		/// <summary>
		/// Reloads the entity wrapped by this view object.
		/// </summary>
		public void Reload ()
		{
			this.ViewCaller.Reload(this.Entity);
		}

		/// <summary>
		/// Resets <see cref="IsAdded"/>, <see cref="IsAttached"/>, and <see cref="IsAddedOrAttached"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method might be usefull in cases when an entity is no longer considered &quot;new&quot; after it was added or attached.
		/// </para>
		/// </remarks>
		public void ResetIsAddedOrAttached ()
		{
			this.IsAdded = false;
			this.IsAttached = false;

			this.RaiseEntityChanged();
		}

		/// <summary>
		/// Selects the entity wrapped by this view object.
		/// </summary>
		public void Select ()
		{
			this.ViewCaller.Select(this.Entity);
		}

		/// <summary>
		/// Validates the entity wrapped by this view object.
		/// </summary>
		public void Validate ()
		{
			this.ViewCaller.Validate(this.Entity);
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Raises <see cref="ErrorsChanged"/> and <see cref="PropertyChanged"/> for all error/validation relevant properties.
		/// </summary>
		protected virtual void OnErrorsChanged ()
		{
			this.OnPropertyChanged(nameof(this.Errors));
			this.OnPropertyChanged(nameof(this.ErrorLines));
			this.OnPropertyChanged(nameof(this.ErrorStrings));
			this.OnPropertyChanged(nameof(this.IsValid));

			this.OnPropertyChanged(null);
			this.OnPropertyChanged(string.Empty);
			this.OnPropertyChanged(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));

			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
		}

		/// <summary>
		/// Raises <see cref="PropertyChanged"/> for all wrapped entity relevant properties.
		/// </summary>
		protected internal virtual void RaiseEntityChanged ()
		{
			this.OnPropertyChanged(nameof(this.Entity));
		}

		#endregion




		#region Interface: IChangeTracking

		/// <inheritdoc />
		bool IChangeTracking.IsChanged => this.IsEdited;

		/// <inheritdoc />
		void IChangeTracking.AcceptChanges ()
		{
			this.EndEdit();
		}

		#endregion




		#region Interface: IDataErrorInfo

		/// <inheritdoc />
		string IDataErrorInfo.Error => this.Errors?.EntityErrors?.Join(Environment.NewLine)?.ToNullIfNullOrEmptyOrWhitespace()?.Trim();

		/// <inheritdoc />
		string IDataErrorInfo.this [string columnName]
		{
			get
			{
				if (this.Errors == null)
				{
					return null;
				}

				if (columnName.IsNullOrEmptyOrWhitespace())
				{
					return ((IDataErrorInfo)this).Error;
				}

				if (!this.Errors.PropertyErrors.ContainsKey(columnName))
				{
					return null;
				}

				return this.Errors.PropertyErrors[columnName].Join(Environment.NewLine).ToNullIfNullOrEmptyOrWhitespace()?.Trim();
			}
		}

		#endregion




		#region Interface: IEditableObject

		/// <inheritdoc />
		public void BeginEdit ()
		{
			this.ViewCaller.BeginEdit(this.Entity);
		}

		/// <inheritdoc />
		public void CancelEdit ()
		{
			this.ViewCaller.CancelEdit(this.Entity);
		}

		/// <inheritdoc />
		public void EndEdit ()
		{
			this.ViewCaller.EndEdit(this.Entity);
		}

		#endregion




		#region Interface: INotifyDataErrorInfo

		/// <inheritdoc />
		bool INotifyDataErrorInfo.HasErrors => this.Errors != null;

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <inheritdoc />
		IEnumerable INotifyDataErrorInfo.GetErrors (string propertyName)
		{
			if (this.Errors == null)
			{
				return new string[0];
			}

			if (propertyName.IsNullOrEmptyOrWhitespace())
			{
				return this.Errors.EntityErrors.ToArray();
			}

			if (!this.Errors.PropertyErrors.ContainsKey(propertyName))
			{
				return new string[0];
			}

			return this.Errors.PropertyErrors[propertyName].ToArray();
		}

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion




		#region Interface: IRevertibleChangeTracking

		/// <inheritdoc />
		void IRevertibleChangeTracking.RejectChanges ()
		{
			this.CancelEdit();
		}

		#endregion
	}
}
