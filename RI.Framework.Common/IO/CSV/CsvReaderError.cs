using System;




namespace RI.Framework.IO.CSV
{
	/// <summary>
	///     Describes an error which ocurred during reading CSV data using an <see cref="CsvReader" />.
	/// </summary>
	[Serializable]
	public enum CsvReaderError
	{
		/// <summary>
		///     No error (no line read or the last line which was read is valid).
		/// </summary>
		None = 0,
	}
}
