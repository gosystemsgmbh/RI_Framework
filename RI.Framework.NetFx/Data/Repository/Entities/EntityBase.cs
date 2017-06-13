using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;

using RI.Framework.Data.Repository.Views;
using RI.Framework.Utilities;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	///     Implements a base class for entities.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="EntityBase" /> implements basic functionality which might be desirable by entities which are used together with <see cref="IRepositoryContext" /> and/or <see cref="EntityView{TEntity}" />, such as property change notification, change tracking, and error tracking.
	///     </para>
	/// </remarks>
	[Serializable]
	public abstract class EntityBase : INotifyPropertyChanged, IEntityChangeTracking, IEntityErrorTracking, IDataErrorInfo, INotifyDataErrorInfo
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityBase" />.
		/// </summary>
		protected EntityBase ()
		{
			this.CreateTimestamp = null;
			this.CreateContext = null;

			this.ModifyTimestamp = null;
			this.ModifyContext = null;

			this.Errors = null;
		}

		#endregion




		#region Instance Fields

		private string _createContext;
		private DateTime? _createTimestamp;

		[field: NonSerialized]
		[field: XmlIgnore]
		private RepositorySetErrors _errors;

		private string _modifyContext;
		private DateTime? _modifyTimestamp;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the application-specific context for when this entity was created.
		/// </summary>
		/// <value>
		///     The application-specific context for when this entity was created.
		/// </value>
		public string CreateContext
		{
			get
			{
				return this._createContext;
			}
			set
			{
				string oldValue = this._createContext;
				this._createContext = value;
				bool notify = oldValue != value;
				this.OnPropertyChanged(notify, nameof(this.CreateContext));
			}
		}

		/// <summary>
		///     Gets or sets the timestamp this entity was created.
		/// </summary>
		/// <value>
		///     The timestamp this entity was created.
		/// </value>
		public DateTime? CreateTimestamp
		{
			get
			{
				return this._createTimestamp;
			}
			set
			{
				DateTime? oldValue = this._createTimestamp;
				this._createTimestamp = value;
				bool notify = oldValue != value;
				this.OnPropertyChanged(notify, nameof(this.CreateTimestamp));
			}
		}

		/// <summary>
		///     Gets or sets the errors associated with this entity.
		/// </summary>
		/// <value>
		///     The errors associated with this entity or null if no errors are associated with this entity.
		/// </value>
		public RepositorySetErrors Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				RepositorySetErrors oldValue = this._errors;
				this._errors = value;
				bool notify = !object.ReferenceEquals(oldValue, value);
				this.OnPropertyChanged(notify, nameof(this.Errors));
				this.OnPropertyChanged(notify, nameof(this.ErrorStringWithSpaces));
				this.OnPropertyChanged(notify, nameof(this.ErrorStringWithNewLines));
				this.OnErrorsChanged();
			}
		}

		/// <summary>
		///     Gets all errors of this entity as a string where each error is separated with a line feed.
		/// </summary>
		/// <value>
		///     All errors of this entity or null if this entity has no errors.
		/// </value>
		public string ErrorStringWithNewLines => this.Errors?.ToErrorString(Environment.NewLine).ToNullIfNullOrEmpty();

		/// <summary>
		///     Gets all errors of this entity as a string where each error is separated with a single space.
		/// </summary>
		/// <value>
		///     All errors of this entity or null if this entity has no errors.
		/// </value>
		public string ErrorStringWithSpaces => this.Errors?.ToErrorString(" ").ToNullIfNullOrEmpty();

		/// <summary>
		///     Gets or sets the application-specific context for when this entity was modified.
		/// </summary>
		/// <value>
		///     The application-specific context for when this entity was modified.
		/// </value>
		public string ModifyContext
		{
			get
			{
				return this._modifyContext;
			}
			set
			{
				string oldValue = this._modifyContext;
				this._modifyContext = value;
				bool notify = oldValue != value;
				this.OnPropertyChanged(notify, nameof(this.ModifyContext));
			}
		}

		/// <summary>
		///     Gets or sets the timestamp this entity was modified.
		/// </summary>
		/// <value>
		///     The timestamp this entity was modified.
		/// </value>
		public DateTime? ModifyTimestamp
		{
			get
			{
				return this._modifyTimestamp;
			}
			set
			{
				DateTime? oldValue = this._modifyTimestamp;
				this._modifyTimestamp = value;
				bool notify = oldValue != value;
				this.OnPropertyChanged(notify, nameof(this.ModifyTimestamp));
			}
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called when the errors associated with this entity have changed.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default implementation raises <see cref="ErrorsChanged" /> and calls <see cref="OnPropertyChanged" /> with the notify parameter set to true and the propertyName parameter set to null.
		///     </para>
		/// </remarks>
		protected virtual void OnErrorsChanged ()
		{
			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
			this.OnPropertyChanged(true, null);
		}

		/// <summary>
		///     Called when a property of the entity has changed.
		/// </summary>
		/// <param name="notify"> Indicates whether the property has its value changed and notifications should be raised. </param>
		/// <param name="propertyName"> The name of the property which has changed. Can be null or an empty string to indicate multiple properties or indexers have changed. </param>
		/// <remarks>
		///     <para>
		///         The default implementation raises <see cref="PropertyChanged" /> if <paramref name="notify" /> is true.
		///     </para>
		/// </remarks>
		protected virtual void OnPropertyChanged (bool notify, string propertyName)
		{
			if (notify)
			{
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		///     Called by <see cref="IEntityChangeTracking.SetCreation" /> and <see cref="IEntityChangeTracking.SetModification" /> to convert the application-specific change tracking context to a string.
		/// </summary>
		/// <param name="changeTrackingContext"> The application-specific change tracking context. Can be null. </param>
		/// <returns>
		///     The change tracking context serialized as a string or null if the context cannot be serialized as a string or <paramref name="changeTrackingContext" /> is null.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The default implementation calls <see cref="object.ToString" />.
		///     </para>
		/// </remarks>
		protected virtual string SerializeChangeTrackingContext (object changeTrackingContext)
		{
			return changeTrackingContext?.ToString();
		}

		#endregion




		#region Interface: IDataErrorInfo

		/// <inheritdoc />
		string IDataErrorInfo.Error
		{
			get
			{
				if (this.Errors == null)
				{
					return null;
				}

				if (this.Errors.EntityErrors.Count == 0)
				{
					return null;
				}

				return this.Errors.EntityErrors.Join(Environment.NewLine).ToNullIfNullOrEmpty();
			}
		}

		/// <inheritdoc />
		string IDataErrorInfo.this [string columnName]
		{
			get
			{
				if (this.Errors == null)
				{
					return null;
				}

				if (!this.Errors.PropertyErrors.ContainsKey(columnName))
				{
					return null;
				}

				return this.Errors.PropertyErrors[columnName].Join(Environment.NewLine).ToNullIfNullOrEmpty();
			}
		}

		#endregion




		#region Interface: IEntityChangeTracking

		/// <inheritdoc />
		void IEntityChangeTracking.SetCreation (object changeTrackingContext, DateTime timestamp)
		{
			if (this.CreateTimestamp == null)
			{
				this.CreateTimestamp = timestamp;
				this.CreateContext = this.SerializeChangeTrackingContext(changeTrackingContext);
			}
		}

		/// <inheritdoc />
		void IEntityChangeTracking.SetModification (object changeTrackingContext, DateTime timestamp)
		{
			this.ModifyTimestamp = timestamp;
			this.ModifyContext = this.SerializeChangeTrackingContext(changeTrackingContext);
		}

		#endregion




		#region Interface: IEntityErrorTracking

		/// <inheritdoc />
		void IEntityErrorTracking.SetErrors (RepositorySetErrors errors)
		{
			this.Errors = errors;
		}

		#endregion




		#region Interface: INotifyDataErrorInfo

		/// <inheritdoc />
		bool INotifyDataErrorInfo.HasErrors => this.Errors != null;

		/// <inheritdoc />
		[field: NonSerialized]
		[field: XmlIgnore]
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <inheritdoc />
		IEnumerable INotifyDataErrorInfo.GetErrors (string propertyName)
		{
			if (this.Errors == null)
			{
				return null;
			}

			if (propertyName.IsNullOrEmpty())
			{
				return this.Errors.EntityErrors;
			}
			else
			{
				if (!this.Errors.PropertyErrors.ContainsKey(propertyName))
				{
					return null;
				}

				return this.Errors.PropertyErrors[propertyName];
			}
		}

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		[field: NonSerialized]
		[field: XmlIgnore]
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
