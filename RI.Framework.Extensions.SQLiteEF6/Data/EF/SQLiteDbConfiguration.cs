using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.EF.Resolvers;




namespace RI.Framework.Data.EF
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
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public abstract class SQLiteDbConfiguration : DbConfiguration
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDbConfiguration" />.
		/// </summary>
		protected SQLiteDbConfiguration ()
		{
			this.SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
			this.SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

			this.SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
			this.SetProviderServices("System.Data.SQLite.EF6", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));

			this.AddDefaultResolver(new ServiceLocatorDbDependencyResolver());
		}

		#endregion
	}
}
