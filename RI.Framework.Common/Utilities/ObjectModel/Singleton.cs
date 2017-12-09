using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.ObjectModel
{
	/// <summary>
	///     Provides a centralized functionality to store singletons.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         <see cref="Singleton" /> shares its singleton instances with <see cref="Singleton{T}" />.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // gets an existing singleton or null if it does not exist
	/// var playerManager = Singleton.Get(typeof(PlayerManager));
	/// 
	/// // sets a new or replaces an existing singleton
	/// Singleton.Set(typeof(PlayerManager), new PlayerManager());
	/// 
	/// // gets an existing singleton or creates one if it does not exist
	/// // (might throw an exception if the type has no default constructor)
	/// var gameRules = Singleton.Ensure(typeof(GameRules));
	/// 
	/// // gets an existing singleton or gets one from a callback if it does not exist
	/// var enemyManager = Singleton.Ensure(typeof(EnemyManager), () => new EnemyManager());
	/// ]]>
	/// </code>
	/// </example>
	public static class Singleton
	{
		#region Static Constructor/Destructor

		static Singleton ()
		{
			Singleton.GlobalSyncRoot = new object();
			Singleton.Instances = new Dictionary<Type, object>();
			Singleton.Resolver = new SingletonResolver();
			Singleton.Provider = new SingletonProvider();
		}

		#endregion




		#region Static Properties/Indexer

		/// <summary>
		///     Gets a service provider which uses <see cref="Singleton" />.
		/// </summary>
		/// <value>
		///     A service provider which uses <see cref="Singleton" />.
		/// </value>
		public static IServiceProvider Provider { get; }

		/// <summary>
		///     Gets a dependency resolver which uses <see cref="Singleton" />.
		/// </summary>
		/// <value>
		///     A dependency resolver which uses <see cref="Singleton" />.
		/// </value>
		public static IDependencyResolver Resolver { get; }

		internal static object GlobalSyncRoot { get; }

		private static Dictionary<Type, object> Instances { get; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <param name="type"> The type of the singleton. </param>
		/// <returns>
		///     The current instance of the singleton.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method uses <see cref="Activator" /> to create a new instance of the singleton type if there is currently no instance set.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static object Ensure (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			lock (Singleton.GlobalSyncRoot)
			{
				if (!Singleton.Instances.ContainsKey(type))
				{
					Singleton.Instances.Add(type, Activator.CreateInstance(type));
				}
				return Singleton.Instances[type];
			}
		}

		/// <summary>
		///     Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <param name="type"> The type of the singleton. </param>
		/// <param name="creator"> The creator delegate used to create the singleton. </param>
		/// <returns>
		///     The current instance of the singleton.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method uses the delegate specified by <paramref name="creator" /> to create a new instance of the singleton type if there is currently no instance set.
		///         This is useful in cases the singleton types constructor has parameters or can only be created through a static method.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="creator" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="creator" /> did not return a new instance of the singleton type. </exception>
		public static object Ensure (Type type, Func<object> creator)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			lock (Singleton.GlobalSyncRoot)
			{
				if (!Singleton.Instances.ContainsKey(type))
				{
					object instance = creator();
					if (instance == null)
					{
						throw new InvalidOperationException("The creator delegate did not return a new instance of the singleton.");
					}
					Singleton.Instances.Add(type, instance);
				}
				return Singleton.Instances[type];
			}
		}

		/// <summary>
		///     Gets the current instance of a singleton.
		/// </summary>
		/// <param name="type"> The type of the singleton. </param>
		/// <returns>
		///     The current instance of the singleton or null if there is no current instance set.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static object Get (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			lock (Singleton.GlobalSyncRoot)
			{
				if (Singleton.Instances.ContainsKey(type))
				{
					return Singleton.Instances[type];
				}

				return null;
			}
		}

		/// <summary>
		///     Sets the current instance of a singleton.
		/// </summary>
		/// <param name="type"> The type of the singleton. </param>
		/// <param name="instance"> The current instance of the singleton or null if there is no current instance set. </param>
		/// <returns>
		///     The current instance of the singleton or null if there is no current instance set.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static object Set (Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			lock (Singleton.GlobalSyncRoot)
			{
				Singleton.Instances.Remove(type);

				if (instance != null)
				{
					Singleton.Instances.Add(type, instance);
				}

				return instance;
			}
		}

		#endregion




		#region Type: SingletonProvider

		private sealed class SingletonProvider : IServiceProvider
		{
			#region Interface: IServiceProvider

			public object GetService (Type serviceType)
			{
				if (serviceType == null)
				{
					throw new ArgumentNullException(nameof(serviceType));
				}

				if ((!serviceType.IsClass) && (!serviceType.IsInterface))
				{
					throw new InvalidTypeArgumentException(nameof(serviceType));
				}

				return Singleton.Get(serviceType);
			}

			#endregion
		}

		#endregion




		#region Type: SingletonResolver

		private sealed class SingletonResolver : IDependencyResolver
		{
			#region Interface: IDependencyResolver

			public object GetInstance (Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if ((!type.IsClass) && (!type.IsInterface))
				{
					throw new InvalidTypeArgumentException(nameof(type));
				}

				return Singleton.Get(type);
			}

			public object GetInstance (string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException(nameof(name));
				}

				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}

				Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => string.Equals(x.FullName, name, StringComparison.Ordinal));
				if (type == null)
				{
					return null;
				}

				return this.GetInstance(type);
			}

			public T GetInstance <T> ()
				where T : class
			{
				return (T)this.GetInstance(typeof(T));
			}

			public List<object> GetInstances (Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if ((!type.IsClass) && (!type.IsInterface))
				{
					throw new InvalidTypeArgumentException(nameof(type));
				}

				List<object> instances = new List<object>();
				instances.Add(this.GetInstance(type));
				return instances;
			}

			public List<object> GetInstances (string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException(nameof(name));
				}

				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}

				List<object> instances = new List<object>();
				instances.Add(this.GetInstance(name));
				return instances;
			}

			public List<T> GetInstances <T> ()
				where T : class
			{
				return this.GetInstances(typeof(T)).Cast<T>();
			}

			#endregion
		}

		#endregion
	}


	/// <summary>
	///     Provides a centralized functionality to store singletons.
	/// </summary>
	/// <typeparam name="T"> The singleton type. </typeparam>
	/// <remarks>
	///     <note type="note">
	///         <see cref="Singleton{T}" /> shares its singleton instances with <see cref="Singleton" />.
	///     </note>
	/// </remarks>
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // gets an existing singleton or null if it does not exist
	/// var playerManager = Singleton<PlayerManager>.Instance;
	/// 
	/// // sets a new or replaces an existing singleton
	/// Singleton<PlayerManager>.Instance = new PlayerManager();
	/// 
	/// // gets an existing singleton or creates one if it does not exist
	/// // (might throw an exception if the type has no default constructor)
	/// var gameRules = Singleton<GameRules>.Ensure();
	/// 
	/// // gets an existing singleton or gets one from a callback if it does not exist
	/// var enemyManager = Singleton<EnemyManager>.Ensure(() => new EnemyManager());
	/// ]]>
	/// </code>
	/// </example>
	public static class Singleton <T>
		where T : class
	{
		#region Static Properties/Indexer

		/// <summary>
		///     Gets or sets the current instance of the singleton.
		/// </summary>
		/// <value>
		///     The current instance of the singleton or null if there is no current instance set.
		/// </value>
		public static T Instance
		{
			get
			{
				return Singleton.Get(typeof(T)) as T;
			}
			set
			{
				Singleton.Set(typeof(T), value);
			}
		}

		#endregion




		#region Static Methods

		/// <summary>
		///     Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <returns>
		///     The current instance of the singleton.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method uses <see cref="Activator" /> to create a new instance of the singleton type if there is currently no instance set.
		///     </para>
		/// </remarks>
		public static T Ensure ()
		{
			lock (Singleton.GlobalSyncRoot)
			{
				if (Singleton<T>.Instance == null)
				{
					Singleton<T>.Instance = Activator.CreateInstance<T>();
				}
				return Singleton<T>.Instance;
			}
		}

		/// <summary>
		///     Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <param name="creator"> The creator delegate used to create the singleton. </param>
		/// <returns>
		///     The current instance of the singleton.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method uses the delegate specified by <paramref name="creator" /> to create a new instance of the singleton type if there is currently no instance set.
		///         This is useful in cases the singleton types constructor has parameters or can only be created through a static method.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="creator" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="creator" /> did not return a new instance of the singleton type. </exception>
		public static T Ensure (Func<T> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			lock (Singleton.GlobalSyncRoot)
			{
				if (Singleton<T>.Instance == null)
				{
					Singleton<T>.Instance = creator();
					if (Singleton<T>.Instance == null)
					{
						throw new InvalidOperationException("The creator delegate did not return a new instance of the singleton.");
					}
				}
				return Singleton<T>.Instance;
			}
		}

		#endregion
	}
}
