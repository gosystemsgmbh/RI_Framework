﻿using System;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.IO.INI;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings.Storages
{
	/// <summary>
	///     Implements a setting storage which reads/writes from/to a specified INI file.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This setting store internally uses <see cref="IniDocument" /> to process the INI file.
	///         See <see cref="IniDocument" /> for more details about INI files.
	///     </para>
	///     <para>
	///         See <see cref="ISettingStorage" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class IniFileSettingStorage : ISettingStorage, ILogSource
	{
		#region Constants

		/// <summary>
		///     The default text encoding which is used for INI files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default text encoding is UTF-8.
		///     </para>
		/// </remarks>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		/// <summary>
		///     The default INI section name for reading/writing values.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default file name is null, which searches and places all values outside any section.
		///     </para>
		/// </remarks>
		public static readonly string DefaultSectionName = null;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="IniFileSettingStorage" />.
		/// </summary>
		/// <param name="filePath"> The path to the INI file. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> contains wildcards. </exception>
		/// <remarks>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding for the INI file.
		///     </para>
		///     <para>
		///         The default section name <see cref="DefaultSectionName" /> is used as the INI section name.
		///     </para>
		/// </remarks>
		public IniFileSettingStorage (FilePath filePath)
			: this(filePath, null, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="IniFileSettingStorage" />.
		/// </summary>
		/// <param name="filePath"> The path to the INI file. </param>
		/// <param name="fileEncoding"> The text encoding of the INI file (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <param name="sectionName"> The INI section name where all the values are read/written from/to (can be null to use <see cref="DefaultSectionName" />). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> contains wildcards. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public IniFileSettingStorage (FilePath filePath, Encoding fileEncoding, string sectionName)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException(nameof(filePath));
			}

			if (filePath.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(filePath));
			}

			if (sectionName != null)
			{
				if (sectionName.IsEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			this.FilePath = filePath;
			this.FileEncoding = fileEncoding ?? IniFileSettingStorage.DefaultEncoding;
			this.SectionName = sectionName ?? IniFileSettingStorage.DefaultSectionName;

			this.Document = new IniDocument(StringComparerEx.InvariantCultureIgnoreCase);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the text encoding of the used INI file.
		/// </summary>
		/// <value>
		///     The text encoding of the used INI file.
		/// </value>
		public Encoding FileEncoding { get; private set; }

		/// <summary>
		///     Gets the path to the used INI file.
		/// </summary>
		/// <value>
		///     The path to the used INI file.
		/// </value>
		public FilePath FilePath { get; private set; }

		/// <summary>
		///     Gets the INI section name where all the values are read/written from/to in the INI file.
		/// </summary>
		/// <value>
		///     The INI section name where all the values are read/written from/to in the INI file.
		/// </value>
		public string SectionName { get; private set; }

		private IniDocument Document { get; set; }

		#endregion




		#region Interface: ISettingStorage

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public string GetValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return this.Document.GetValue(this.SectionName, name);
		}

		/// <inheritdoc />
		public bool HasValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return this.Document.GetValue(this.SectionName, name) != null;
		}

		/// <inheritdoc />
		public void Load ()
		{
			if (!this.FilePath.Exists)
			{
				this.Log(LogLevel.Debug, "Creating INI settings file: {0}", this.FilePath);
			}

			this.FilePath.Directory.Create();
			this.FilePath.CreateIfNotExist();

			this.Log(LogLevel.Debug, "Loading INI settings file: {0}", this.FilePath);

			this.Document.Clear();
			this.Document.Load(this.FilePath, this.FileEncoding);
		}

		/// <inheritdoc />
		public void Save ()
		{
			if (!this.FilePath.Exists)
			{
				this.Log(LogLevel.Debug, "Creating INI settings file: {0}", this.FilePath);
			}

			this.FilePath.Directory.Create();

			this.Log(LogLevel.Debug, "Saving INI settings file: {0}", this.FilePath);

			this.Document.SortElements(this.SectionName);
			this.Document.Save(this.FilePath, this.FileEncoding);
		}

		/// <inheritdoc />
		public void SetValue (string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (value == null)
			{
				this.Document.DeleteValue(this.SectionName, name);
			}
			else
			{
				this.Document.SetValue(this.SectionName, name, value);
			}
		}

		#endregion
	}
}
