using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="object" /> type.
	/// </summary>
	public static class ObjectExtensions
	{
		#region Static Constructor/Destructor

		static ObjectExtensions ()
		{
			ObjectExtensions.GetSyncRootSyncRoot = new object();
			ObjectExtensions.IsSynchronizedGetters = new Dictionary<Type, MethodInfo>();
			ObjectExtensions.SyncRootGetters = new Dictionary<Type, MethodInfo>();
		}

		#endregion




		#region Static Properties/Indexer

		private static object GetSyncRootSyncRoot { get; }
		private static Dictionary<Type, MethodInfo> IsSynchronizedGetters { get; }
		private static Dictionary<Type, MethodInfo> SyncRootGetters { get; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Performs a deep clone of an object.
		/// </summary>
		/// <typeparam name="T"> The type of the object to clone. </typeparam>
		/// <param name="obj"> The object to clone. </param>
		/// <returns>
		///     The deeply cloned object.
		/// </returns>
		/// <remarks>
		///     <para>
		///         To perform deep cloning, the object to be cloned is serialized into memory using binary serialization (<see cref="BinaryFormatter" />), and then deserialized.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		/// <exception cref="NotSupportedException"> The object does not support deep cloning, usually because it cannot be serialized using binary serialization. </exception>
		public static T CloneDeep <T> (this T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					BinaryFormatter serializer = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
					serializer.Serialize(ms, obj);
					ms.Flush();
					ms.Position = 0;
					return (T)serializer.Deserialize(ms);
				}
			}
			catch (Exception exception)
			{
				throw new NotSupportedException("The object of type " + obj.GetType().Name + " does not support deep cloning.", exception);
			}
		}

		/// <summary>
		///     Attempts to clone an object or returns a default value if the object cannot be cloned.
		/// </summary>
		/// <typeparam name="T"> The type of the object to clone. </typeparam>
		/// <param name="obj"> The object to clone. </param>
		/// <returns>
		///     The cloned object or the default value of <typeparamref name="T" /> if the object cannot be cloned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="CloneOrDefault{T}(T,T)" /> for more information.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		/// <exception cref="InvalidCastException"> <paramref name="obj" /> implements <see cref="ICloneable" /> but the return value of <see cref="ICloneable.Clone" /> cannot be converted to <typeparamref name="T" />. </exception>
		public static T CloneOrDefault <T> (this T obj)
		{
			return obj.CloneOrDefault(default(T));
		}

		/// <summary>
		///     Attempts to clone an object or returns a default value if the object cannot be cloned.
		/// </summary>
		/// <typeparam name="T"> The type of the object to clone. </typeparam>
		/// <param name="obj"> The object to clone. </param>
		/// <param name="defaultValue"> The default value returned as the clone if the object cannot be cloned. </param>
		/// <returns>
		///     The cloned object or <paramref name="defaultValue" /> if the object cannot be cloned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The cloning is attempted using the following procedure, in the following order:
		///     </para>
		///     <para>
		///         1. If <paramref name="obj" /> implements <see cref="ICloneable{T}" />, <see cref="ICloneable{T}.Clone" /> is called and its return value returned.
		///     </para>
		///     <para>
		///         2. If <paramref name="obj" /> implements <see cref="ICloneable" />, <see cref="ICloneable.Clone" /> is called and its return value returned.
		///     </para>
		///     <para>
		///         3. If none of the above is implemented, <paramref name="defaultValue" /> is returned.
		///     </para>
		///     <note type="note">
		///         Cloning is stopped as soon as one of the types/members is found implemented.
		///         E.g.: If the object implements <see cref="ICloneable{T}" />, cloning stops at this step, regardless of the value returned by <see cref="ICloneable{T}.Clone" />.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		/// <exception cref="InvalidCastException"> <paramref name="obj" /> implements <see cref="ICloneable" /> but the return value of <see cref="ICloneable.Clone" /> cannot be converted to <typeparamref name="T" />. </exception>
		public static T CloneOrDefault <T> (this T obj, T defaultValue)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			ICloneable<T> cloneable1 = obj as ICloneable<T>;
			if (cloneable1 != null)
			{
				return cloneable1.Clone();
			}

			ICloneable cloneable2 = obj as ICloneable;
			if (cloneable2 != null)
			{
				return (T)cloneable2.Clone();
			}

			//TODO: USe reflection

			return defaultValue;
		}

		/// <summary>
		///     Attempts to clone an object or returns the object uncloned if it cannot be cloned.
		/// </summary>
		/// <typeparam name="T"> The type of the object to clone. </typeparam>
		/// <param name="obj"> The object to clone. </param>
		/// <returns>
		///     The cloned value or <paramref name="obj" /> if the object cannot be cloned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="CloneOrDefault{T}(T,T)" /> for more information.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		/// <exception cref="InvalidCastException"> <paramref name="obj" /> implements <see cref="ICloneable" /> but the return value of <see cref="ICloneable.Clone" /> cannot be converted to <typeparamref name="T" />. </exception>
		public static T CloneOrSelf <T> (this T obj)
		{
			return obj.CloneOrDefault(obj);
		}

		/// <summary>
		///     Gets a synchronization object which can be used for synchronized access to an object.
		/// </summary>
		/// <param name="obj"> The object. </param>
		/// <returns>
		///     The synchronization object or null if no synchronization object is available or the object is not synchronized.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The synchronization object is determined using the following procedure, in the following order:
		///     </para>
		///     <para>
		///         1. If <paramref name="obj" /> implements <see cref="ISynchronizable" />, <see cref="ISynchronizable.IsSynchronized" /> is evaluated.
		///         If true, the value of <see cref="ISynchronizable.SyncRoot" /> is returned, null otherwise.
		///     </para>
		///     <para>
		///         2. If <paramref name="obj" /> implements <see cref="ICollection" />, <see cref="ICollection.IsSynchronized" /> is evaluated.
		///         If true, the value of <see cref="ICollection.SyncRoot" /> is returned, null otherwise.
		///     </para>
		///     <para>
		///         3. Reflection is used to determine whether the object implements the following public instance properties:
		///         <c> public bool IsSynchronized { get; } </c> and <c> public [any class type] SyncRoot { get; } </c>.
		///         If so, <c> IsSynchronized </c> is evaluated.
		///         If true, the value of <c> SyncRoot </c> is returned, null otherwise.
		///     </para>
		///     <para>
		///         4. If none of the above is implemented, null is returned.
		///     </para>
		///     <note type="note">
		///         Determination of the synchronization object is stopped as soon as one of the types/members is found implemented.
		///         E.g.: If the object implements <see cref="ISynchronizable" />, determination stops at this step, even if <see cref="ISynchronizable.IsSynchronized" /> is false or <see cref="ISynchronizable.SyncRoot" /> is null.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		public static object GetSyncRoot (this object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			ISynchronizable synchronizable = obj as ISynchronizable;
			if (synchronizable != null)
			{
				return synchronizable.IsSynchronized ? synchronizable.SyncRoot : null;
			}

			ICollection collection = obj as ICollection;
			if (collection != null)
			{
				return collection.IsSynchronized ? collection.SyncRoot : null;
			}

			lock (ObjectExtensions.GetSyncRootSyncRoot)
			{
				Type objType = obj.GetType();
				MethodInfo isSynchronizedGetter = null;
				MethodInfo syncRootGetter = null;

				if (ObjectExtensions.IsSynchronizedGetters.ContainsKey(objType) && ObjectExtensions.SyncRootGetters.ContainsKey(objType))
				{
					isSynchronizedGetter = ObjectExtensions.IsSynchronizedGetters[objType];
					syncRootGetter = ObjectExtensions.SyncRootGetters[objType];
				}

				if ((isSynchronizedGetter == null) && (syncRootGetter == null))
				{
					PropertyInfo[] properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
					foreach (PropertyInfo property in properties)
					{
						if ((isSynchronizedGetter == null) && (property.Name.Equals(nameof(ISynchronizable.IsSynchronized), StringComparison.InvariantCultureIgnoreCase)) && (property.PropertyType == typeof(bool)) && (property.GetIndexParameters().Length == 0))
						{
							isSynchronizedGetter = property.GetGetMethod(false);
						}
						else if ((syncRootGetter == null) && (property.Name.Equals(nameof(ISynchronizable.SyncRoot), StringComparison.InvariantCultureIgnoreCase)) && property.PropertyType.IsClass && (property.GetIndexParameters().Length == 0))
						{
							syncRootGetter = property.GetGetMethod(false);
						}
					}
					if ((isSynchronizedGetter != null) && (syncRootGetter != null))
					{
						ObjectExtensions.IsSynchronizedGetters.Add(objType, isSynchronizedGetter);
						ObjectExtensions.SyncRootGetters.Add(objType, syncRootGetter);
					}
				}

				if ((isSynchronizedGetter != null) && (syncRootGetter != null))
				{
					bool isSynchronized = (bool)isSynchronizedGetter.Invoke(obj, null);
					return isSynchronized ? syncRootGetter.Invoke(obj, null) : null;
				}
			}

			return null;
		}

		#endregion
	}
}
