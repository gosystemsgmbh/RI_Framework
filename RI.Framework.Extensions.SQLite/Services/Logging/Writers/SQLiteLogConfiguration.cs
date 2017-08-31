using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Readers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	/// Holds table, index, and column configurations for SQLite log database schemas.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteLogConfiguration : ICloneable<SQLiteLogConfiguration>, ICloneable
	{
		/// <summary>
		/// Gets or sets the table name.
		/// </summary>
		/// <value>
		/// The table name.
		/// </value>
		public string TableName { get; set; } = "Log";

		/// <summary>
		/// Gets or sets the column name for the file column.
		/// </summary>
		/// <value>
		/// The column name for the file column.
		/// </value>
		public string ColumnNameFile { get; set; } = "File";
		/// <summary>
		/// Gets or sets the column name for the timestamp column.
		/// </summary>
		/// <value>
		/// The column name for the timestamp column.
		/// </value>
		public string ColumnNameTimestamp { get; set; } = "Timestamp";
		/// <summary>
		/// Gets or sets the column name for the thread ID column.
		/// </summary>
		/// <value>
		/// The column name for the thread ID column.
		/// </value>
		public string ColumnNameThreadId { get; set; } = "ThreadId";
		/// <summary>
		/// Gets or sets the column name for the severity column.
		/// </summary>
		/// <value>
		/// The column name for the severity column.
		/// </value>
		public string ColumnNameSeverity { get; set; } = "Severity";
		/// <summary>
		/// Gets or sets the column name for the source column.
		/// </summary>
		/// <value>
		/// The column name for the source column.
		/// </value>
		public string ColumnNameSource { get; set; } = "Source";
		/// <summary>
		/// Gets or sets the column name for the message column.
		/// </summary>
		/// <value>
		/// The column name for the message column.
		/// </value>
		public string ColumnNameMessage { get; set; } = "Message";

		/// <summary>
		/// Gets or sets additional constraints for the file column.
		/// </summary>
		/// <value>
		/// The additional constraints for the file column.
		/// </value>
		public string ColumnConstraintFile { get; set; } = null;
		/// <summary>
		/// Gets or sets additional constraints for the timestamp column.
		/// </summary>
		/// <value>
		/// The additional constraints for the timestamp column.
		/// </value>
		public string ColumnConstraintTimestamp { get; set; } = null;
		/// <summary>
		/// Gets or sets additional constraints for the thread ID column.
		/// </summary>
		/// <value>
		/// The additional constraints for the thread ID column.
		/// </value>
		public string ColumnConstraintThreadId { get; set; } = null;
		/// <summary>
		/// Gets or sets additional constraints for the severity column.
		/// </summary>
		/// <value>
		/// The additional constraints for the severity column.
		/// </value>
		public string ColumnConstraintSeverity { get; set; } = null;
		/// <summary>
		/// Gets or sets additional constraints for the source column.
		/// </summary>
		/// <value>
		/// The additional constraints for the source column.
		/// </value>
		public string ColumnConstraintSource { get; set; } = null;
		/// <summary>
		/// Gets or sets additional constraints for the message column.
		/// </summary>
		/// <value>
		/// The additional constraints for the message column.
		/// </value>
		public string ColumnConstraintMessage { get; set; } = null;

		/// <summary>
		/// Gets or sets the data type for the file column.
		/// </summary>
		/// <value>
		/// The data type for the file column.
		/// </value>
		public string ColumnTypeFile { get; set; } = "TEXT";
		/// <summary>
		/// Gets or sets the data type for the timestamp column.
		/// </summary>
		/// <value>
		/// The data type for the timestamp column.
		/// </value>
		public string ColumnTypeTimestamp { get; set; } = "DATETIME";
		/// <summary>
		/// Gets or sets the data type for the thread ID column.
		/// </summary>
		/// <value>
		/// The data type for the thread ID column.
		/// </value>
		public string ColumnTypeThreadId { get; set; } = "INTEGER";
		/// <summary>
		/// Gets or sets the data type for the severity column.
		/// </summary>
		/// <value>
		/// The data type for the severity column.
		/// </value>
		public string ColumnTypeSeverity { get; set; } = "INTEGER";
		/// <summary>
		/// Gets or sets the data type for the source column.
		/// </summary>
		/// <value>
		/// The data type for the source column.
		/// </value>
		public string ColumnTypeSource { get; set; } = "TEXT";
		/// <summary>
		/// Gets or sets the data type for the message column.
		/// </summary>
		/// <value>
		/// The data type for the message column.
		/// </value>
		public string ColumnTypeMessage { get; set; } = "TEXT";

		/// <summary>
		/// Gets or sets the nullability for the file column.
		/// </summary>
		/// <value>
		/// The nullability for the file column.
		/// </value>
		public bool ColumnNullableFile { get; set; } = true;
		/// <summary>
		/// Gets or sets the nullability for the timestamp column.
		/// </summary>
		/// <value>
		/// The nullability for the timestamp column.
		/// </value>
		public bool ColumnNullableTimestamp { get; set; } = false;
		/// <summary>
		/// Gets or sets the nullability for the thread ID column.
		/// </summary>
		/// <value>
		/// The nullability for the thread ID column.
		/// </value>
		public bool ColumnNullableThreadId { get; set; } = false;
		/// <summary>
		/// Gets or sets the nullability for the severity column.
		/// </summary>
		/// <value>
		/// The nullability for the severity column.
		/// </value>
		public bool ColumnNullableSeverity { get; set; } = false;
		/// <summary>
		/// Gets or sets the nullability for the source column.
		/// </summary>
		/// <value>
		/// The nullability for the source column.
		/// </value>
		public bool ColumnNullableSource { get; set; } = false;
		/// <summary>
		/// Gets or sets the nullability for the message column.
		/// </summary>
		/// <value>
		/// The nullability for the message column.
		/// </value>
		public bool ColumnNullableMessage { get; set; } = false;

		/// <summary>
		/// Gets or sets the name for the file index.
		/// </summary>
		/// <value>
		/// The name for the file index.
		/// </value>
		public string IndexNameFile { get; set; } = "Log_File";
		/// <summary>
		/// Gets or sets the name for the timestamp index.
		/// </summary>
		/// <value>
		/// The name for the timestamp index.
		/// </value>
		public string IndexNameTimestamp { get; set; } = "Log_Timestamp";
		/// <summary>
		/// Gets or sets the name for the thread ID index.
		/// </summary>
		/// <value>
		/// The name for the thread ID index.
		/// </value>
		public string IndexNameThreadId { get; set; } = "Log_ThreadId";
		/// <summary>
		/// Gets or sets the name for the severity index.
		/// </summary>
		/// <value>
		/// The name for the severity index.
		/// </value>
		public string IndexNameSeverity { get; set; } = "Log_Severity";
		/// <summary>
		/// Gets or sets the name for the source index.
		/// </summary>
		/// <value>
		/// The name for the source index.
		/// </value>
		public string IndexNameSource { get; set; } = "Log_Source";
		/// <summary>
		/// Gets or sets the name for the message index.
		/// </summary>
		/// <value>
		/// The name for the message index.
		/// </value>
		public string IndexNameMessage { get; set; } = "Log_Message";

		/// <summary>
		/// Builds the SQLite command string which can be used to create the log table, based on the current configuration.
		/// </summary>
		/// <returns>
		/// The SQLite command string used to build the log table.
		/// The return value is never null or an empty string.
		/// </returns>
		/// <exception cref="InvalidOperationException">There are no columns configured.</exception>
		public string BuildCreateTableCommand()
		{
			SQLiteLogConfiguration configuration = this.GetPreprocessedConfiguration();

			List<string> columns = new List<string>();
			Action<string, string, bool, string> addColumn = (name, type, nullable, constraint) =>
			{
				if (name.IsNullOrEmptyOrWhitespace())
				{
					return;
				}

				columns.Add("[" + name + "] " + type + (nullable ? " NULL" : " NOT NULL") + (constraint.IsNullOrEmptyOrWhitespace() ? string.Empty : (" " + constraint)) );
			};

			addColumn(configuration.ColumnNameFile, configuration.ColumnTypeFile, configuration.ColumnNullableFile, configuration.ColumnConstraintFile);
			addColumn(configuration.ColumnNameTimestamp, configuration.ColumnTypeTimestamp, configuration.ColumnNullableTimestamp, configuration.ColumnConstraintTimestamp);
			addColumn(configuration.ColumnNameThreadId, configuration.ColumnTypeThreadId, configuration.ColumnNullableThreadId, configuration.ColumnConstraintThreadId);
			addColumn(configuration.ColumnNameSeverity, configuration.ColumnTypeSeverity, configuration.ColumnNullableSeverity, configuration.ColumnConstraintSeverity);
			addColumn(configuration.ColumnNameSource, configuration.ColumnTypeSource, configuration.ColumnNullableSource, configuration.ColumnConstraintSource);
			addColumn(configuration.ColumnNameMessage, configuration.ColumnTypeMessage, configuration.ColumnNullableMessage, configuration.ColumnConstraintMessage);

			if (columns.Count == 0)
			{
				throw new InvalidOperationException("No columns configured.");
			}

			StringBuilder sb = new StringBuilder();

			sb.Append("CREATE TABLE IF NOT EXISTS [" + configuration.TableName + "]");
			sb.Append(" (");
			sb.Append(columns.Join(", "));
			sb.Append(");");

			return sb.ToString();
		}

		/// <summary>
		/// Builds the SQLite command string which can be used to create the log indices, based on the current configuration.
		/// </summary>
		/// <returns>
		/// The SQLite command string used to build the log indices or null if no indices are configured.
		/// The return value is never an empty string.
		/// </returns>
		public string BuildCreateIndexCommand()
		{
			SQLiteLogConfiguration configuration = this.GetPreprocessedConfiguration();

			List<string> indices = new List<string>();
			Action<string, string, string> addIndex = (name, table, column) =>
			{
				if (name.IsNullOrEmptyOrWhitespace())
				{
					return;
				}

				if (column.IsNullOrEmptyOrWhitespace())
				{
					return;
				}

				indices.Add("CREATE INDEX IF NOT EXISTS [" + name + "] ON [" + table + "] ([" + column + "])");
			};

			addIndex(configuration.IndexNameFile, configuration.TableName, configuration.ColumnNameFile);
			addIndex(configuration.IndexNameTimestamp, configuration.TableName, configuration.ColumnNameTimestamp);
			addIndex(configuration.IndexNameThreadId, configuration.TableName, configuration.ColumnNameThreadId);
			addIndex(configuration.IndexNameSeverity, configuration.TableName, configuration.ColumnNameSeverity);
			addIndex(configuration.IndexNameSource, configuration.TableName, configuration.ColumnNameSource);
			addIndex(configuration.IndexNameMessage, configuration.TableName, configuration.ColumnNameMessage);

			if (indices.Count == 0)
			{
				return null;
			}

			StringBuilder sb = new StringBuilder();

			sb.Append(indices.Join("; "));

			return sb.ToString();
		}

		/// <summary>
		/// Builds the SQLite command string which can be used to insert a single log entry.
		/// </summary>
		/// <returns>
		/// The SQLite command string used to insert a single log entry.
		/// The return value is never null or an empty string.
		/// </returns>
		/// <exception cref="InvalidOperationException">There are no columns configured.</exception>
		public string BuildInsertEntryCommand()
		{
			SQLiteLogConfiguration configuration = this.GetPreprocessedConfiguration();

			List<string> columns = new List<string>();
			List<string> values = new List<string>();
			Action<string> addEntry = column =>
			{
				if (column.IsNullOrEmptyOrWhitespace())
				{
					return;
				}

				columns.Add("[" + column + "]");
				values.Add("@" + column.ToLowerInvariant());
			};

			addEntry(configuration.ColumnNameFile);
			addEntry(configuration.ColumnNameTimestamp);
			addEntry(configuration.ColumnNameThreadId);
			addEntry(configuration.ColumnNameSeverity);
			addEntry(configuration.ColumnNameSource);
			addEntry(configuration.ColumnNameMessage);

			if (columns.Count == 0)
			{
				throw new InvalidOperationException("No columns configured.");
			}

			StringBuilder sb = new StringBuilder();

			sb.Append("INSERT INTO [" + configuration.TableName + "]");
			sb.Append(" (");
			sb.Append(columns.Join(", "));
			sb.Append(") VALUES (");
			sb.Append(values.Join(", "));
			sb.Append(");");

			return sb.ToString();
		}

		/// <summary>
		/// Builds a dictionary for SQLite command parameters based on a log entry.
		/// </summary>
		/// <returns>
		/// The dictionary for SQLite command parameters based on a log entry.
		/// The return value is never null or an empty dictionary.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="entry"/> is null.</exception>
		/// <exception cref="InvalidOperationException">There are no columns configured.</exception>
		public Dictionary<string, object> BuildInsertEntryParameters(FilePath file, LogFileEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException(nameof(entry));
			}

			SQLiteLogConfiguration configuration = this.GetPreprocessedConfiguration();

			Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparerEx.TrimmedInvariantCultureIgnoreCase);
			Action<string, object> addParameter = (column, value) =>
			{
				if (column.IsNullOrEmptyOrWhitespace())
				{
					return;
				}

				parameters.Add("@" + column.ToLowerInvariant(), value);
			};

			if (parameters.Count == 0)
			{
				throw new InvalidOperationException("No columns configured.");
			}

			addParameter(configuration.ColumnNameFile, file?.PathOriginal);
			addParameter(configuration.ColumnNameTimestamp, entry.Timestamp);
			addParameter(configuration.ColumnNameThreadId, entry.ThreadId);
			addParameter(configuration.ColumnNameSeverity, entry.Severity);
			addParameter(configuration.ColumnNameSource, entry.Source);
			addParameter(configuration.ColumnNameMessage, entry.Message);

			return parameters;
		}

		/// <summary>
		/// Builds the SQLite command which can be used to cleanup old log entries.
		/// </summary>
		/// <param name="retentionDate">The date and time where all previous log entries are deleted.</param>
		/// <param name="connection">The connection to use.</param>
		/// <param name="transaction">The transaction to use (can be null.</param>
		/// <returns>
		/// The SQLite command used to cleanup old log entries.
		/// The return value is never null.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="connection"/> is null.</exception>
		public SQLiteCommand BuildCleanupCommand (DateTime retentionDate, SQLiteConnection connection, SQLiteTransaction transaction)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			SQLiteLogConfiguration configuration = this.GetPreprocessedConfiguration();

			string commandString = "DELETE FROM [" + configuration.TableName + "] WHERE [" + configuration.ColumnNameTimestamp + "] < @retentionDate;";
			SQLiteCommand command = transaction == null ? new SQLiteCommand(commandString, connection) : new SQLiteCommand(commandString, connection, transaction);
			command.Parameters.AddWithValue("@retentionDate", retentionDate);

			return command;
		}

		/// <summary>
		/// Creates a default SQLite connection for writing log entries to a SQLite database file.
		/// </summary>
		/// <param name="dbFile">The SQLite database file.</param>
		/// <returns>
		/// The opened SQLite connection to the database file.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dbFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="dbFile" /> contains wildcards. </exception>
		public SQLiteConnection CreateConnection(FilePath dbFile)
		{
			if (dbFile == null)
			{
				throw new ArgumentNullException(nameof(dbFile));
			}

			if (dbFile.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(dbFile), "Wildcards are not allowed.");
			}

			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
			builder.DataSource = dbFile.PathResolved;
			builder.ForeignKeys = true;
			builder.FailIfMissing = false;
			builder.ReadOnly = false;

			SQLiteConnection connection = new SQLiteConnection(builder.ToString());

			connection.Open();

			return connection;
		}

		private SQLiteLogConfiguration GetPreprocessedConfiguration ()
		{
			SQLiteLogConfiguration configuration = new SQLiteLogConfiguration();

			configuration.TableName = this.NormalizeName(this.TableName);

			configuration.ColumnNameFile = this.NormalizeName(this.ColumnNameFile);
			configuration.ColumnNameTimestamp = this.NormalizeName(this.ColumnNameTimestamp);
			configuration.ColumnNameThreadId = this.NormalizeName(this.ColumnNameThreadId);
			configuration.ColumnNameSeverity = this.NormalizeName(this.ColumnNameSeverity);
			configuration.ColumnNameSource = this.NormalizeName(this.ColumnNameSource);
			configuration.ColumnNameMessage = this.NormalizeName(this.ColumnNameMessage);

			configuration.IndexNameFile = this.NormalizeName(this.IndexNameFile);
			configuration.IndexNameTimestamp = this.NormalizeName(this.IndexNameTimestamp);
			configuration.IndexNameThreadId = this.NormalizeName(this.IndexNameThreadId);
			configuration.IndexNameSeverity = this.NormalizeName(this.IndexNameSeverity);
			configuration.IndexNameSource = this.NormalizeName(this.IndexNameSource);
			configuration.IndexNameMessage = this.NormalizeName(this.IndexNameMessage);

			return configuration;
		}

		private string NormalizeName (string name)
		{
			if (name.IsNullOrEmptyOrWhitespace())
			{
				return null;
			}

			return name.TrimStart('[').TrimEnd(']');
		}

		/// <inheritdoc />
		public SQLiteLogConfiguration Clone ()
		{
			SQLiteLogConfiguration clone = new SQLiteLogConfiguration();

			clone.TableName = this.TableName;

			clone.ColumnNameFile = this.ColumnNameFile;
			clone.ColumnNameTimestamp = this.ColumnNameTimestamp;
			clone.ColumnNameThreadId = this.ColumnNameThreadId;
			clone.ColumnNameSeverity = this.ColumnNameSeverity;
			clone.ColumnNameSource = this.ColumnNameSource;
			clone.ColumnNameMessage = this.ColumnNameMessage;

			clone.ColumnConstraintFile = this.ColumnConstraintFile;
			clone.ColumnConstraintTimestamp = this.ColumnConstraintTimestamp;
			clone.ColumnConstraintThreadId = this.ColumnConstraintThreadId;
			clone.ColumnConstraintSeverity = this.ColumnConstraintSeverity;
			clone.ColumnConstraintSource = this.ColumnConstraintSource;
			clone.ColumnConstraintMessage = this.ColumnConstraintMessage;

			clone.ColumnTypeFile = this.ColumnTypeFile;
			clone.ColumnTypeTimestamp = this.ColumnTypeTimestamp;
			clone.ColumnTypeThreadId = this.ColumnTypeThreadId;
			clone.ColumnTypeSeverity = this.ColumnTypeSeverity;
			clone.ColumnTypeSource = this.ColumnTypeSource;
			clone.ColumnTypeMessage = this.ColumnTypeMessage;

			clone.ColumnNullableFile = this.ColumnNullableFile;
			clone.ColumnNullableTimestamp = this.ColumnNullableTimestamp;
			clone.ColumnNullableThreadId = this.ColumnNullableThreadId;
			clone.ColumnNullableSeverity = this.ColumnNullableSeverity;
			clone.ColumnNullableSource = this.ColumnNullableSource;
			clone.ColumnNullableMessage = this.ColumnNullableMessage;

			clone.IndexNameFile = this.IndexNameFile;
			clone.IndexNameTimestamp = this.IndexNameTimestamp;
			clone.IndexNameThreadId = this.IndexNameThreadId;
			clone.IndexNameSeverity = this.IndexNameSeverity;
			clone.IndexNameSource = this.IndexNameSource;
			clone.IndexNameMessage = this.IndexNameMessage;

			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}
	}
}
