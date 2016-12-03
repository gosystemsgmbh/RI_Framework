using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityViewObject <TEntity> : INotifyPropertyChanged, INotifyPropertyChanging, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
		where TEntity : class
	{
		#region Instance Constructor/Destructor

		public EntityViewObject ()
		{
			this.ViewCaller = null;

			this.IsAdded = false;
			this.IsAttached = false;
			this.IsDeleted = false;
			this.IsEdited = false;
			this.IsModified = false;

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

		#endregion




		#region Instance Properties/Indexer

		public TEntity Entity
		{
			get
			{
				return this._entity;
			}
			internal set
			{
				this.OnPropertyChanging(nameof(this.Entity));
				this._entity = value;
				this.OnPropertyChanged(nameof(this.Entity));
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
				this.OnErrorsChanging();
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
				this.OnPropertyChanging(nameof(this.IsAdded));
				this.OnPropertyChanging(nameof(this.IsAddedOrAttached));
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
				this.OnPropertyChanging(nameof(this.IsAttached));
				this.OnPropertyChanging(nameof(this.IsAddedOrAttached));
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
				this.OnPropertyChanging(nameof(this.IsDeleted));
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
				this.OnPropertyChanging(nameof(this.IsEdited));
				this._isEdited = value;
				this.OnPropertyChanged(nameof(this.IsEdited));
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

		public bool CanValidate ()
		{
			return this.ViewCaller.CanValidate(this.Entity);
		}

		public void Delete ()
		{
			this.ViewCaller.Delete(this.Entity);
		}

		public void Modify ()
		{
			this.ViewCaller.Modify(this.Entity);
		}

		public void Reload ()
		{
			this.ViewCaller.Reload(this.Entity);
		}

		public void Validate ()
		{
			this.ViewCaller.Validate(this.Entity);
		}

		internal void RaiseEntityChanged ()
		{
			this.OnPropertyChanged(nameof(this.Entity));
		}

		private void OnErrorsChanged ()
		{
			this.OnPropertyChanged(nameof(this.Errors));
			this.OnPropertyChanged(nameof(this.ErrorLines));
			this.OnPropertyChanged(nameof(this.ErrorStrings));
			this.OnPropertyChanged(nameof(this.IsValid));

			this.OnPropertyChanged(nameof(string.Empty));
			this.OnPropertyChanged(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));

			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
		}

		private void OnErrorsChanging ()
		{
			this.OnPropertyChanging(nameof(this.Errors));
			this.OnPropertyChanging(nameof(this.ErrorLines));
			this.OnPropertyChanging(nameof(this.ErrorStrings));
			this.OnPropertyChanging(nameof(this.IsValid));

			this.OnPropertyChanging(nameof(string.Empty));
			this.OnPropertyChanging(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanging(nameof(INotifyDataErrorInfo.HasErrors));
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
		///     Handles the change of a property value by raising the <see cref="PropertyChanging" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which is about to be changed. </param>
		protected virtual void OnPropertyChanging (string propertyName)
		{
			this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
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




		#region Interface: INotifyPropertyChanging

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

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
