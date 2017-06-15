using System;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Database.Scripts
{
	/// <summary>
	///     Defines the interface for a database script locator.
	/// </summary>
	public interface IDatabaseScriptLocator
	{
		/// <summary>
		///     Gets the script of a specified name.
		/// </summary>
		/// <param name="name"> The name of the script. </param>
		/// <returns>
		///     The script batch or null if the script of the specified name was not found.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		string[] GetScript (string name);
	}
}
