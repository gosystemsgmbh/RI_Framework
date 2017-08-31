using System;
using System.Diagnostics.CodeAnalysis;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Describes the transaction requirements of a processing sub-step.
	/// </summary>
	[Serializable]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum SQLiteDatabaseProcessingStepTransactionRequirement
	{
		/// <summary>
		/// The sub-step has no transaction requirement and can be used with or without a transaction.
		/// </summary>
		DontCare = 0,

		/// <summary>
		/// The sub-step requires a transaction.
		/// </summary>
		Required = 1,

		/// <summary>
		/// The sub-step cannot be used with a transaction.
		/// </summary>
		Disallowed = 2,
	}
}
