using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.EF.Resolvers;

namespace RI.Framework.Data.SqlServer
{
	/// <summary>
	///     Implements a default code-based database configuration for SQL Server databases.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This database configuration sets up the ADO.NET and Entity Framework provider factories and provider services for SQL Server.
	///         It also registers a default resolver using <see cref="ServiceLocatorDbDependencyResolver" />.
	///     </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public abstract class SqlServerDbConfiguration : DbConfiguration
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SqlServerDbConfiguration" />.
		/// </summary>
		protected SqlServerDbConfiguration ()
		{
			this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());

			this.SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);

			this.AddDefaultResolver(new ServiceLocatorDbDependencyResolver());
		}

		#endregion
	}
}
