namespace RI.Framework.IO.INI.Elements
{
	/// <summary>
	///     Represents a comment in INI data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IniDocument" /> for more general and detailed information about working with INI data.
	///     </para>
	/// </remarks>
	public sealed class CommentIniElement : IniElement
	{
		#region Instance Constructor/Destructor

		public CommentIniElement (string comment)
		{
			this.Comment = comment;
		}

		#endregion




		#region Instance Fields

		private string _comment;

		#endregion




		#region Instance Properties/Indexer

		public string Comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				this._comment = value ?? string.Empty;
			}
		}

		#endregion
	}
}
