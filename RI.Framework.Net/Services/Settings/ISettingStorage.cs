using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Defines the interface for a setting storage used by a setting service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A setting storage is used by a <see cref="ISettingService" /> to actually persistently store the values as their string representations.
	///         Values are stored as name/value pairs.
	///     </para>
	/// </remarks>
	[Export]
	public interface ISettingStorage
	{
		/// <summary>
		///     Deletes a value.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		void DeleteValue (string name);

		/// <summary>
		///     Reads a value.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     The value or null if the value is not available.
		/// </returns>
		string GetValue (string name);

		/// <summary>
		///     Checks whether a value is available.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     true if the value is available, false otherwise.
		/// </returns>
		bool HasValue (string name);

		/// <summary>
		///     Reads, if necessary and applicable, all values from the storage.
		/// </summary>
		void Load ();

		/// <summary>
		///     Writes, if necessary and applicable, all values to the storage, making them persistent.
		/// </summary>
		void Save ();

		/// <summary>
		///     Creates or updates a value.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <param name="value"> The actual value. </param>
		void SetValue (string name, string value);
	}
}
