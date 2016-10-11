using System;
using System.Data;
using System.Data.SQLite;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite.MigrationSteps
{
	/// <summary>
	///     Implements a simple migration step which executes SQL.
	/// </summary>
	public sealed class CommandMigrationStep : ISQLiteFileMigrationStep
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CommandMigrationStep" />.
		/// </summary>
		/// <param name="command"> The SQL command to execute when this migration step is executed. </param>
		/// <param name="sourceVersion"> The source version supported by this migration step. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="sourceVersion" /> is less than zero. </exception>
		public CommandMigrationStep (string command, int sourceVersion)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			if (command.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(command));
			}

			if (sourceVersion < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sourceVersion));
			}

			this.Command = command;
			this.SourceVersion = sourceVersion;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the SQL command to execute when this migration step is executed.
		/// </summary>
		/// <value>
		///     The SQL command to execute when this migration step is executed.
		/// </value>
		public string Command { get; private set; }

		#endregion




		#region Interface: ISQLiteFileMigrationStep

		/// <inheritdoc />
		public int SourceVersion { get; private set; }

		/// <inheritdoc />
		public bool ExecuteStep (SQLiteConnection temporaryConnection)
		{
			if (temporaryConnection == null)
			{
				throw new ArgumentNullException(nameof(temporaryConnection));
			}

			bool result = false;
			using (SQLiteCommand command = temporaryConnection.CreateCommand())
			{
				command.CommandText = this.Command;
				command.CommandType = CommandType.Text;
				result = command.ExecuteNonQuery() > 0;
			}

			return result;
		}

		#endregion
	}
}
