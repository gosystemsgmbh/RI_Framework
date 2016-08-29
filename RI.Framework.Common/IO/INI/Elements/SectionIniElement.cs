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
