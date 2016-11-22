using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RI.Framework.Collections;
using RI.Framework.Collections.Linq;
using RI.Framework.IO.INI.Elements;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Comparison;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.IO.INI
{
	/// <summary>
	///     Contains and manages structured INI data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <b> GENERAL </b>
	///     </para>
	///     <para>
	///         INI data in an <see cref="IniDocument" /> is stored in the <see cref="Elements" /> property.
	///         <see cref="Elements" /> is always kept up to date and all actions performed on an <see cref="IniDocument" /> directly read from or modify <see cref="Elements" />.
	///     </para>
	///     <para>
	///         <see cref="Elements" /> is a list which contains all the INI elements of the INI data in a sequential order (e.g. as they would appear in an INI file).
	///         The INI elements are all of the abstract base type <see cref="IniElement" />, the concrete type depending on the type of the element (<see cref="SectionIniElement" />, <see cref="ValueIniElement" />, <see cref="CommentIniElement" />, <see cref="TextIniElement" />).
	///     </para>
	///     <para>
	///         <see cref="Elements" /> can be modified arbitrarily by either using methods of <see cref="IniDocument" /> or by modifying the list itself.
	///         The list can contain or be modified to contain any sequence of the four types of INI elements mentioned above.
	///         Any sequence of INI elements will be valid as each instance of a derivate of <see cref="IniElement" /> is independent to any other type of INI element.
	///     </para>
	///     <note type="important">
	///         Be aware that although each INI element is independent of each other, the sequence of INI elements has semantical meaning, depending of the data and its context stored in the INI data.
	///         For example, a <see cref="SectionIniElement" /> is technically independent from its following <see cref="ValueIniElement" />s, but when processed the <see cref="SectionIniElement" /> defines the section to which the following <see cref="ValueIniElement" /> belong.
	///     </note>
	///     <note type="important">
	///         Be careful when performing actions which insert, remove, or reorder INI elements as this might change the semantical meaning of one or several sections!
	///     </note>
	///     <para>
	///         <b> ANATOMY OF INI DATA &amp; ELEMENTS </b>
	///     </para>
	///     <para>
	///         INI data outside an <see cref="IniDocument" /> is organized as text line-by-line (e.g. a string or *.ini file containing the INI data).
	///         Inside an <see cref="IniDocument" />, the INI data represented by <see cref="IniElement" />s, stored in <see cref="Elements" />, is on a technical abstraction (reflecting the line-by-line organization), not a semantical abstraction.
	///         This means that basically each line of INI data is represented by a seperate <see cref="IniElement" />, depending on the type of line.
	///     </para>
	///     <para>
	///         There are four types of lines in INI data, each represented with their own derivate of <see cref="IniElement" />:
	///         Sections (<see cref="SectionIniElement" />), Name-Value-Pairs (<see cref="ValueIniElement" />), Comments (<see cref="CommentIniElement" />), and Text (<see cref="TextIniElement" />).
	///         Any possible line in a set of INI data will fit into exactly one of those types.
	///     </para>
	///     <para>
	///         <b> Sections: </b>
	///         A section is started by a section header.
	///         The section header contains the name of the section.
	///         All elements following the section header belong to that section until the next section header appears.
	///         So a section includes its section header and all elements following the section header.
	///         A section header is a single line, in the form <c> [name of the section] </c>, and is represented using <see cref="SectionIniElement" />.
	///         When INI data is parsed, leading and trailing whitespace of a line is ignored for section headers and the section name must be enclosed in <c> [ </c> and <c> ] </c>.
	///         Leading and trailing whitespace of the section name itself is not ignored.
	///         There can be multiple sections which have the same name.
	///     </para>
	///     <para>
	///         <b> Name-Value-Pairs: </b>
	///         Name-value-pairs are the actual data intended to be stored and transported by INI data.
	///         A name-value-pair is a single line, in the form of <c> name=value </c>, and is represented using <see cref="ValueIniElement" />.
	///         When INI data is parsed, leading and trailing whitespace of both the name and the value is not ignored.
	///         There can be multiple name-value-pairs inside the same (or another) section which have the same name.
	///     </para>
	///     <para>
	///         <b> Comments: </b>
	///         A comment is a text which is explicitly marked as a comment and is only intended for annotating the INI data when viewed directly (e.g. opening an *.ini file in a text editor).
	///         Comments are not processed by <see cref="IniDocument" /> besides loading/saving them to/from <see cref="Elements" />.
	///         A comment is in the form <c> ;comment </c> and represented using <see cref="CommentIniElement" />.
	///         Note that consecutive comment lines will be combined into a single <see cref="CommentIniElement" />.
	///         When INI data is parsed, leading whitespace of a comment is ignored, up to <c> ; </c>, but not ignored in the comment itself.
	///     </para>
	///     <para>
	///         <b> Text: </b>
	///         A text is everything else which is not a section header, name-value-pair, or comment, and is represented using <see cref="TextIniElement" />
	///         Therefore, technically speaking, text elements are actually invalid sections and should not be used in any processing.
	///         Text is not processed by <see cref="IniDocument" /> besides loading/saving them to/from <see cref="Elements" />.
	///         Note that consecutive text lines will be combined into a single <see cref="TextIniElement" />.
	///     </para>
	///     <note type="note">
	///         Note that elements always belong to a section.
	///         This is either the section started by the last section header or the default section if no section header appeared before the element.
	///         When &quot;outside a section&quot; is mentioned in the description of <see cref="IniDocument" />, that default section is meant.
	///     </note>
	///     <para>
	///         <b> ESCAPING </b>
	///     </para>
	///     <para>
	///         INI data uses characters with special meanings to structure elements: <c> [ </c>, <c> ] </c>, <c> = </c>, <c> ; </c>, and <c> CRLF </c> or <c> LF </c> respectively (depending on the used line-ending-style).
	///         This means that those special characters cannot appear as-is within section names, names of name-value-pairs, or values of name-value-pairs.
	///         Therefore, if such special characters are used in such a way, they need to be escaped (similar to escape sequences such as <c> \r\n </c>).
	///     </para>
	///     <para>
	///         Another special character is used to start an escape sequence: <c> | </c>.
	///         This means that the character after <c> | </c> defines which one of the special characters is to be represented by the escape sequence.
	///         The following escape sequences are possible: <c> |[ </c>, <c> |] </c>, <c> |= </c>, <c> |; </c>, <c> |r </c>, <c> |n </c>, <c> || </c>.
	///         <c> |r </c> is carriage-return (CR), <c> |n </c> new-line or line-feed (LF), and <c> || </c> is used to represent <c> | </c> itself.
	///     </para>
	///     <para>
	///         This escaping mechanism makes it possible to have any text, including multi-line text, for names and values.
	///         When INI data is read or written by <see cref="IniDocument" />, <see cref="IniReader" />, <see cref="IniWriter" />, the encoding and decoding of those escape sequences is performed automatically.
	///     </para>
	///     <para>
	///         The <c> | </c> character was choosen to start escape sequences instead of the <c> \ </c> character so that name-value-pairs, which contain windows file or directory paths as their values, are more human-readable.
	///         The used escape sequence character can be changed using <see cref="IniSettings.EscapeCharacter" />.
	///     </para>
	/// </remarks>
	public sealed class IniDocument : ICloneable,
	                                  ICloneable<IniDocument>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="IniDocument" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used for name comparison of section names and name-value-pairs.
		///     </para>
		/// </remarks>
		public IniDocument ()
			: this(null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="IniDocument" />.
		/// </summary>
		/// <param name="nameComparer"> The comparer used to compare section names and names of name-value-pairs. </param>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used if <paramref name="nameComparer" /> is null.
		///     </para>
		/// </remarks>
		public IniDocument (IEqualityComparer<string> nameComparer)
			: this(nameComparer, nameComparer)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="IniDocument" />.
		/// </summary>
		/// <param name="sectionNameComparer"> The comparer used to compare section names. </param>
		/// <param name="valueNameComparer"> The comparer used to compare names of name-value-pairs. </param>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used if <paramref name="sectionNameComparer" /> or <paramref name="valueNameComparer" /> is null.
		///     </para>
		/// </remarks>
		public IniDocument (IEqualityComparer<string> sectionNameComparer, IEqualityComparer<string> valueNameComparer)
		{
			this.Elements = new List<IniElement>();

			this.SectionNameComparer = sectionNameComparer ?? StringComparerEx.InvariantCultureIgnoreCase;
			this.ValueNameComparer = valueNameComparer ?? StringComparerEx.InvariantCultureIgnoreCase;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the number of INI elements in <see cref="Elements" />.
		/// </summary>
		/// <value>
		///     The number of INI elements in <see cref="Elements" />.
		/// </value>
		public int Count
		{
			get
			{
				return this.Elements.Count;
			}
		}

		/// <summary>
		///     Gets the list with all INI elements of this INI document.
		/// </summary>
		public IList<IniElement> Elements { get; private set; }

		/// <summary>
		///     Gets the comparer used to compare section names.
		/// </summary>
		/// <value>
		///     The comparer used to compare section names.
		/// </value>
		public IEqualityComparer<string> SectionNameComparer { get; private set; }

		/// <summary>
		///     Gets the comparer used to compare names of name-value-pairs.
		/// </summary>
		/// <value>
		///     The comparer used to compare names of name-value-pairs.
		/// </value>
		public IEqualityComparer<string> ValueNameComparer { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Adds a comment at the end of all elements.
		/// </summary>
		/// <param name="comment"> The comment. </param>
		/// <returns>
		///     The added comment element.
		/// </returns>
		public CommentIniElement AddComment (string comment)
		{
			CommentIniElement element = new CommentIniElement(comment);
			this.Elements.Add(element);
			return element;
		}

		/// <summary>
		///     Adds the values of a dictionary as a new section.
		/// </summary>
		/// <param name="sectionName"> The name of the new section (can be null). </param>
		/// <param name="mergeSections"> Specifies whether the values should be added to an existing section of the same name if one exists (true) or if a new separate section with the same name should be added (false). </param>
		/// <param name="values"> The dictionary to add as a section. </param>
		/// <returns>
		///     The list of INI elements which were added to <see cref="Elements" />.
		///     An empty list is returned if no elements were added.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, the values are added outside a section (that is: before the first section header or at the end if no section header exists).
		///     </para>
		///     <para>
		///         <paramref name="mergeSections" /> is ignored if <paramref name="sectionName" /> is null.
		///     </para>
		///     <para>
		///         If <paramref name="values" /> is empty, an empty section is added anyways (consisting only of the section header).
		///     </para>
		///     <para>
		///         This method is used if the same name-value-pair only exists once in a section.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="values" /> contains name-value-pairs with invalid names. </exception>
		public List<IniElement> AddSection (string sectionName, bool mergeSections, IDictionary<string, string> values)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			int insertIndex = this.GetInsertIndex(sectionName, ref mergeSections);

			List<IniElement> elements = new List<IniElement>();
			if ((sectionName != null) && (!mergeSections))
			{
				elements.Add(new SectionIniElement(sectionName));
			}

			foreach (KeyValuePair<string, string> value in values)
			{
				try
				{
					elements.Add(new ValueIniElement(value.Key, value.Value));
				}
				catch (ArgumentException exception)
				{
					throw new ArgumentException(exception.Message, nameof(values), exception);
				}
			}

			this.Elements.InsertRange(insertIndex, elements);

			return elements;
		}

		/// <summary>
		///     Adds the values of a dictionary as a new section.
		/// </summary>
		/// <param name="sectionName"> The name of the new section (can be null). </param>
		/// <param name="mergeSections"> Specifies whether the values should be added to an existing section of the same name if one exists (true) or if a new separate section with the same name should be added (false). </param>
		/// <param name="values"> The dictionary to add as a section. </param>
		/// <returns>
		///     The list of INI elements which were added to <see cref="Elements" />.
		///     An empty list is returned if no elements were added.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, the values are added outside a section (that is: before the first section header or at the end if no section header exists).
		///     </para>
		///     <para>
		///         <paramref name="mergeSections" /> is ignored if <paramref name="sectionName" /> is null.
		///     </para>
		///     <para>
		///         If <paramref name="values" /> is empty, an empty section is added anyways (consisting only of the section header).
		///     </para>
		///     <para>
		///         This method is used if the same name-value-pair can exist multiple times in a section.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="values" /> contains name-value-pairs with invalid names. </exception>
		public List<IniElement> AddSection (string sectionName, bool mergeSections, IDictionary<string, IList<string>> values)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			int insertIndex = this.GetInsertIndex(sectionName, ref mergeSections);

			List<IniElement> elements = new List<IniElement>();
			if ((sectionName != null) && (!mergeSections))
			{
				elements.Add(new SectionIniElement(sectionName));
			}

			foreach (KeyValuePair<string, IList<string>> value in values)
			{
				foreach (string actualValue in value.Value)
				{
					try
					{
						elements.Add(new ValueIniElement(value.Key, actualValue));
					}
					catch (ArgumentException exception)
					{
						throw new ArgumentException(exception.Message, nameof(values), exception);
					}
				}
			}

			this.Elements.InsertRange(insertIndex, elements);

			return elements;
		}

		/// <summary>
		///     Adds a section header at the end of all elements.
		/// </summary>
		/// <param name="sectionName"> The section name. </param>
		/// <returns>
		///     The added section header element.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="sectionName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public SectionIniElement AddSectionHeader (string sectionName)
		{
			if (sectionName == null)
			{
				throw new ArgumentNullException(nameof(sectionName));
			}

			if (sectionName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(sectionName));
			}

			SectionIniElement element = new SectionIniElement(sectionName);
			this.Elements.Add(element);
			return element;
		}

		/// <summary>
		///     Adds a text at the end of all elements.
		/// </summary>
		/// <param name="text"> The text. </param>
		/// <returns>
		///     The added text element.
		/// </returns>
		public TextIniElement AddText (string text)
		{
			TextIniElement element = new TextIniElement(text);
			this.Elements.Add(element);
			return element;
		}

		/// <summary>
		///     Adds a name-value-pair at the end of all elements.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns>
		///     The added name-value-pair element.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public ValueIniElement AddValue (string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			ValueIniElement element = new ValueIniElement(name, value);
			this.Elements.Add(element);
			return element;
		}

		/// <summary>
		///     Converts this INI document to a string.
		/// </summary>
		/// <returns>
		///     The string of INI data created from this INI document.
		/// </returns>
		public string AsString ()
		{
			return this.AsString(null);
		}

		/// <summary>
		///     Converts this INI document to a string.
		/// </summary>
		/// <param name="settings"> The used INI writer settings or null if default values should be used. </param>
		/// <returns>
		///     The string of INI data created from this INI document.
		/// </returns>
		public string AsString (IniWriterSettings settings)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (IniWriter iw = new IniWriter(sw, settings))
				{
					this.Save(iw);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		///     Removes all INI elements from <see cref="Elements" />.
		/// </summary>
		public void Clear ()
		{
			this.Elements.Clear();
		}

		/// <summary>
		///     Deletes the value of a specified name in a specified section.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     true if the value existed and has been deleted, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         All matching values of all matching sections are deleted.
		///         If <paramref name="section" /> is null, the value is searched and deleted outside any section.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> or <paramref name="name" /> is an empty string. </exception>
		public bool DeleteValue (string section, string name)
		{
			if (section != null)
			{
				if (section.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(section));
				}
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			List<IniElement> elements = new List<IniElement>();

			bool isMatchingSection = section == null;
			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					isMatchingSection = this.SectionNameComparer.Equals(sectionElement.SectionName, section);
				}

				if (isMatchingSection)
				{
					if (element is ValueIniElement)
					{
						ValueIniElement valueElement = (ValueIniElement)element;
						if (this.ValueNameComparer.Equals(valueElement.Name, name))
						{
							elements.Add(element);
						}
					}
				}
			}

			this.Elements.RemoveRange(elements);

			return elements.Count > 0;
		}

		/// <summary>
		///     Gets the name-value-pairs of a section as a dictionary.
		/// </summary>
		/// <param name="sectionName"> The section name (can be null). </param>
		/// <returns>
		///     The dictionary which contains the name-value-pairs of the specified section or null if the specified section does not exist.
		///     An empty dictionary is returned if the section exists but does not contain any name-value-pairs.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, all values outside a section are returned (that is: before the first section header or all if no section header exists).
		///     </para>
		///     <para>
		///         The returned dictionary uses <see cref="ValueNameComparer" />.
		///     </para>
		///     <para>
		///         If the specified section exists multiple times, only the first section is returned as a dictionary.
		///         If the same name-value-pair exists multiple times in a section, only the first pair is returned in the dictionary.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public Dictionary<string, string> GetSection (string sectionName)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			List<Dictionary<string, string>> sections = this.GetSections(sectionName);
			return sections == null ? null : (sections.Count == 0 ? new Dictionary<string, string>(this.ValueNameComparer) : sections[0]);
		}

		/// <summary>
		///     Gets the name-value-pairs of a section as a dictionary.
		/// </summary>
		/// <param name="sectionName"> The section name (can be null). </param>
		/// <returns>
		///     The dictionary which contains the name-value-pairs of the specified section or null if the specified section does not exist.
		///     An empty dictionary is returned if the section exists but does not contain any name-value-pairs.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, all values outside a section are returned (that is: before the first section header or all if no section header exists).
		///     </para>
		///     <para>
		///         The returned dictionary uses <see cref="ValueNameComparer" />.
		///     </para>
		///     <para>
		///         If the specified section exists multiple times, only the first section is returned as a dictionary.
		///         If the same name-value-pair exists multiple times in a section, all pairs are returned in the dictionary.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public Dictionary<string, List<string>> GetSectionAll (string sectionName)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			List<Dictionary<string, List<string>>> sections = this.GetSectionsAll(sectionName);
			return sections == null ? null : (sections.Count == 0 ? new Dictionary<string, List<string>>(this.ValueNameComparer) : sections[0]);
		}

		/// <summary>
		///     Gets a set of the names of all available sections.
		/// </summary>
		/// <returns>
		///     The set with the names of all available sections.
		///     An empty set is returned if no sections are available.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If values exist outside a section, the returned set also contains null.
		///     </para>
		///     <para>
		///         The returned set uses <see cref="SectionNameComparer" />.
		///     </para>
		/// </remarks>
		public HashSet<string> GetSectionNames ()
		{
			HashSet<string> sectionNames = new HashSet<string>(this.SectionNameComparer);
			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					sectionNames.Add(sectionElement.SectionName);
				}
			}
			return sectionNames;
		}

		/// <summary>
		///     Gets the name-value-pairs of a section as dictionaries.
		/// </summary>
		/// <param name="sectionName"> The section name (can be null). </param>
		/// <returns>
		///     The list which contains all the dictionaries which contain the name-value-pairs of the specified section or null if the specified section does not exist.
		///     An empty list is returned if the section exists but does not contain any name-value-pairs.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, all values outside a section are returned (that is: before the first section header or all if no section header exists).
		///         In that case, the returned list contains only one dictionary.
		///     </para>
		///     <para>
		///         The returned dictionary uses <see cref="ValueNameComparer" />.
		///     </para>
		///     <para>
		///         The returned list contains a separate dictionary for each separate section of the specified name.
		///         If the same name-value-pair exists multiple times in a section, only the first pair is returned in the corresponding dictionary.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public List<Dictionary<string, string>> GetSections (string sectionName)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			List<Dictionary<string, List<string>>> sections = this.GetSectionsAll(sectionName);
			if (sections == null)
			{
				return null;
			}

			List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
			foreach (Dictionary<string, List<string>> section in sections)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>(this.ValueNameComparer);
				foreach (KeyValuePair<string, List<string>> value in section)
				{
					dict.Add(value.Key, value.Value[0]);
				}
				result.Add(dict);
			}
			return result;
		}

		/// <summary>
		///     Gets the name-value-pairs of a section as dictionaries.
		/// </summary>
		/// <param name="sectionName"> The section name (can be null). </param>
		/// <returns>
		///     The list which contains all the dictionaries which contain the name-value-pairs of the specified section or null if the specified section does not exist.
		///     An empty list is returned if the section exists but does not contain any name-value-pairs.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, all values outside a section are returned (that is: before the first section header or all if no section header exists).
		///         In that case, the returned list contains only one dictionary.
		///     </para>
		///     <para>
		///         The returned dictionary uses <see cref="ValueNameComparer" />.
		///     </para>
		///     <para>
		///         The returned list contains a separate dictionary for each separate section of the specified name.
		///         If the same name-value-pair exists multiple times in a section, all pairs are returned in the corresponding dictionary.
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public List<Dictionary<string, List<string>>> GetSectionsAll (string sectionName)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			List<Dictionary<string, List<string>>> result = new List<Dictionary<string, List<string>>>();
			bool found = false;

			Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>(this.ValueNameComparer);
			result.Add(dict);

			bool isMatchingSection = sectionName == null;
			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					isMatchingSection = this.SectionNameComparer.Equals(sectionElement.SectionName, sectionName);
					if (isMatchingSection)
					{
						found = true;
						dict = new Dictionary<string, List<string>>(this.ValueNameComparer);
						result.Add(dict);
					}
				}

				if (isMatchingSection)
				{
					if (element is ValueIniElement)
					{
						ValueIniElement valueElement = (ValueIniElement)element;
						if (!dict.ContainsKey(valueElement.Name))
						{
							dict.Add(valueElement.Name, new List<string>());
						}
						dict[valueElement.Name].Add(valueElement.Value);
					}
				}
			}

			if (!found)
			{
				return null;
			}

			foreach (Dictionary<string, List<string>> section in result)
			{
				section.RemoveWhere(x => (x.Value == null) || (x.Value.Count == 0));
			}
			result.RemoveWhere(x => x.Count == 0);

			return result;
		}

		/// <summary>
		///     Gets the value of a specified name in a specified section.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     The value or null if the value does not exist.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The first matching value of the first matching section is returned.
		///         If <paramref name="section" /> is null, the value is searched outside any section.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> or <paramref name="name" /> is an empty string. </exception>
		public string GetValue (string section, string name)
		{
			if (section != null)
			{
				if (section.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(section));
				}
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			bool isMatchingSection = section == null;
			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					isMatchingSection = this.SectionNameComparer.Equals(sectionElement.SectionName, section);
				}

				if (isMatchingSection)
				{
					if (element is ValueIniElement)
					{
						ValueIniElement valueElement = (ValueIniElement)element;
						if (this.ValueNameComparer.Equals(valueElement.Name, name))
						{
							return valueElement.Value;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		///     Gets the name-value-pairs of all sections as a dictionary.
		/// </summary>
		/// <returns>
		///     The dictionary which contains dictionaries (one for each section) which contain the name-value-pairs.
		///     An empty dictionary is returned if no name-value-pairs exist.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The returned outer dictionary uses <see cref="SectionNameComparer" /> and the inner dictionaries use <see cref="ValueNameComparer" />.
		///     </para>
		///     <para>
		///         An inner dictionary can be empty if the section exists but has no name-value-pairs.
		///     </para>
		///     <para>
		///         If a section exists multiple times, only the first section is returned as an inner dictionary.
		///         If the same name-value-pair exists multiple times in a section, only the first pair is returned in an inner dictionary.
		///     </para>
		/// </remarks>
		public Dictionary<string, Dictionary<string, string>> GetValues ()
		{
			Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>(this.SectionNameComparer);
			HashSet<string> sectionNames = this.GetSectionNames();
			foreach (string sectionName in sectionNames)
			{
				Dictionary<string, string> section = this.GetSection(sectionName);
				result.Add(sectionName, section);
			}
			return result;
		}

		/// <summary>
		///     Loads INI elements from an existing INI reader.
		/// </summary>
		/// <param name="reader"> The INI reader from which the elements are loaded. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new sections are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="reader" /> is null. </exception>
		/// <exception cref="IniParsingException"> The INI data read by <paramref name="reader" /> contains invalid elements. </exception>
		public void Load (IniReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			List<IniElement> elements = new List<IniElement>();
			while (reader.ReadNext())
			{
				if (reader.CurrentError != IniReaderError.None)
				{
					throw new IniParsingException(reader.CurrentLineNumber, reader.CurrentError);
				}
				elements.Add(reader.CurrentElement);
			}

			this.Elements.Clear();
			this.Elements.AddRange(elements);
		}

		/// <summary>
		///     Loads INI elements from a string.
		/// </summary>
		/// <param name="data"> The INI data to load. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new sections are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="data" /> is null. </exception>
		/// <exception cref="IniParsingException"> The INI data read from <paramref name="data" /> contains invalid elements. </exception>
		public void Load (string data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			this.Load(data, (IniReaderSettings)null);
		}

		/// <summary>
		///     Loads INI elements from a string.
		/// </summary>
		/// <param name="data"> The INI data to load. </param>
		/// <param name="settings"> The used INI reader settings or null if default values should be used. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new sections are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="data" /> is null. </exception>
		/// <exception cref="IniParsingException"> The INI data read from <paramref name="data" /> contains invalid elements. </exception>
		public void Load (string data, IniReaderSettings settings)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using (StringReader sr = new StringReader(data))
			{
				using (IniReader ir = new IniReader(sr, settings))
				{
					this.Load(ir);
				}
			}
		}

		/// <summary>
		///     Loads INI elements from an existing INI file.
		/// </summary>
		/// <param name="file"> The path of the INI file to load. </param>
		/// <param name="encoding"> The encoding for reading the INI file. </param>
		/// <remarks>
		///     <para>
		///         All existing INI elements will be discarded before the new sections are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		/// <exception cref="IniParsingException"> The INI data read from <paramref name="file" /> contains invalid elements. </exception>
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
		///         All existing INI elements will be discarded before the new sections are loaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="encoding" /> is null. </exception>
		/// <exception cref="IniParsingException"> The INI data read from <paramref name="file" /> contains invalid elements. </exception>
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
		///     Merges all sections with the same name into one section.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="SectionNameComparer" /> is used for comparison.
		///     </para>
		/// </remarks>
		public void MergeSections ()
		{
			List<KeyValuePair<string, List<IniElement>>> sections = null;
			this.DecomposeSections(out sections);

			Dictionary<string, List<IniElement>> result = new Dictionary<string, List<IniElement>>(this.SectionNameComparer);
			foreach (KeyValuePair<string, List<IniElement>> section in sections)
			{
				if (!result.ContainsKey(section.Key))
				{
					result.Add(section.Key, new List<IniElement>());
				}
				result[section.Key].AddRange(section.Value.Skip(section.Key == null ? 0 : 1));
			}

			this.Elements.Clear();
			foreach (KeyValuePair<string, List<IniElement>> section in result)
			{
				this.Elements.AddRange(section.Value);
			}
		}

		/// <summary>
		///     Removes all comment INI elements from this document.
		/// </summary>
		/// <returns>
		///     true if any comment INI elements were removed, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This removes all <see cref="CommentIniElement" />s from all sections.
		///     </para>
		/// </remarks>
		public bool RemoveComments ()
		{
			return this.Elements.RemoveWhere(x => x is CommentIniElement).Count > 0;
		}

		/// <summary>
		///     Removes all sections which do not have any name-value-pairs.
		/// </summary>
		/// <returns>
		///     A set of section names which were removed.
		///     An empty set is returned if no sections were removed.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The returned set uses <see cref="SectionNameComparer" />.
		///     </para>
		///     <para>
		///         If a section only contains text or comment INI elements, the section is considered empty and is removed.
		///         Use <see cref="RemoveEmptySections(bool, bool)" /> if such elements should count as not-empty.
		///     </para>
		/// </remarks>
		public HashSet<string> RemoveEmptySections ()
		{
			return this.RemoveEmptySections(false, false);
		}

		/// <summary>
		///     Removes all sections which are empty.
		/// </summary>
		/// <param name="keepIfText"> Specifies whether sections with text INI elements (<see cref="TextIniElement" />) are considered not empty. </param>
		/// <param name="keepIfComments"> Specifies whether sections with comment INI elements (<see cref="CommentIniElement" />) are considered not empty. </param>
		/// <returns>
		///     A set of section names which were removed.
		///     An empty set is returned if no sections were removed.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The returned set uses <see cref="SectionNameComparer" />.
		///     </para>
		/// </remarks>
		public HashSet<string> RemoveEmptySections (bool keepIfText, bool keepIfComments)
		{
			List<KeyValuePair<string, List<IniElement>>> sections = null;
			this.DecomposeSections(out sections);

			List<IniElement> elementsToRemove = new List<IniElement>();
			HashSet<string> result = new HashSet<string>(this.SectionNameComparer);

			for (int i1 = 0; i1 < sections.Count; i1++)
			{
				int count = sections[i1].Value.Count(x =>
				                                     {
					                                     if ((x is SectionIniElement) || (x is ValueIniElement))
					                                     {
						                                     return true;
					                                     }

					                                     if (x is TextIniElement)
					                                     {
						                                     return keepIfText;
					                                     }

					                                     if (x is CommentIniElement)
					                                     {
						                                     return keepIfComments;
					                                     }

					                                     return false;
				                                     });

				if (count == 0)
				{
					elementsToRemove.AddRange(sections[i1].Value);
					result.Add(sections[i1].Key);
				}
			}

			this.Elements.RemoveRange(elementsToRemove);

			return result;
		}

		/// <summary>
		///     Removes all sections of a specified name.
		/// </summary>
		/// <param name="sectionName"> The name of the sections to remove (can be null). </param>
		/// <returns>
		///     The list of INI elements which were removed from <see cref="Elements" />.
		///     An empty list is returned if no elements were removed.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="sectionName" /> is null, all values outside a section are removed (that is: before the first section header or all if no section header exists).
		///     </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
		public List<IniElement> RemoveSections (string sectionName)
		{
			if (sectionName != null)
			{
				if (sectionName.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(sectionName));
				}
			}

			List<IniElement> elements = new List<IniElement>();

			bool isMatchingSection = sectionName == null;
			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					isMatchingSection = this.SectionNameComparer.Equals(sectionElement.SectionName, sectionName);
				}

				if (isMatchingSection)
				{
					elements.Add(element);
				}
			}

			this.Elements.RemoveRange(elements);

			return elements;
		}

		/// <summary>
		///     Removes all text INI elements from this document.
		/// </summary>
		/// <returns>
		///     true if any text INI elements were removed, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This removes all <see cref="TextIniElement" />s from all sections.
		///     </para>
		/// </remarks>
		public bool RemoveText ()
		{
			return this.Elements.RemoveWhere(x => x is TextIniElement).Count > 0;
		}

		/// <summary>
		///     Removes all text and comment INI elements from this document.
		/// </summary>
		/// <returns>
		///     true if any text or comment INI elements were removed, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This removes all <see cref="TextIniElement" />s and <see cref="CommentIniElement" />s from all sections.
		///     </para>
		/// </remarks>
		public bool RemoveTextAndComments ()
		{
			bool result = false;
			if (this.RemoveText())
			{
				result = true;
			}
			if (this.RemoveComments())
			{
				result = true;
			}
			return result;
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

		/// <summary>
		///     Sets the value of a specified name in a specified section.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <param name="name"> The name of the value. </param>
		/// <param name="value"> The value or null if the value should be removed (similar to <see cref="DeleteValue" />). </param>
		/// <returns>
		///     true if the value existed before, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The first matching value of the first matching section is set.
		///         If <paramref name="section" /> is null, the value is set outside any section.
		///     </para>
		///     <para>
		///         If the section does not yet exist, a new section is created and the value added at its end.
		///         If the section exists but not the value, the value is added at the end of the first matching section.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> or <paramref name="name" /> is an empty string. </exception>
		public bool SetValue (string section, string name, string value)
		{
			if (section != null)
			{
				if (section.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(section));
				}
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (value == null)
			{
				return this.DeleteValue(section, name);
			}

			bool isMatchingSection = section == null;
			int insertIndex = -1;
			for (int i1 = 0; i1 < this.Elements.Count; i1++)
			{
				IniElement element = this.Elements[i1];
				if (element is SectionIniElement)
				{
					bool wasMatchingSection = isMatchingSection;
					SectionIniElement sectionElement = (SectionIniElement)element;
					isMatchingSection = this.SectionNameComparer.Equals(sectionElement.SectionName, section);
					if (wasMatchingSection && (!isMatchingSection) && (insertIndex == -1))
					{
						insertIndex = i1;
					}
				}

				if (isMatchingSection)
				{
					if (element is ValueIniElement)
					{
						ValueIniElement valueElement = (ValueIniElement)element;
						if (this.ValueNameComparer.Equals(valueElement.Name, name))
						{
							valueElement.Value = value;
							return true;
						}
					}
				}
			}

			if (insertIndex == -1)
			{
				this.Elements.Add(new SectionIniElement(section));
				this.Elements.Add(new ValueIniElement(name, value));
			}
			else
			{
				this.Elements.Insert(insertIndex, new ValueIniElement(name, value));
			}

			return false;
		}

		/// <summary>
		///     Sets the name-value-pairs of sections specified by a dictionary.
		/// </summary>
		/// <param name="values"> The dictionary with sections and inner dictionaries for the name-value-pairs. </param>
		/// <remarks>
		///     <para>
		///         All existing sections and their name-value-pairs which are specified in the outer dictionary will be replaced by the specified values.
		///         Sections not specified in the outer dictionary will remain unchanged.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="values" /> contains invalid section names or name-value-pairs with invalid names. </exception>
		public void SetValues (IDictionary<string, IDictionary<string, string>> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			List<IniElement> backup = new List<IniElement>(this.Elements);
			bool success = false;
			try
			{
				foreach (KeyValuePair<string, IDictionary<string, string>> value in values)
				{
					try
					{
						this.RemoveSections(value.Key);
						this.AddSection(value.Key, false, value.Value);
					}
					catch (ArgumentException exception)
					{
						throw new ArgumentException(exception.Message, nameof(values), exception);
					}
				}
				success = true;
			}
			finally
			{
				if (!success)
				{
					this.Elements.Clear();
					this.Elements.AddRange(backup);
				}
			}
		}

		/// <summary>
		///     Sorts the INI elements in all sections based on their names.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used for comparison.
		///     </para>
		///     <note type="important">
		///         Sorting of INI elements in a section does only work reliable if a section only contains <see cref="ValueIniElement" />s.
		///     </note>
		/// </remarks>
		public void SortElements ()
		{
			this.SortElements(StringComparerEx.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Sorts the INI elements in all sections based on their names.
		/// </summary>
		/// <param name="comparer"> The comparer used to compare the names of name-value-pairs. </param>
		/// <remarks>
		///     <note type="important">
		///         Sorting of INI elements in a section does only work reliable if a section only contains <see cref="ValueIniElement" />s.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="comparer" /> is null. </exception>
		public void SortElements (IComparer<string> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			this.SortElements(this.CreateElementOrderComparer(comparer));
		}

		/// <summary>
		///     Sorts the INI elements in all sections.
		/// </summary>
		/// <param name="comparer"> The comparer used to compare INI elements. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="comparer" /> is null. </exception>
		public void SortElements (IComparer<IniElement> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			this.SortElementsInternal(null, comparer, true);
		}

		/// <summary>
		///     Sorts the INI elements in a specified section based on their names.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used for comparison.
		///     </para>
		///     <para>
		///         All elements in all sections matching the specified section name are sorted.
		///         If <paramref name="section" /> is null, the elements outside any sections are sorted.
		///     </para>
		///     <note type="important">
		///         Sorting of INI elements in a section does only work reliable if a section only contains <see cref="ValueIniElement" />s.
		///     </note>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> is an empty string. </exception>
		public void SortElements (string section)
		{
			this.SortElements(section, StringComparerEx.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Sorts the INI elements in a specified section based on their names.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <param name="comparer"> The comparer used to compare the names of name-value-pairs. </param>
		/// <remarks>
		///     <para>
		///         All elements in all sections matching the specified section name are sorted.
		///         If <paramref name="section" /> is null, the elements outside any sections are sorted.
		///     </para>
		///     <note type="important">
		///         Sorting of INI elements in a section does only work reliable if a section only contains <see cref="ValueIniElement" />s.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="comparer" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> is an empty string. </exception>
		public void SortElements (string section, IComparer<string> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			this.SortElements(section, this.CreateElementOrderComparer(comparer));
		}

		/// <summary>
		///     Sorts the INI elements in a specified section.
		/// </summary>
		/// <param name="section"> The name of the section (can be null). </param>
		/// <param name="comparer"> The comparer used to compare INI elements. </param>
		/// <remarks>
		///     <para>
		///         All elements in all sections matching the specified section name are sorted.
		///         If <paramref name="section" /> is null, the elements outside any sections are sorted.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="comparer" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="section" /> is an empty string. </exception>
		public void SortElements (string section, IComparer<IniElement> comparer)
		{
			if (section != null)
			{
				if (section.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(section));
				}
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			this.SortElementsInternal(section, comparer, false);
		}

		/// <summary>
		///     Sorts the regions based on their names.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used for comparison.
		///     </para>
		/// </remarks>
		public void SortRegions ()
		{
			this.SortRegions(StringComparerEx.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Sorts the regions based on their names.
		/// </summary>
		/// <param name="comparer"> The comparer used to compare the region names. </param>
		public void SortRegions (IComparer<string> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			List<KeyValuePair<string, List<IniElement>>> sections = null;
			this.DecomposeSections(out sections);

			if (sections.Count == 0)
			{
				return;
			}

			OrderComparison<KeyValuePair<string, List<IniElement>>> sectionComparer = new OrderComparison<KeyValuePair<string, List<IniElement>>>((x, y) => comparer.Compare(x.Key, y.Key));

			int startIndex = sections[0].Key == null ? 1 : 0;
			sections.Sort(startIndex, sections.Count - startIndex, sectionComparer);

			this.Elements.Clear();
			foreach (KeyValuePair<string, List<IniElement>> section in sections)
			{
				this.Elements.AddRange(section.Value);
			}
		}

		private IComparer<IniElement> CreateElementOrderComparer (IComparer<string> comparer)
		{
			return new OrderComparison<IniElement>((x, y) =>
			                                       {
				                                       if ((!(x is ValueIniElement)) || (!(y is ValueIniElement)))
				                                       {
					                                       return 0;
				                                       }
				                                       return comparer.Compare(((ValueIniElement)x).Name, ((ValueIniElement)y).Name);
			                                       });
		}

		private void DecomposeSections (out List<KeyValuePair<string, List<IniElement>>> sections)
		{
			sections = new List<KeyValuePair<string, List<IniElement>>>();

			List<IniElement> currentElements = new List<IniElement>();
			sections.Add(new KeyValuePair<string, List<IniElement>>(null, currentElements));

			foreach (IniElement element in this.Elements)
			{
				if (element is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)element;
					currentElements = new List<IniElement>();
					sections.Add(new KeyValuePair<string, List<IniElement>>(sectionElement.SectionName, currentElements));
				}
				currentElements.Add(element);
			}
		}

		private int GetInsertIndex (string sectionName, ref bool mergeSections)
		{
			if (sectionName == null)
			{
				mergeSections = false;
				for (int i1 = 0; i1 < this.Elements.Count; i1++)
				{
					if (this.Elements[i1] is SectionIniElement)
					{
						return i1;
					}
				}
				return this.Elements.Count;
			}

			int lastMatchingIndex = -1;
			bool matchingSectionFound = false;
			for (int i1 = 0; i1 < this.Elements.Count; i1++)
			{
				if (this.Elements[i1] is SectionIniElement)
				{
					SectionIniElement sectionElement = (SectionIniElement)this.Elements[i1];
					if (this.SectionNameComparer.Equals(sectionElement.SectionName, sectionName))
					{
						matchingSectionFound = true;
					}
					else
					{
						if (matchingSectionFound)
						{
							lastMatchingIndex = i1;
							break;
						}
					}
				}
			}

			if (lastMatchingIndex == -1)
			{
				mergeSections = false;
				lastMatchingIndex = this.Elements.Count;
			}

			return lastMatchingIndex;
		}

		private void SortElementsInternal (string section, IComparer<IniElement> comparer, bool allSections)
		{
			List<KeyValuePair<string, List<IniElement>>> sections = null;
			this.DecomposeSections(out sections);

			if (sections.Count == 0)
			{
				return;
			}

			foreach (KeyValuePair<string, List<IniElement>> currentSection in sections)
			{
				if (allSections || this.SectionNameComparer.Equals(currentSection.Key, section))
				{
					int startIndex = currentSection.Key == null ? 0 : 1;
					currentSection.Value.Sort(startIndex, currentSection.Value.Count - startIndex, comparer);
				}
			}

			this.Elements.Clear();
			foreach (KeyValuePair<string, List<IniElement>> currentSection in sections)
			{
				this.Elements.AddRange(currentSection.Value);
			}
		}

		#endregion




		#region Interface: ICloneable

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<IniDocument>

		/// <inheritdoc />
		public IniDocument Clone ()
		{
			IniDocument clone = new IniDocument(this.SectionNameComparer, this.SectionNameComparer);
			clone.Load(this.AsString());
			return clone;
		}

		#endregion
	}
}
