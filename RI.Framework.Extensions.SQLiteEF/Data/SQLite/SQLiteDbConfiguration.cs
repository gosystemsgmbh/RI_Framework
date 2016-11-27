﻿using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.Linq;

using RI.Framework.Data.EF;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Implements a default code-based database configuration for SQLite databases.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This database configuration sets up the ADO.NET and Entity Framework provider factories and provider services for SQLite.
	///         It also registers a default resolver using <see cref="ServiceLocatorDbDependencyResolver" />.
	///     </para>
	/// </remarks>
	/// TODO: Why the fuck does this not work?!
	public abstract class SQLiteDbConfiguration : DbConfiguration
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDbConfiguration" />.
		/// </summary>
		public SQLiteDbConfiguration ()
		{
			this.SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
			this.SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

			this.SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));

			this.AddDefaultResolver(new ServiceLocatorDbDependencyResolver());
		}

		#endregion
	}
}
