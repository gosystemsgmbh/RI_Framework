using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.ComponentModel
{
	/// <summary>
	///     Provides a centralized and global locator to lookup services and instances.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The actual lookup is performed by handling the <see cref="Translate" /> and <see cref="Lookup" /> events.
	///         This allows to globally retrieve services in a way which is independent on how they are structured, instantiated, and managed.
	///     </para>
	///     <para>
	///         A &quot;service&quot; can actually be any object/instance which is required to be made globally available (comparable to a singleton).
	///     </para>
	///     <para>
	///         To improve performance, caching can be used (controlled by <see cref="UseCaching" />).
	///         If caching is used, previously retrieved services are cached and no lookup is performed for those.
	///         Caching is enabled by default.
	///     </para>
	///     <para>
	///         <see cref="ServiceLocator" /> can be tied to <see cref="Singleton" /> and <see cref="Singleton{T}" />, controllable through <see cref="UseSingletons" />.
	///         If enabled, services which cannot be resolved through a lookup are tried to be resolved using the singletons.
	///         It can only be used when working with types, not with names.
	///         The connection to the singletons is enabled by default.
	///     </para>
	///     <para>
	///         By default, if <see cref="Translate" /> is not handled, <see cref="CompositionContainer.GetNameOfType" /> is used to translate a type to a name.
	///     </para>
	///     <note type="note">
	///         The use of a service locator is considered a bad practice in some contexts and under some circumstances.
	///         Please reflect carefully whether a service locator is the right architectural/design choice.
	///         Dependency injection, e.g. using <see cref="CompositionContainer" />, might be a better approach.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public static class ServiceLocator
	{
		#region Static Constructor/Destructor

		static ServiceLocator ()
		{
			ServiceLocator.GlobalSyncRoot = new object();

			ServiceLocator.Cache = new Dictionary<string, object[]>(CompositionContainer.NameComparer);
			ServiceLocator.DependencyResolverBindings = new HashSet<IDependencyResolver>();
			ServiceLocator.ServiceProviderBindings = new HashSet<IServiceProvider>();

			ServiceLocator.UseCaching = true;
			ServiceLocator.UseSingletons = true;

			ServiceLocator.CacheVersion = 0;

			ServiceLocator.Resolver = new ServiceLocatorResolver();
			ServiceLocator.Provider = new ServiceLocatorProvider();
		}

		#endregion




		#region Static Fields

		private static bool _useCaching;

		private static bool _useSingletons;

		#endregion




		#region Static Properties/Indexer

		/// <summary>
		///     Gets a service provider which uses <see cref="ServiceLocator" />.
		/// </summary>
		/// <value>
		///     A service provider which uses <see cref="ServiceLocator" />.
		/// </value>
		public static IServiceProvider Provider { get; }

		/// <summary>
		///     Gets a dependency resolver which uses <see cref="ServiceLocator" />.
		/// </summary>
		/// <value>
		///     A dependency resolver which uses <see cref="ServiceLocator" />.
		/// </value>
		public static IDependencyResolver Resolver { get; }

		/// <summary>
		///     Gets or sets whether cahcing is used.
		/// </summary>
		/// <value>
		///     true if caching is used, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public static bool UseCaching
		{
			get
			{
				lock (ServiceLocator.GlobalSyncRoot)
				{
					return ServiceLocator._useCaching;
				}
			}
			set
			{
				lock (ServiceLocator.GlobalSyncRoot)
				{
					ServiceLocator._useCaching = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets whether <see cref="ServiceLocator" /> is connected to <see cref="Singleton" /> / <see cref="Singleton{T}" />.
		/// </summary>
		/// <value>
		///     true if connected to <see cref="Singleton" /> / <see cref="Singleton{T}" />, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public static bool UseSingletons
		{
			get
			{
				lock (ServiceLocator.GlobalSyncRoot)
				{
					return ServiceLocator._useSingletons;
				}
			}
			set
			{
				lock (ServiceLocator.GlobalSyncRoot)
				{
					ServiceLocator._useSingletons = value;
				}
			}
		}

		private static Dictionary<string, object[]> Cache { get; }

		private static int CacheVersion { get; set; }

		private static HashSet<IDependencyResolver> DependencyResolverBindings { get; }

		private static object GlobalSyncRoot { get; }

		private static HashSet<IServiceProvider> ServiceProviderBindings { get; }

		#endregion




		#region Static Events

		/// <summary>
		///     Raised when a service is to be looked-up by its name.
		/// </summary>
		public static event EventHandler<ServiceLocatorLookupEventArgs> Lookup;

		/// <summary>
		///     Raised when a type needs to be translated to a name.
		/// </summary>
		/// <remarks>
		///     This event is raised before <see cref="Lookup" /> in case the lookup is specified using a type instead of a name so that the type needs to be translated into a name which then can be used for the actual lookup using <see cref="Lookup" />.
		/// </remarks>
		public static event EventHandler<ServiceLocatorTranslationEventArgs> Translate;

		#endregion




		#region Static Methods

		/// <summary>
		///     Binds the service locator to a specified dependency resolver (e.g. <see cref="CompositionContainer" />) which is then used for service lookup.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver to bind to. </param>
		/// <remarks>
		///     <para>
		///         It is possible to bind multiple dependency resolvers to the service locator.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		public static void BindToDependencyResolver (IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null)
			{
				throw new ArgumentNullException(nameof(dependencyResolver));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.DependencyResolverBindings.Add(dependencyResolver);
			}
		}

		/// <summary>
		///     Binds the service locator to a specified service provider which is then used for service lookup.
		/// </summary>
		/// <param name="serviceProvider"> The service provider to bind to. </param>
		/// <remarks>
		///     <para>
		///         It is possible to bind multiple dependency resolvers to the service locator.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="serviceProvider" /> is null. </exception>
		public static void BindToServiceProvider (IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.ServiceProviderBindings.Add(serviceProvider);
			}
		}

		/// <summary>
		///     Clears the cache.
		/// </summary>
		public static void ClearCache ()
		{
			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.Cache.Clear();
				ServiceLocator.CacheVersion += 1;
			}
		}

		/// <summary>
		///     Retrieves a service instance by its type.
		/// </summary>
		/// <typeparam name="T"> The type of the service to retrieve. </typeparam>
		/// <returns>
		///     The service instance or null if it cannot be found.
		/// </returns>
		public static T GetInstance <T> ()
			where T : class
		{
			return ServiceLocator.GetInstance(typeof(T)) as T;
		}

		/// <summary>
		///     Retrieves a service instance by its type.
		/// </summary>
		/// <param name="type"> The type of the service to retrieve. </param>
		/// <typeparam name="T"> The type to which the service is converted to. </typeparam>
		/// <returns>
		///     The service instance or null if it cannot be found or converted to <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static T GetInstance <T> (Type type)
			where T : class
		{
			return ServiceLocator.GetInstance(type) as T;
		}

		/// <summary>
		///     Retrieves a service instance by its name.
		/// </summary>
		/// <param name="name"> The name of the service to retrieve. </param>
		/// <typeparam name="T"> The type to which the service is converted to. </typeparam>
		/// <returns>
		///     The service instance or null if it cannot be found or converted to <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static T GetInstance <T> (string name)
			where T : class
		{
			return ServiceLocator.GetInstance(name) as T;
		}

		/// <summary>
		///     Retrieves a service instance by its type.
		/// </summary>
		/// <param name="type"> The type of the service to retrieve. </param>
		/// <returns>
		///     The service instance or null if it cannot be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static object GetInstance (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			string name = ServiceLocator.TranslateTypeToName(type);
			return ServiceLocator.LookupService(name, type);
		}

		/// <summary>
		///     Retrieves a service instance by its name.
		/// </summary>
		/// <param name="name"> The name of the service to retrieve. </param>
		/// <returns>
		///     The service instance or null if it cannot be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static object GetInstance (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return ServiceLocator.LookupService(name, null);
		}

		/// <summary>
		///     Retrieves service instances by type.
		/// </summary>
		/// <typeparam name="T"> The type of the services to retrieve. </typeparam>
		/// <returns>
		///     The array of service instances.
		///     An empty array is returned if no services can be found.
		/// </returns>
		public static T[] GetInstances <T> ()
			where T : class
		{
			return ServiceLocator.GetInstances(typeof(T)).OfType<T>().ToArray();
		}

		/// <summary>
		///     Retrieves service instances by type.
		/// </summary>
		/// <param name="type"> The type of the services to retrieve. </param>
		/// <typeparam name="T"> The type to which the services are converted to. </typeparam>
		/// <returns>
		///     The array of service instances.
		///     An empty array is returned if no services can be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static T[] GetInstances <T> (Type type)
			where T : class
		{
			return ServiceLocator.GetInstances(type).OfType<T>().ToArray();
		}

		/// <summary>
		///     Retrieves service instances by name.
		/// </summary>
		/// <param name="name"> The name of the services to retrieve. </param>
		/// <typeparam name="T"> The type to which the services are converted to. </typeparam>
		/// <returns>
		///     The array of service instances.
		///     An empty array is returned if no services can be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static T[] GetInstances <T> (string name)
			where T : class
		{
			return ServiceLocator.GetInstances(name).OfType<T>().ToArray();
		}

		/// <summary>
		///     Retrieves services instance by type.
		/// </summary>
		/// <param name="type"> The type of the services to retrieve. </param>
		/// <returns>
		///     The array of service instances.
		///     An empty array is returned if no services can be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static object[] GetInstances (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			string name = ServiceLocator.TranslateTypeToName(type);
			return ServiceLocator.LookupServices(name, type);
		}

		/// <summary>
		///     Retrieves service instances by name.
		/// </summary>
		/// <param name="name"> The name of the services to retrieve. </param>
		/// <returns>
		///     The array of service instances.
		///     An empty array is returned if no services can be found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static object[] GetInstances (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return ServiceLocator.LookupServices(name, null);
		}

		/// <summary>
		///     Unbinds the service locator from a specified dependency resolver (e.g. <see cref="CompositionContainer" />) which is then no longer used for service lookup.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver to unbind from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		public static void UnbindFromDependencyResolver (IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null)
			{
				throw new ArgumentNullException(nameof(dependencyResolver));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.DependencyResolverBindings.Remove(dependencyResolver);
			}
		}

		/// <summary>
		///     Unbinds the service locator from a specified service provider which is then no longer used for service lookup.
		/// </summary>
		/// <param name="serviceProvider"> The service provider to unbind from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="serviceProvider" /> is null. </exception>
		public static void UnbindFromServiceProvider (IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.ServiceProviderBindings.Remove(serviceProvider);
			}
		}

		private static object LookupService (string name, Type typeHint)
		{
			if (name == null)
			{
				return null;
			}

			object[] instances = ServiceLocator.Resolve(name, typeHint);
			return instances.Length == 0 ? null : instances[0];
		}

		private static object[] LookupServices (string name, Type typeHint)
		{
			if (name == null)
			{
				return new object[0];
			}

			object[] instances = ServiceLocator.Resolve(name, typeHint);
			return instances;
		}

		private static object[] Resolve (string name, Type typeHint)
		{
			object[] resolved = null;
			bool cont = true;

			while (cont)
			{
				int cacheVersion;
				bool useSingletons;
				bool useCaching;
				HashSet<IDependencyResolver> resolverBindings;
				HashSet<IServiceProvider> providerBindings;

				lock (ServiceLocator.GlobalSyncRoot)
				{
					if (ServiceLocator.UseCaching && ServiceLocator.Cache.ContainsKey(name))
					{
						return ServiceLocator.Cache[name];
					}

					ServiceLocator.CacheVersion += 1;

					cacheVersion = ServiceLocator.CacheVersion;
					useSingletons = ServiceLocator.UseSingletons;
					useCaching = ServiceLocator.UseCaching;
					resolverBindings = new HashSet<IDependencyResolver>(ServiceLocator.DependencyResolverBindings);
					providerBindings = new HashSet<IServiceProvider>(ServiceLocator.ServiceProviderBindings);
				}

				ServiceLocatorLookupEventArgs args = new ServiceLocatorLookupEventArgs(name);
				ServiceLocator.Lookup?.Invoke(null, args);
				HashSet<object> instances = new HashSet<object>(args.Instances);

				foreach (IDependencyResolver resolver in resolverBindings)
				{
					instances.AddRange(resolver.GetInstances(name));
				}

				if (typeHint != null)
				{
					foreach (IServiceProvider provider in providerBindings)
					{
						instances.Add(provider.GetService(typeHint));
					}

					if (useSingletons)
					{
						object singleton = Singleton.Get(typeHint);
						if (singleton != null)
						{
							instances.Add(singleton);
						}
					}
				}

				resolved = instances.ToArray();

				lock (ServiceLocator.GlobalSyncRoot)
				{
					if (useCaching && (resolved.Length > 0))
					{
						ServiceLocator.Cache.Remove(name);
						ServiceLocator.Cache.Add(name, resolved);
					}

					cont = cacheVersion != ServiceLocator.CacheVersion;
				}
			}

			return resolved;
		}

		private static string TranslateTypeToName (Type type)
		{
			if (type == null)
			{
				return null;
			}

			ServiceLocatorTranslationEventArgs args = new ServiceLocatorTranslationEventArgs(type);
			args.Name = CompositionContainer.GetNameOfType(type);
			ServiceLocator.Translate?.Invoke(null, args);
			return args.Name;
		}

		#endregion




		#region Type: ServiceLocatorProvider

		private sealed class ServiceLocatorProvider : IServiceProvider
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

				return ServiceLocator.GetInstance(serviceType);
			}

			#endregion
		}

		#endregion




		#region Type: ServiceLocatorResolver

		private sealed class ServiceLocatorResolver : IDependencyResolver
		{
			#region Interface: IDependencyResolver

			object IDependencyResolver.GetInstance (Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if ((!type.IsClass) && (!type.IsInterface))
				{
					throw new InvalidTypeArgumentException(nameof(type));
				}

				return ServiceLocator.GetInstance(type);
			}

			object IDependencyResolver.GetInstance (string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException(nameof(name));
				}

				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}

				return ServiceLocator.GetInstance(name);
			}

			T IDependencyResolver.GetInstance <T> ()
			{
				return (T)((IDependencyResolver)this).GetInstance(typeof(T));
			}

			List<object> IDependencyResolver.GetInstances (Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if ((!type.IsClass) && (!type.IsInterface))
				{
					throw new InvalidTypeArgumentException(nameof(type));
				}

				return new List<object>(ServiceLocator.GetInstances(type));
			}

			List<object> IDependencyResolver.GetInstances (string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException(nameof(name));
				}

				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}

				return new List<object>(ServiceLocator.GetInstances(name));
			}

			List<T> IDependencyResolver.GetInstances <T> ()
			{
				return ((IDependencyResolver)this).GetInstances(typeof(T)).Cast<T>();
			}

			#endregion
		}

		#endregion
	}
}
