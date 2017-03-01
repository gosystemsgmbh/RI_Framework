﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityViewObject <TEntity> : INotifyPropertyChanged, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
		where TEntity : class
	{
		#region Instance Constructor/Destructor

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

		public string ErrorLines => this.Errors?.ToErrorString(Environment.NewLine);

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

		public string ErrorStrings => this.Errors?.ToErrorString(" ");

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

		public bool IsAddedOrAttached => this.IsAdded || this.IsAttached;

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

		public bool IsValid => this.Errors == null;

		internal IEntityViewCaller<TEntity> ViewCaller { get; set; }

		#endregion




		#region Instance Methods

		public bool CanDelete ()
		{
			return this.ViewCaller.CanDelete(this.Entity);
		}

		public bool CanEdit ()
		{
			return this.ViewCaller.CanEdit(this.Entity);
		}

		public bool CanModify ()
		{
			return this.ViewCaller.CanModify(this.Entity);
		}

		public bool CanReload ()
		{
			return this.ViewCaller.CanReload(this.Entity);
		}

		public bool CanSelect ()
		{
			return this.ViewCaller.CanSelect(this.Entity);
		}

		public bool CanValidate ()
		{
			return this.ViewCaller.CanValidate(this.Entity);
		}

		public void Delete ()
		{
			this.ViewCaller.Delete(this.Entity);
		}

		public void Deselect ()
		{
			this.ViewCaller.Deselect(this.Entity);
		}

		public void Modify ()
		{
			this.ViewCaller.Modify(this.Entity);
		}

		public void Reload ()
		{
			this.ViewCaller.Reload(this.Entity);
		}

		public void ResetIsAddedOrAttached ()
		{
			this.IsAdded = false;
			this.IsAttached = false;

			this.RaiseEntityChanged();
		}

		public void Select ()
		{
			this.ViewCaller.Select(this.Entity);
		}

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
		string IDataErrorInfo.Error => this.Errors?.EntityErrors?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();

		/// <inheritdoc />
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

				return this.Errors.PropertyErrors[columnName].Join(Environment.NewLine).ToNullIfNullOrEmpty()?.Trim();
			}
		}

		#endregion




		#region Interface: IEditableObject

		public void BeginEdit ()
		{
			this.ViewCaller.BeginEdit(this.Entity);
		}

		public void CancelEdit ()
		{
			this.ViewCaller.CancelEdit(this.Entity);
		}

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

			if (propertyName.IsNullOrEmpty())
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