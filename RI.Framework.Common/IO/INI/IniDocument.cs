using System;
using System.Collections.Generic;
using System.IO;
using System.Text;




namespace RI.Framework.IO.INI
{
	public sealed class IniDocument
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="IniDocument" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         After creation, the INI document does not contain any elements.
		///     </para>
		/// </remarks>
		public IniDocument ()
		{
			this.Elements = new List<IniElement>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the list with all INI elements of this INI document.
		/// </summary>
		public IList<IniElement> Elements { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Loads INI elements from an existing INI reader.
		/// </summary>
		/// <param name="reader"> The INI reader from which the elements are loaded. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new elements are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="reader" /> is null. </exception>
		public void Load (IniReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			this.Elements.Clear();

			while (reader.ReadNext())
			{
				this.Elements.Add(reader.CurrentElement);
			}
		}

		/// <summary>
		///     Loads INI elements from an existing INI file.
		/// </summary>
		/// <param name="file"> The path of the INI file to load. </param>
		/// <param name="encoding"> The encoding for reading the INI file. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new elements are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		public void Load (string file, Encoding encoding)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			this.Load(file, encoding, null);
		}

		/// <summary>
		///     Loads INI elements from an existing INI file.
		/// </summary>
		/// <param name="file"> The path of the INI file to load. </param>
		/// <param name="encoding"> The encoding for reading the INI file. </param>
		/// <param name="settings"> The used INI reader settings or null if default values should be used. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new elements are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		public void Load (string file, Encoding encoding, IniReaderSettings settings)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			using (StreamReader sr = new StreamReader(file, encoding))
			{
				using (IniReader ir = new IniReader(sr, settings))
				{
					this.Load(ir);
				}
			}
		}

		/// <summary>
		///     Saves all INI elements of this INI document to an existing INI writer.
		/// </summary>
		/// <param name="writer"> The INI writer to which the elements are saved. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="writer" /> is null. </exception>
		public void Save (IniWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			foreach (IniElement element in this.Elements)
			{
				writer.WriteElement(element);
			}
		}

		/// <summary>
		///     Saves all INI elements of this INI document to an INI file.
		/// </summary>
		/// <param name="file"> The path of the INI file to save. </param>
		/// <param name="encoding"> The encoding for writing the INI file. </param>
		/// <remarks>
		///     <para>
		///         The INI file will be overwritten with the INI elements from this INI document.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		public void Save (string file, Encoding encoding)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			this.Save(file, encoding, null);
		}

		/// <summary>
		///     Saves all INI elements of this INI document to an INI file.
		/// </summary>
		/// <param name="file"> The path of the INI file to save. </param>
		/// <param name="encoding"> The encoding for writing the INI file. </param>
		/// <param name="settings"> The used INI writer settings or null if default values should be used. </param>
		/// <remarks>
		///     <para>
		///         The INI file will be overwritten with the INI elements from this INI document.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		public void Save (string file, Encoding encoding, IniWriterSettings settings)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			using (StreamWriter sw = new StreamWriter(file, false, encoding))
			{
				using (IniWriter iw = new IniWriter(sw, settings))
				{
					this.Save(iw);
				}
			}
		}

		#endregion
	}
}
