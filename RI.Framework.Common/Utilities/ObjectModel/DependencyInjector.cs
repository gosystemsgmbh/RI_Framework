using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;




namespace RI.Framework.Utilities.ObjectModel
{
	/// <summary>
	///     Implemens a wrapper for <see cref="IDependencyResolver" />s which allows modification/interception of resolved instances.
	/// </summary>
	public abstract class DependencyInjector : IDependencyResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DependencyInjector" />.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		protected DependencyInjector (IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null)
			{
				throw new ArgumentNullException(nameof(dependencyResolver));
			}

			this.DependencyResolver = dependencyResolver;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dependency resolver.
		/// </summary>
		/// <value>
		///     The used dependency resolver.
		/// </value>
		public IDependencyResolver DependencyResolver { get; }

		#endregion




		#region Instance Methods

		private List<T> InterceptInternal <T> (List<T> list)
			where T : class => this.InterceptInternal(typeof(T), list.OfType<object>()).OfType<T>().ToList();

		private List<object> InterceptInternal (string name, List<object> list)
		{
			this.Intercept(name, list);
			return list;
		}

		private List<object> InterceptInternal (Type type, List<object> list)
		{
			this.Intercept(type, list);
			return list;
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Intercepts instance resolving by name.
		/// </summary>
		/// <param name="name"> The name to resolve. </param>
		/// <param name="instances"> The list of instances already resolved by <see cref="DependencyResolver" /> which can be modified to perform the interception. </param>
		protected abstract void Intercept (string name, List<object> instances);

		#endregion




		#region Virtuals

		/// <summary>
		///     Intercepts instance resolving by type.
		/// </summary>
		/// <param name="type"> The type to resolve. </param>
		/// <param name="instances"> The list of instances already resolved by <see cref="DependencyResolver" /> which can be modified to perform the interception. </param>
		/// <remarks>
		///     <para>
		///         The default implementation uses <see cref="CompositionContainer.GetNameOfType" /> to get the name of the type and then uses <see cref="Intercept(string,List{object})" />.
		///     </para>
		/// </remarks>
		protected virtual void Intercept (Type type, List<object> instances) => this.Intercept(CompositionContainer.GetNameOfType(type), instances);

		#endregion




		#region Interface: IDependencyResolver

		/// <inheritdoc />
		public object GetInstance (Type type) => this.InterceptInternal(type, this.DependencyResolver.GetInstances(type)).GetIndexOrDefault(0);

		/// <inheritdoc />
		public object GetInstance (string name) => this.InterceptInternal(name, this.DependencyResolver.GetInstances(name)).GetIndexOrDefault(0);

		/// <inheritdoc />
		public T GetInstance <T> ()
			where T : class => this.InterceptInternal(this.DependencyResolver.GetInstances<T>()).GetIndexOrDefault(0);

		/// <inheritdoc />
		public List<object> GetInstances (Type type) => this.InterceptInternal(type, this.DependencyResolver.GetInstances(type));

		/// <inheritdoc />
		public List<object> GetInstances (string name) => this.InterceptInternal(name, this.DependencyResolver.GetInstances(name));

		/// <inheritdoc />
		public List<T> GetInstances <T> ()
			where T : class => this.InterceptInternal(this.DependencyResolver.GetInstances<T>());

		#endregion
	}
}
