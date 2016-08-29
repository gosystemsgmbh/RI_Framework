namespace RI.Framework.IO.INI.Elements
{
	/// <summary>
	///     Represents arbitrary text in INI data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IniDocument" /> for more general and detailed information about working with INI data.
	///     </para>
	///     <para>
	///         Arbitrary text is everything in an INI file which is not a section header, comment, or name-value-pair.
	///     </para>
	/// </remarks>
	public sealed class TextIniElement : IniElement
	{
		#region Instance Constructor/Destructor

		public TextIniElement (string text)
		{
			this.Text = text;
		}

		#endregion




		#region Instance Fields

		private string _text;

		#endregion




		#region Instance Properties/Indexer

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value ?? string.Empty;
			}
		}

		#endregion
	}
}
