using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;

using RI.Framework.Collections;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Implements an SQLite file migration chain.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An SQLite file migration chain is used to organize multiple <see cref="ISQLiteFileMigrationStep" />s to form a valid migration chain from any version of the database to the supported maximum version.
	///     </para>
	///     <para>
	///         An SQLite file migration chain is used with <see cref="SQLiteFile" />.<see cref="SQLiteFile.Migrate" />.
	///         See <see cref="SQLiteFile.Migrate" /> for more details.
	///     </para>
	///     <para>
	///         See also <see cref="IsValid" /> for details about how a migration chain can be valid/invalid.
	///     </para>
	///     <para>
	///         This collection is always kept sorted, executed on each insert or replacement of an element.
	///     </para>
	/// </remarks>
	/// TODO: Provide logging
	public sealed class SQLiteFileMigrationChain : Collection<ISQLiteFileMigrationStep>
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether this migration chain is valid.
		/// </summary>
		/// <value>
		///     true if the migration chain is valid, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         A migration chain is valid if it contains at least one element and if all elements, when sorted, have successive source versions (<see cref="ISQLiteFileMigrationStep.SourceVersion" />).
		///     </para>
		/// </remarks>
		public bool IsValid
		{
			get
			{
				if (this.Count == 0)
				{
					return false;
				}

				for (int i1 = 0; i1 < this.Count; i1++)
				{
					if (this[i1].SourceVersion != (this.MinSourceVersion + i1))
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		///     Gets the highest target version supported by this migration chain.
		/// </summary>
		/// <value>
		///     The highest target version supported by this migration chain or null if this migration chain contains no elements.
		/// </value>
		public int? MaxTargetVersion => this.Count == 0 ? (int?)null : this[this.Count - 1].SourceVersion + 1;

		/// <summary>
		///     Gets the lowest source version supported by this migration chain.
		/// </summary>
		/// <value>
		///     The lowest source version supported by this migration chain or null if this migration chain contains no elements.
		/// </value>
		public int? MinSourceVersion => this.Count == 0 ? (int?)null : this[0].SourceVersion;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Executes this migration chain on a given temporary database connection.
		/// </summary>
		/// <param name="temporaryConnection"> The temporary open connection to the database. </param>
		/// <returns>
		///     true if all migration steps executed successfully, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="temporaryConnection" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> This migration chain is invalid. </exception>
		internal bool Execute (SQLiteConnection temporaryConnection)
		{
			if (temporaryConnection == null)
			{
				throw new ArgumentNullException(nameof(temporaryConnection));
			}

			if (!this.IsValid)
			{
				throw new InvalidOperationException("The SQLite file migration chain is invalid.");
			}

			foreach (ISQLiteFileMigrationStep step in this)
			{
				if (!step.ExecuteStep(temporaryConnection))
				{
					return false;
				}
			}

			return true;
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void InsertItem (int index, ISQLiteFileMigrationStep item)
		{
			base.InsertItem(index, item);
			this.Sort(true);
		}

		/// <inheritdoc />
		protected override void SetItem (int index, ISQLiteFileMigrationStep item)
		{
			base.SetItem(index, item);
			this.Sort(true);
		}

		#endregion
	}
}
