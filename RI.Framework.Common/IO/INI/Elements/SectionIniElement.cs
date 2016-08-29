using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.IO.INI.Elements
{
	/// <summary>
	///     Represents a section header in INI data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IniDocument" /> for more general and detailed information about working with INI data.
	///     </para>
	/// </remarks>
	public sealed class SectionIniElement : IniElement
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SectionIniElement" />.
		/// </summary>
		/// <param name="sectionName"> The section name. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="sectionName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public SectionIniElement (string sectionName)
		{
			if (sectionName == null)
			{
				throw new ArgumentNullException(nameof(sectionName));
			}

			if (sectionName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(sectionName));
			}

			this.SectionName = sectionName;
		}

		#endregion




		#region Instance Fields

		private string _sectionName;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the section name.
		/// </summary>
		/// <value>
		///     The section name.
		/// </value>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="value" /> is an empty string. </exception>
		public string SectionName
		{
			get
			{
				return this._sectionName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(value));
				}

				this._sectionName = value;
			}
		}

		#endregion
	}
}
