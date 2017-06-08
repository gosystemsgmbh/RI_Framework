namespace RI.Framework.IO.CSV
{
	/// <summary>
	///     Provides CSV reader settings.
	/// </summary>
	/// <remarks>
	///     <para>
	///         CSV reader settings can be used to customize how CSV data is read and processed.
	///     </para>
	///     <para>
	///         See <see cref="CsvDocument" /> for more general and detailed information about working with CSV data.
	///     </para>
	/// </remarks>
	public sealed class CsvReaderSettings : CsvSettings
	{
		#region Constants

		/// <summary>
		///     The default whether reading is whitespace-tolerant.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public const bool DefaultWhitespaceTolerant = true;

		/// <summary>
		///     The default whether multiline values are allowed.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public const bool DefaultAllowMultilineValues = true;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether reading is whitespace-tolerant.
		/// </summary>
		/// <value>
		///     true if reading is whitespace-tolerant, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="DefaultWhitespaceTolerant" />.
		///     </para>
		/// <para>
		/// Whitespace tolerance allows any whitesoace characters between a closing quote and the following next separator or line-feed.
		/// </para>
		/// </remarks>
		public bool WhitespaceTolerant { get; set; } = CsvReaderSettings.DefaultWhitespaceTolerant;

		/// <summary>
		///     Gets or sets whether multiline values are allowed.
		/// </summary>
		/// <value>
		///     true if multiline values are allowed, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="DefaultAllowMultilineValues" />.
		///     </para>
		/// </remarks>
		public bool AllowMultilineValues { get; set; } = CsvReaderSettings.DefaultAllowMultilineValues;

		#endregion
	}
}
