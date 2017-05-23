using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;

using RI.Framework.Services;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Implements a dependency resolver for Entity Framework contexts which uses <see cref="ServiceLocator" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The key parameter of <see cref="GetService" /> and <see cref="GetServices" /> is ignored.
	///     </note>
	/// </remarks>
	public sealed class ServiceLocatorDbDependencyResolver : IDbDependencyResolver
	{
		#region Interface: IDbDependencyResolver

		/// <inheritdoc />
		public object GetService (Type type, object key)
		{
			return ServiceLocator.GetInstance(type) ?? Singleton.Get(type);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetServices (Type type, object key)
		{
			List<object> values = new List<object>();
			values.AddRange(ServiceLocator.GetInstances(type));
			object singleton = Singleton.Get(type);
			if (singleton != null)
			{
				values.Add(singleton);
			}
			return values;
		}

		#endregion
	}
}
