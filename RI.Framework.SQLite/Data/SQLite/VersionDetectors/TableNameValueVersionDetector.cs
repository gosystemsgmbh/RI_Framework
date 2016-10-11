using System;
using System.Data;
using System.Data.SQLite;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite.VersionDetectors
{
	/// <summary>
	///     Implements an SQLite file database version detector which uses a specified table and name-value-pairs in that table.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This version detector uses a specified table where it looks up two columns, one for the name and one for the value, and queries the name-value-pair with the specified name from that table.
	///         The value of that name-value-pair is used as the database version.
	///         The value must be compatible to <see cref="int" />.
	///     </para>
	/// </remarks>
	public sealed class TableNameValueVersionDetector : ISQLiteFileVersionDetector
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TableNameValueVersionDetector" />.
		/// </summary>
		/// <param name="table"> The name of the table which contains the name-value-pairs. </param>
		/// <param name="nameColumn"> The name of the column in the table which contains the name (or key) values. </param>
		/// <param name="valueColumn"> The name of the column in the table which contains the actual value. </param>
		/// <param name="name"> The name of the name-value-pair which states the database version. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="table" />, <paramref name="nameColumn" />, <paramref name="valueColumn" />, or <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="table" />, <paramref name="nameColumn" />, <paramref name="valueColumn" />, or <paramref name="name" /> is an empty string. </exception>
		public TableNameValueVersionDetector (string table, string nameColumn, string valueColumn, string name)
		{
			if (table == null)
			{
				throw new ArgumentNullException(nameof(table));
			}

			if (table.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(table));
			}

			if (nameColumn == null)
			{
				throw new ArgumentNullException(nameof(nameColumn));
			}

			if (nameColumn.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(nameColumn));
			}

			if (valueColumn == null)
			{
				throw new ArgumentNullException(nameof(valueColumn));
			}

			if (valueColumn.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(valueColumn));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Table = table;
			this.NameColumn = nameColumn;
			this.ValueColumn = valueColumn;
			this.Name = name;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the name of the name-value-pair which states the database version.
		/// </summary>
		/// <value>
		///     The name of the name-value-pair which states the database version.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the name of the column in the table which contains the name (or key) values.
		/// </summary>
		/// <value>
		///     The name of the column in the table which contains the name (or key) values.
		/// </value>
		public string NameColumn { get; private set; }

		/// <summary>
		///     Gets the name of the table which contains the name-value-pairs.
		/// </summary>
		/// <value>
		///     The name of the table which contains the name-value-pairs.
		/// </value>
		public string Table { get; private set; }

		/// <summary>
		///     Gets the name of the column in the table which contains the actual value.
		/// </summary>
		/// <value>
		///     The name of the column in the table which contains the actual value.
		/// </value>
		public string ValueColumn { get; private set; }

		#endregion




		#region Interface: ISQLiteFileVersionDetector

		/// <inheritdoc />
		public int? DetectVersion (SQLiteConnection temporaryConnection, bool newFile, FilePath file)
		{
			if (temporaryConnection == null)
			{
				throw new ArgumentNullException(nameof(temporaryConnection));
			}

			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (newFile)
			{
				return 0;
			}

			using (SQLiteCommand command = temporaryConnection.CreateCommand())
			{
				command.CommandText = "TYPES [int] SELECT [" + this.ValueColumn + "] FROM [" + this.Table + "] WHERE [" + this.NameColumn + "] == " + this.Name;
				command.CommandType = CommandType.Text;
				object result = command.ExecuteScalar(CommandBehavior.Default);
				if (result is int)
				{
					return (int)result;
				}
				return null;
			}
		}

		#endregion
	}
}
