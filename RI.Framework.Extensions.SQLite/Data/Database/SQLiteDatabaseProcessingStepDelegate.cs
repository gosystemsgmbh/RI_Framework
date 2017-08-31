using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Defines a delegate which can be used to define code sub-steps for <see cref="SQLiteDatabaseProcessingStep"/>s.
	/// </summary>
	/// <param name="step">The processing step being executed.</param>
	/// <param name="manager">The used database manager.</param>
	/// <param name="connection">The current connection used to execute the processing step or sub-step respectively.</param>
	/// <param name="transaction">The current transaction used to execute the processing step or sub-step respectively. Can be null, depending on <see cref="SQLiteDatabaseProcessingStep.RequiresTransaction"/>.</param>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public delegate void SQLiteDatabaseProcessingStepDelegate(SQLiteDatabaseProcessingStep step, SQLiteDatabaseManager manager, SQLiteConnection connection, SQLiteTransaction transaction);
}
