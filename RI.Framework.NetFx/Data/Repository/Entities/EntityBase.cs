using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
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
	/// <note type="note">
	/// <see cref="EntityBase"/> is serializable, using <see cref="ISerializable"/>.
	/// Therefore, to serialize and deserialize values in types inheriting from <see cref="EntityBase"/>, you must use <see cref="ISerializable.GetObjectData"/> and <see cref="EntityBase(SerializationInfo,StreamingContext)"/> (usually automatically used by serializers/deserializers).
	/// The default implementations of <see cref="SerializeEntityValues"/> and <see cref="DeserializeEntityValues"/> performs serialization as defined by the <see cref="SerializationOptions"/> property.
	/// These methods must be overloaded if custom serialization behaviour needs to be implemented.
	/// </note>
	/// </remarks>
	/// TODO: Add ICloneable
	[Serializable]
	public abstract class EntityBase : INotifyPropertyChanged, IEntityChangeTracking, IEntityErrorTracking, IDataErrorInfo, INotifyDataErrorInfo, ISerializable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityBase" />.
		/// </summary>
		protected EntityBase ()
		{
			this.SerializationOptions = EntityBaseSerializationOptions.All;

			this.CreateTimestamp = null;
			this.CreateContext = null;

			this.ModifyTimestamp = null;
			this.ModifyContext = null;

			this.Errors = null;
		}

		/// <summary>
		/// Creates a new instance of <see cref="EntityBase" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> is null.</exception>
		protected EntityBase (SerializationInfo info, StreamingContext context)
			: this()
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.DeserializeEntityValues(info, context);
		}

		/// <inheritdoc />
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.SerializeEntityValues(info, context);
		}

		/// <summary>
		/// Called when the entity is serialized.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="SerializeSerializationOptions"/>, <see cref="SerializeErrors"/>, <see cref="SerializeCreateTracking"/>, <see cref="SerializeModifyTracking"/>, and/or <see cref="SerializePublicProperties"/>, depending on the value of <see cref="SerializationOptions"/>.
		/// </para>
		/// </remarks>
		protected virtual void SerializeEntityValues (SerializationInfo info, StreamingContext context)
		{
			if ((this.SerializationOptions & EntityBaseSerializationOptions.SerializationOptions) == EntityBaseSerializationOptions.SerializationOptions)
			{
				this.SerializeSerializationOptions(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.Errors) == EntityBaseSerializationOptions.Errors)
			{
				this.SerializeErrors(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.CreateTracking) == EntityBaseSerializationOptions.CreateTracking)
			{
				this.SerializeCreateTracking(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.ModifyTracking) == EntityBaseSerializationOptions.ModifyTracking)
			{
				this.SerializeModifyTracking(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.PublicProperties) == EntityBaseSerializationOptions.PublicProperties)
			{
				this.SerializePublicProperties(info, context);
			}
		}

		/// <summary>
		/// Called when the entity is deserialized.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="DeserializeSerializationOptions"/>, <see cref="DeserializeErrors"/>, <see cref="DeserializeCreateTracking"/>, <see cref="DeserializeModifyTracking"/>, and/or <see cref="DeserializePublicProperties"/>, depending on the value of <see cref="SerializationOptions"/>.
		/// If <see cref="SerializationOptions"/> specifies <see cref="EntityBaseSerializationOptions.SerializationOptions"/>, the serialization options are deserialized first, replacing the value in <see cref="SerializationOptions"/>, and using the deserialized value for further deserialization.
		/// </para>
		/// </remarks>
		protected virtual void DeserializeEntityValues(SerializationInfo info, StreamingContext context)
		{
			EntityBaseSerializationOptions serializationOptions = this.SerializationOptions;
			if ((this.SerializationOptions & EntityBaseSerializationOptions.SerializationOptions) == EntityBaseSerializationOptions.SerializationOptions)
			{
				this.DeserializeSerializationOptions(info, context);
				if ((this.SerializationOptions & EntityBaseSerializationOptions.SerializationOptions) == EntityBaseSerializationOptions.None)
				{
					this.SerializationOptions = serializationOptions;
				}
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.Errors) == EntityBaseSerializationOptions.Errors)
			{
				this.DeserializeErrors(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.CreateTracking) == EntityBaseSerializationOptions.CreateTracking)
			{
				this.DeserializeCreateTracking(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.ModifyTracking) == EntityBaseSerializationOptions.ModifyTracking)
			{
				this.DeserializeModifyTracking(info, context);
			}

			if ((this.SerializationOptions & EntityBaseSerializationOptions.PublicProperties) == EntityBaseSerializationOptions.PublicProperties)
			{
				this.DeserializePublicProperties(info, context);
			}
		}

		/// <summary>
		/// Gets or sets the serialization options for this entity.
		/// </summary>
		/// <value>
		/// 
		/// </value>
		public EntityBaseSerializationOptions SerializationOptions
		{
			get
			{
				return this._serializationOptions;
			}
			set
			{
				EntityBaseSerializationOptions oldValue = this._serializationOptions;
				this._serializationOptions = value;
				bool notify = oldValue != value;
				this.OnPropertyChanged(notify, nameof(this.SerializationOptions));
			}
		}

		/// <summary>
		/// Serializes creation tracking data (<see cref="CreateTimestamp"/>, <see cref="CreateContext"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void SerializeCreateTracking (SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(this.CreateTimestamp), this.CreateTimestamp.GetValueOrDefault(DateTime.MinValue));
			info.AddValue(nameof(this.CreateContext), this.CreateContext);
		}

		/// <summary>
		/// Serializes modification tracking data (<see cref="ModifyTimestamp"/>, <see cref="ModifyContext"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void SerializeModifyTracking (SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(this.ModifyTimestamp), this.ModifyTimestamp.GetValueOrDefault(DateTime.MinValue));
			info.AddValue(nameof(this.ModifyContext), this.ModifyContext);
		}

		/// <summary>
		/// Serializes errors (<see cref="Errors"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void SerializeErrors (SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(this.Errors), this.Errors);
		}

		/// <summary>
		/// Serializes the serialization options (<see cref="SerializationOptions"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void SerializeSerializationOptions(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(this.SerializationOptions), (int)this.SerializationOptions);
		}

		/// <summary>
		/// Deserializes creation tracking data (<see cref="CreateTimestamp"/>, <see cref="CreateContext"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void DeserializeCreateTracking(SerializationInfo info, StreamingContext context)
		{
			DateTime? timestamp;
			string contextString;

			try
			{
				timestamp = info.GetDateTime(nameof(this.CreateTimestamp));
				contextString = info.GetString(nameof(this.CreateContext));
			}
			catch (SerializationException)
			{
				return;
			}

			if (timestamp == DateTime.MinValue)
			{
				timestamp = null;
			}

			this.CreateTimestamp = timestamp;
			this.CreateContext = contextString;
		}

		/// <summary>
		/// Deserializes modification tracking data (<see cref="ModifyTimestamp"/>, <see cref="ModifyContext"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void DeserializeModifyTracking(SerializationInfo info, StreamingContext context)
		{
			DateTime? timestamp;
			string contextString;

			try
			{
				timestamp = info.GetDateTime(nameof(this.ModifyTimestamp));
				contextString = info.GetString(nameof(this.ModifyContext));
			}
			catch (SerializationException)
			{
				return;
			}

			if (timestamp == DateTime.MinValue)
			{
				timestamp = null;
			}

			this.ModifyTimestamp = timestamp;
			this.ModifyContext = contextString;
		}

		/// <summary>
		/// Deserializes errors (<see cref="Errors"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void DeserializeErrors(SerializationInfo info, StreamingContext context)
		{
			RepositorySetErrors errors;

			try
			{
				errors = (RepositorySetErrors)info.GetValue(nameof(this.Errors), typeof(RepositorySetErrors));
			}
			catch (SerializationException)
			{
				return;
			}

			this.Errors = errors;
		}

		/// <summary>
		/// Deserializes the serialization options (<see cref="SerializationOptions"/>).
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void DeserializeSerializationOptions(SerializationInfo info, StreamingContext context)
		{
			EntityBaseSerializationOptions serializationOptions;

			try
			{
				serializationOptions = (EntityBaseSerializationOptions)info.GetInt32(nameof(this.SerializationOptions));
			}
			catch (SerializationException)
			{
				return;
			}

			this.SerializationOptions = serializationOptions;
		}

		/// <summary>
		/// Serializes all public properties.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void SerializePublicProperties (SerializationInfo info, StreamingContext context)
		{
			//TODO: Implement
		}

		/// <summary>
		/// Deserializes all public properties.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected virtual void DeserializePublicProperties (SerializationInfo info, StreamingContext context)
		{
			//TODO: Implement
		}

		#endregion




		#region Instance Fields

		private string _createContext;
		private DateTime? _createTimestamp;
		private RepositorySetErrors _errors;
		private string _modifyContext;
		private DateTime? _modifyTimestamp;
		private EntityBaseSerializationOptions _serializationOptions;

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
