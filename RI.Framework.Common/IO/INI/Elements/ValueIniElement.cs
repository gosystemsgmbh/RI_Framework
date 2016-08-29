using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.IO.INI.Elements
{
	/// <summary>
	///     Represents a name-value-pair in INI data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IniDocument" /> for more general and detailed information about working with INI data.
	///     </para>
	/// </remarks>
	public sealed class ValueIniElement : IniElement
	{
		#region Instance Constructor/Destructor

		public ValueIniElement (string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Name = name;
			this.Value = value;
		}

		#endregion




		#region Instance Fields

		private string _name;

		private string _value;

		#endregion




		#region Instance Properties/Indexer

		public string Name
		{
			get
			{
				return this._name;
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

				this._name = value;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value ?? string.Empty;
			}
		}

		#endregion
	}
}
