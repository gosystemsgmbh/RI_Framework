using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;

using RI.Framework.Data.Database.Scripts;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Database.Upgrading
{
	/// <summary>
	///     Provides an utility to extract all version upgrade steps from an assembly.
	/// </summary>
	/// <typeparam name="TProcessingStep"> The type of processing steps this utility creates. </typeparam>
	/// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
	/// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
	/// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
	/// <typeparam name="TManager"> The type of the database manager. </typeparam>
	/// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
	/// <remarks>
	///     <para>
	///         It is assumed that each version upgrade step has its own script file which is added to an assembly as an embedded resource and all script files share a common name format (e.g. <c> upgrade01 </c>).
	///     </para>
	/// </remarks>
	public abstract class AssemblyResourceVersionUpgraderUtility <TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TProcessingStep : IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
	{
		#region Instance Methods

		/// <summary>
		///     Extracts all verion upgrade steps from an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <param name="nameFormat"> The common name format of the embedded script files. </param>
		/// <param name="steps"> Returns the list of upgrade steps. </param>
		/// <param name="scriptLocator"> Returns the script locator for the assembly. </param>
		/// <returns>
		///     true if at least one version upgrade step was extracted, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="nameFormat" /> must be a format string, as used by <see cref="string.Format(IFormatProvider,string,object[])" />.
		///         <see cref="GetUpgradeStepsFromAssembly" /> provides one argument to the format string, <c> {0} </c>, which is an incrementing number, starting at zero, which indicates the source version of a version upgrade script.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> or <paramref name="nameFormat" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="nameFormat" /> is an empty string. </exception>
		public bool GetUpgradeStepsFromAssembly (Assembly assembly, string nameFormat, out List<TProcessingStep> steps, out AssemblyRessourceScriptLocator scriptLocator)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			if (nameFormat == null)
			{
				throw new ArgumentNullException(nameof(nameFormat));
			}

			if (nameFormat.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(nameFormat));
			}

			steps = new List<TProcessingStep>();
			string[] names = assembly.GetManifestResourceNames();
			for (int i1 = 0; i1 < names.Length; i1++)
			{
				string candidate = string.Format(CultureInfo.InvariantCulture, nameFormat, i1);
				if (names.Contains(candidate, StringComparerEx.InvariantCultureIgnoreCase))
				{
					TProcessingStep step = this.CreateProcessingStep(i1, candidate);
					steps.Add(step);
				}
				else
				{
					break;
				}
			}

			scriptLocator = null;
			if (steps.Count > 0)
			{
				scriptLocator = new AssemblyRessourceScriptLocator(assembly);
			}

			return steps.Count > 0;
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Creates the actual processing step.
		/// </summary>
		/// <param name="sourceVersion"> The source version. </param>
		/// <param name="resourceName"> The corresponding embedded script file name. </param>
		/// <returns>
		///     The created processing step.
		/// </returns>
		protected abstract TProcessingStep CreateProcessingStep (int sourceVersion, string resourceName);

		#endregion
	}
}
