﻿using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services
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
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public static class ServiceLocator
	{
		#region Static Constructor/Destructor

		static ServiceLocator ()
		{
			ServiceLocator.GlobalSyncRoot = new object();

			ServiceLocator.Cache = new Dictionary<string, object[]>(CompositionContainer.NameComparer);
			ServiceLocator.CompositionContainerBindings = new HashSet<CompositionContainer>();

			ServiceLocator.UseCaching = true;
			ServiceLocator.UseSingletons = true;
		}

		#endregion




		#region Static Fields

		private static bool _useCaching;

		private static bool _useSingletons;

		#endregion




		#region Static Properties/Indexer

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

		private static Dictionary<string, object[]> Cache { get; set; }

		private static HashSet<CompositionContainer> CompositionContainerBindings { get; set; }

		private static object GlobalSyncRoot { get; set; }

		#endregion




		#region Static Events

		/// <summary>
		///     Raised when a service is to be looked-up by its name.
		/// </summary>
		public static event Func<string, IList<object>> Lookup;

		/// <summary>
		///     Raised when a type needs to be translated to a name.
		/// </summary>
		/// <remarks>
		///     This event is raised before <see cref="Lookup" /> in case the lookup is specified using a type instead of a name so that the type needs to be translated into a name which then can be used for the actual lookup using <see cref="Lookup" />.
		/// </remarks>
		public static event Func<Type, string> Translate;

		#endregion




		#region Static Methods

		/// <summary>
		///     Binds the service locator to a specified composition container which is then used for service lookup.
		/// </summary>
		/// <param name="compositionContainer"> The composition container to bind to. </param>
		/// <remarks>
		///     <para>
		///         <see cref="CompositionContainer.GetNameOfType" /> is used for type-to-name translation and <see cref="CompositionContainer.GetExports{T}(string)" /> for lookup.
		///     </para>
		///     <para>
		///         It is possible to bind multiple composition containers to the service locator.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="compositionContainer" /> is null. </exception>
		public static void BindToCompositionContainer (CompositionContainer compositionContainer)
		{
			if (compositionContainer == null)
			{
				throw new ArgumentNullException(nameof(compositionContainer));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.CompositionContainerBindings.Add(compositionContainer);
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
		///     Unbinds the service locator from a specified composition container which is then no longer used for service lookup.
		/// </summary>
		/// <param name="compositionContainer"> The composition container to unbind from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="compositionContainer" /> is null. </exception>
		public static void UnbindFromCompositionContainer (CompositionContainer compositionContainer)
		{
			if (compositionContainer == null)
			{
				throw new ArgumentNullException(nameof(compositionContainer));
			}

			lock (ServiceLocator.GlobalSyncRoot)
			{
				ServiceLocator.CompositionContainerBindings.Remove(compositionContainer);
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
			bool useSingletons;
			lock (ServiceLocator.GlobalSyncRoot)
			{
				useSingletons = ServiceLocator.UseSingletons;
				if (ServiceLocator.UseCaching && ServiceLocator.Cache.ContainsKey(name))
				{
					return ServiceLocator.Cache[name];
				}
			}

			List<object> instances = new List<object>(ServiceLocator.Lookup?.Invoke(name) ?? new object[0]);

			lock (ServiceLocator.GlobalSyncRoot)
			{
				foreach (CompositionContainer container in ServiceLocator.CompositionContainerBindings)
				{
					instances.AddRange(container.GetExports<object>(name));
				}
			}

			if ((typeHint != null) && useSingletons)
			{
				object singleton = Singleton.Get(typeHint);
				if (singleton != null)
				{
					instances.Add(singleton);
				}
			}

			object[] resolved = instances.ToArray();

			lock (ServiceLocator.GlobalSyncRoot)
			{
				if (ServiceLocator.UseCaching && (resolved.Length > 0))
				{
					ServiceLocator.Cache.Remove(name);
					ServiceLocator.Cache.Add(name, resolved);
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

			return ServiceLocator.Translate?.Invoke(type) ?? CompositionContainer.GetNameOfType(type);
		}

		#endregion
	}
}
