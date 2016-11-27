using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Views
{
	public class EntityViewObject : INotifyPropertyChanged, INotifyPropertyChanging, IEditableObject, IDataErrorInfo, INotifyDataErrorInfo, IChangeTracking, IRevertibleChangeTracking
	{
		#region Virtuals

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

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

		#region Instance Constructor/Destructor

		public EntityViewObject()
		{
			this.ViewModel = null;
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

		private TEntity _entity;

		private RepositorySetErrors _errors;

		private bool _isModified;

		private bool _isDeleted;

		private bool _isEditing;

		private bool _isNew;

		private bool _isSelected;

		private bool _isAdded;

		private EntityViewModel<TModule, TEntity> _viewModel;

		#endregion




		#region Instance Properties/Indexer

		public TEntity Entity
		{
			get
			{
				return this._entity;
			}
			set
			{
				this._entity = value;
				this.OnEntityOrErrorsChanged();
			}
		}

		public RepositorySetErrors Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				this._errors = value;
				this.OnEntityOrErrorsChanged();
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

		public bool IsModified
		{
			get
			{
				return this._isModified;
			}
			set
			{
				this._isModified = value;
				this.OnPropertyChanged(nameof(this.IsModified));
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this._isDeleted;
			}
			set
			{
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
			set
			{
				this._isEditing = value;
				this.OnPropertyChanged(nameof(this.IsEditing));
			}
		}

		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
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
			set
			{
				this._isSelected = value;
				this.OnPropertyChanged(nameof(this.IsSelected));
			}
		}

		public bool IsAdded
		{
			get
			{
				return this._isAdded;
			}
			set
			{
				this._isAdded = value;
				this.OnPropertyChanged(nameof(this.IsAdded));
			}
		}

		public bool IsValid => this.Errors == null;

		public EntityViewModel<TModule, TEntity> ViewModel
		{
			get
			{
				return this._viewModel;
			}
			set
			{
				this._viewModel = value;
				this.OnPropertyChanged(nameof(this.ViewModel));
			}
		}

		#endregion




		#region Instance Methods

		public void Deselect()
		{
			this.ViewModel.EntityDeselect(this);
		}

		public void Delete()
		{
			this.ViewModel.EntityDelete(this);
		}

		public void Select()
		{
			this.ViewModel.EntitySelect(this);
		}

		public void Validate()
		{
			this.ViewModel.EntityValidate(this);
		}

		private void OnEntityOrErrorsChanged()
		{
			this.OnPropertyChanged(nameof(this.Entity));
			this.OnPropertyChanged(nameof(this.Errors));
			this.OnPropertyChanged(nameof(this.ErrorsString));
			this.OnPropertyChanged(nameof(this.IsValid));
			this.OnPropertyChanged(nameof(string.Empty));
			this.OnPropertyChanged(nameof(IDataErrorInfo.Error));
			this.OnPropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));

			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
		}

		#endregion




		#region Interface: IChangeTracking

		bool IChangeTracking.IsChanged => this.IsEditing;

		void IChangeTracking.AcceptChanges()
		{
			this.EndEdit();
		}

		#endregion




		#region Interface: IDataErrorInfo

		string IDataErrorInfo.Error
		{
			get
			{
				if (this.Errors == null)
				{
					return null;
				}

				return this.Errors?.EntityErrors?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();
			}
		}

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

				return this.Errors?.PropertyErrors?[columnName]?.Join(Environment.NewLine)?.ToNullIfNullOrEmpty()?.Trim();
			}
		}

		#endregion




		#region Interface: IEditableObject

		public void BeginEdit()
		{
			this.ViewModel.EntityEditBegin(this);
		}

		public void CancelEdit()
		{
			this.ViewModel.EntityEditCancel(this);
		}

		public void EndEdit()
		{
			this.ViewModel.EntityEditEnd(this);
		}

		#endregion




		#region Interface: INotifyDataErrorInfo

		bool INotifyDataErrorInfo.HasErrors => this.Errors != null;

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
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




		#region Interface: IRevertibleChangeTracking

		void IRevertibleChangeTracking.RejectChanges()
		{
			this.CancelEdit();
		}

		#endregion

		#endregion
	}
}