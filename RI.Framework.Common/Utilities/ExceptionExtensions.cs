using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Text;




namespace RI.Framework.Utilities
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Exception" /> type.
	/// </summary>
	public static class ExceptionExtensions
	{
		#region Constants

		private const string DefaultIndent = " ";

		private const string NullString = "[null]";

		private const string PropertyPrefix = "# ";

		private const string PropertySeparator = " : ";

		private const string StackTracePrefix = "-> ";

		private const string TargetSiteSeparator = ".";

		private static readonly string[] IgnoredProperties = new[]
		{
			"Message", "Source", "TargetSite", "HelpLink", "StackTrace", "InnerException"
		};

		#endregion




		#region Static Methods

		/// <summary>
		///     Creates a detailed string representation of an exception.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <returns>
		///     The detailed string representation of the exception.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The created string representation is not intented for serializing or storing an exception, it is only used for logging and debugging purposes.
		///     </note>
		///     <para>
		///         A single space character is used as an indentation string for inner exceptions.
		///     </para>
		/// </remarks>
		public static string ToDetailedString (this Exception exception)
		{
			return exception.ToDetailedString(null);
		}

		/// <summary>
		///     Creates a detailed string representation of an exception.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <param name="indentString"> An indentation string which is used to indent inner exceptions in the string. </param>
		/// <returns>
		///     The detailed string representation of the exception.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The created string representation is not intented for serializing or storing an exception, it is only used for logging and debugging purposes.
		///     </note>
		/// </remarks>
		[SuppressMessage ("ReSharper", "ConstantConditionalAccessQualifier")]
		[SuppressMessage ("ReSharper", "ConstantNullCoalescingCondition")]
		public static string ToDetailedString (this Exception exception, string indentString)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			indentString = indentString ?? ExceptionExtensions.DefaultIndent;

			StringBuilder sb = new StringBuilder();

			using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				using (IndentedTextWriter writer = new IndentedTextWriter(stringWriter))
				{
					writer.IndentEmptyLines = false;
					writer.IndentLevel = 0;
					writer.IndentString = indentString;

					writer.Write("Message:     ");
					writer.WriteLine(exception.Message.Trim());

					writer.Write("Type:        ");
					writer.WriteLine(exception.GetType().AssemblyQualifiedName);

					writer.Write("Source:      ");
					writer.WriteLine(exception.Source == null ? ExceptionExtensions.NullString : exception.Source.Trim());

					writer.Write("Target site: ");
					if (exception.TargetSite == null)
					{
						writer.WriteLine(ExceptionExtensions.NullString);
					}
					else
					{
						writer.Write(exception.TargetSite?.DeclaringType?.AssemblyQualifiedName?.Trim() ?? ExceptionExtensions.NullString);
						writer.Write(ExceptionExtensions.TargetSiteSeparator);
						writer.WriteLine(exception.TargetSite?.Name?.Trim() ?? ExceptionExtensions.NullString);
					}

					writer.Write("Help link:   ");
					writer.WriteLine(exception.HelpLink == null ? ExceptionExtensions.NullString : exception.HelpLink.Trim());

					try
					{
						PropertyInfo[] properties = exception.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (PropertyInfo property in properties)
						{
							try
							{
								MethodInfo getter = property.GetGetMethod(true);
								object propertyValue = getter.Invoke(exception, null);

								string name = property.Name;
								if (ExceptionExtensions.IgnoredProperties.Contains(name))
								{
									continue;
								}

								string type = propertyValue?.GetType()?.Name ?? property.PropertyType.Name;
								string stringValue = ExceptionExtensions.NullString;

								if (propertyValue != null)
								{
									if (propertyValue is IDictionary)
									{
										stringValue = (from DictionaryEntry x in ((IDictionary)propertyValue) select "[" + x.Key + "]=[" + x.Value + "]").Join(";");
									}
									else if (propertyValue is IEnumerable)
									{
										stringValue = (from x in ((IEnumerable)propertyValue).OfType<object>() select "[" + x + "]").Join(";");
									}
									else
									{
										stringValue = propertyValue.ToString();
									}
								}

								string escapedStringValue = stringValue.Escape();

								writer.Write(ExceptionExtensions.PropertyPrefix);
								writer.Write(name);
								writer.Write(ExceptionExtensions.PropertySeparator);
								writer.Write(type);
								writer.Write(ExceptionExtensions.PropertySeparator);
								writer.WriteLine(escapedStringValue);
							}
							catch
							{
								writer.Write("(failure while printing exception properties; property: )");
								writer.WriteLine(property.Name);
							}
						}
					}
					catch
					{
						writer.WriteLine("(failure while printing exception properties; general)");
					}

					writer.Write("Stack trace:");
					if (exception.StackTrace == null)
					{
						writer.Write(" ");
						writer.WriteLine(ExceptionExtensions.NullString);
					}
					else
					{
						string[] lines = exception.StackTrace.SplitLines(StringSplitOptions.RemoveEmptyEntries);
						if (lines.Length == 0)
						{
							writer.Write(" ");
							writer.WriteLine(ExceptionExtensions.NullString);
						}
						else
						{
							writer.WriteLine();
							for (int i1 = 0; i1 < lines.Length; i1++)
							{
								string line = lines[i1];
								writer.Write(ExceptionExtensions.StackTracePrefix);
								writer.WriteLine(line.Trim());
							}
						}
					}

					if (exception.InnerException != null)
					{
						writer.WriteLine("Inner exception:");
						writer.IndentLevel++;
						writer.WriteLine(exception.InnerException.ToDetailedString(indentString));
						writer.IndentLevel--;
					}
				}
			}

			return sb.ToString().Trim();
		}

		/// <summary>
		///     Creates a detailed string representation of an exception.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <param name="indentChar"> An indentation character which is used to indent inner exceptions in the string. </param>
		/// <returns>
		///     The detailed string representation of the exception.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The created string representation is not intented for serializing or storing an exception, it is only used for logging and debugging purposes.
		///     </note>
		/// </remarks>
		public static string ToDetailedString (this Exception exception, char indentChar)
		{
			return exception.ToDetailedString(new string(indentChar, 1));
		}

		#endregion
	}
}
