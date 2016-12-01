using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityViewObject<TEntity> : INotifyPropertyChanged, INotifyPropertyChanging, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
		where TEntity : class
	{
		public EntityViewObject ()
		{
			this.ViewCaller = null;

			this.IsAdded = false;
			this.IsDeleted = false;
			this.IsEdited = false;
			this.IsModified = false;

			this.Entity = null;
			this.Errors = null;
		}






		private bool _isAdded;

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

		private bool _isDeleted;

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

		private bool _isEdited;

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

		private bool _isModified;
		
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

		private TEntity _entity;

		public TEntity Entity
		{
			get
			{
				return this._entity;
			}
			internal set
			{
				//TODO: Handle INPC of Entity
				this.OnPropertyChanging(nameof(this.Entity));
				this._entity = value;
				this.OnPropertyChanged(nameof(this.Entity));
			}
		}

		private RepositorySetErrors _errors;

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

		public bool IsValid => this.Errors == null;

		internal IEntityViewCaller<TEntity> ViewCaller { get; set; }

		private void OnErrorsChanged()
		{
			this.OnPropertyChanged(nameof(this.Errors));
			this.OnPropertyChanged(nameof(this.ErrorsString));
			this.OnPropertyChanged(nameof(this.IsValid));

			this.OnPropertyChanged(nameof(string.Empty));
			this.OnPropertyChanged(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));

			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
		}

		private void OnErrorsChanging()
		{
			this.OnPropertyChanging(nameof(this.Errors));
			this.OnPropertyChanging(nameof(this.ErrorsString));
			this.OnPropertyChanging(nameof(this.IsValid));

			this.OnPropertyChanging(nameof(string.Empty));
			this.OnPropertyChanging(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanging(nameof(INotifyDataErrorInfo.HasErrors));
		}

		public void BeginEdit()
		{
			this.ViewCaller.BeginEdit(this.Entity);
		}

		public void CancelEdit()
		{
			this.ViewCaller.CancelEdit(this.Entity);
		}

		public void EndEdit()
		{
			this.ViewCaller.EndEdit(this.Entity);
		}

		public void Delete()
		{
			this.ViewCaller.Delete(this.Entity);
		}

		public void Reload()
		{
			this.ViewCaller.Reload(this.Entity);
		}

		public void Modify()
		{
			this.ViewCaller.Modify(this.Entity);
		}

		public void Validate()
		{
			this.ViewCaller.Validate(this.Entity);
		}

		public bool CanEdit()
		{
			return this.ViewCaller.CanEdit(this.Entity);
		}

		public bool CanDelete()
		{
			return this.ViewCaller.CanDelete(this.Entity);
		}

		public bool CanReload()
		{
			return this.ViewCaller.CanReload(this.Entity);
		}

		public bool CanModify()
		{
			return this.ViewCaller.CanModify(this.Entity);
		}

		public bool CanValidate()
		{
			return this.ViewCaller.CanValidate(this.Entity);
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




		#region Interface: IChangeTracking

		/// <inheritdoc />
		bool IChangeTracking.IsChanged => this.IsEdited;

		/// <inheritdoc />
		void IChangeTracking.AcceptChanges()
		{
			this.EndEdit();
		}

		/// <inheritdoc />
		void IRevertibleChangeTracking.RejectChanges()
		{
			this.CancelEdit();
		}

		#endregion




		#region Interface: IDataErrorInfo

		/// <inheritdoc />
		string IDataErrorInfo.Error => this.Errors?.EntityErrors?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();

		/// <inheritdoc />
		string IDataErrorInfo.this[string columnName]
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




		#region Interface: INotifyDataErrorInfo

		/// <inheritdoc />
		bool INotifyDataErrorInfo.HasErrors => this.Errors != null;

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <inheritdoc />
		IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
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
	}
}
