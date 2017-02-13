using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;




namespace RI.Tools.Framework.VersionUpdater
{
	public static class VersionUpdater
	{
		#region Constants

		private const string TemplateExtension = "TEMPLATE";

		#endregion




		#region Static Methods

		public static void Main (string[] args)
		{
			args = args ?? new string[0];

			if (args.Length != 6)
			{
				Console.WriteLine("ERROR: Invalid command line parameter count!");
				return;
			}

			string fullPath = Path.GetFullPath(args[0]);
			bool recursive = string.Equals(args[1], "r", StringComparison.InvariantCultureIgnoreCase);

			Version newVersion;
			string newCompany;
			string newCopyright;

			if (string.Equals(args[2], "file", StringComparison.InvariantCultureIgnoreCase))
			{
				newVersion = new Version(File.ReadAllText(Path.GetFullPath(args[3])).Trim());
				newCompany = File.ReadAllText(Path.GetFullPath(args[4])).Trim();
				newCopyright = File.ReadAllText(Path.GetFullPath(args[5])).Trim();
			}
			else
			{
				newVersion = new Version(args[3]);
				newCompany = args[4];
				newCopyright = args[5];
			}

			Console.WriteLine("Directory:     " + fullPath);
			Console.WriteLine("Recursive:     " + recursive);
			Console.WriteLine("New version:   " + newVersion.ToString(4));
			Console.WriteLine("New company:   " + newCompany);
			Console.WriteLine("New copyright: " + newCopyright);
			Console.WriteLine("Command line:  " + Environment.CommandLine);

			string[] files = Directory.GetFiles(fullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			Console.WriteLine("---");

			foreach (string file in files)
			{
				string extension = Path.GetExtension(file)?.ToUpperInvariant();
				string templateExtension = "." + VersionUpdater.TemplateExtension + extension;
				if (file.EndsWith(templateExtension, StringComparison.InvariantCultureIgnoreCase))
				{
					string outFile = file.Substring(0, file.Length - templateExtension.Length) + Path.GetExtension(file);
					VersionUpdater.ProcessFileToken(file, outFile, newVersion, newCompany, newCopyright);
				}
				else
				{
					VersionUpdater.ProcessFileRegex(file, newVersion, newCompany, newCopyright);
				}
			}

			Console.WriteLine("---");
			Console.WriteLine("Finished!");
		}

		private static void PerformReplacementRegex (string file, string search, string replacement, Encoding encoding)
		{
			string input;
			using (StreamReader reader = new StreamReader(file, encoding, true))
			{
				input = reader.ReadToEnd();
			}

			string output = Regex.Replace(input, search, replacement, RegexOptions.CultureInvariant | RegexOptions.Multiline);

			using (StreamWriter writer = new StreamWriter(file, false, encoding))
			{
				writer.Write(output);
			}
		}

		private static void PerformReplacementToken (string file, string search, string replacement, Encoding encoding)
		{
			string input;
			using (StreamReader reader = new StreamReader(file, encoding, true))
			{
				input = reader.ReadToEnd();
			}

			string output = input.Replace(search, replacement);

			using (StreamWriter writer = new StreamWriter(file, false, encoding))
			{
				writer.Write(output);
			}
		}

		private static void ProcessFileRegex (string file, Version newVersion, string newCompany, string newCopyright)
		{
			string extension = Path.GetExtension(file)?.ToUpperInvariant();
			string fileName = Path.GetFileName(file)?.ToUpperInvariant();

			string newVersionComma = newVersion.ToString(4).Replace('.', ',');
			string newVersionDot = newVersion.ToString(4);

			byte[] versionBytes = Encoding.UTF8.GetBytes(newVersionDot);
			byte[] versionGuidBytes;
			using (MD5 hasher = MD5.Create())
			{
				versionGuidBytes = hasher.ComputeHash(versionBytes);
			}
			Guid versionGuid = new Guid(versionGuidBytes);

			string search;
			string replacement;

			bool processed = false;

			switch (extension)
			{
				case ".DOX":
				{
					search = @"(?'part1'^\s*VersionMajor\s*=\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Major.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*VersionMinor\s*=\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Minor.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*VersionRelease\s*=\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Build.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*VersionBuild\s*=\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Revision.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case ".AIP":
				{
					search = "(?'part1'Property\\s*=\\s*\"\\s*ProductVersion\\s*\"\\s+Value\\s*=\\s*\"\\s*)[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\\s*\")";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					//HINT: Replaced with version-dependend GUID to always get the same GUID for the same version (prevents new installer IDs inter-version)		
					search = "(?'part1'Property\\s*=\\s*\"\\s*ProductCode\\s*\"\\s+Value\\s*=\\s*\"\\s*[0-9]*?[:]*)[\\{]*[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}[\\}]*(?'part2'\\s*\")";
					replacement = "${part1}" + versionGuid.ToString("B", CultureInfo.InvariantCulture).ToUpperInvariant() + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case ".WXS":
				{
					search = "(?'part1'<\\?define\\s+InstallerVersion\\s*=\\s*)[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\\s+\\?>)";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					//HINT: Replaced with version-dependend GUID to always get the same GUID for the same version (prevents new installer IDs inter-version)		
					//search = "(?'part1'Property\\s*=\\s*\"\\s*ProductCode\\s*\"\\s+Value\\s*=\\s*\"\\s*[0-9]*?[:]*)[\\{]*[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}[\\}]*(?'part2'\\s*\")";
					search = "(?'part1'<\\?define\\s+InstallerProductCode\\s*=\\s*)[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(?'part2'\\s+\\?>)";
					replacement = "${part1}" + versionGuid.ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant() + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case ".NSH":
				case ".NSI":
				case ".NSS":
				{
					search = "(?'part1'^\\s*!define\\s*PRODUCT_VERSION\\s*\"\\s*)[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\\s*\")";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'^\\s*!define\\s*COMPANY_NAME\\s*\"\\s*)[\\w\\s\\(\\)\\.,-_]*(?'part2'\\s*\")";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'^\\s*!define\\s*COMPANY_COPYRIGHT\\s*\"\\s*)[\\w\\s\\(\\)\\.,-_]*(?'part2'\\s*\")";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					processed = true;

					break;
				}

				case ".HTML":
				case ".HTM":
				{
					search = @"(?'part1'\s*Version:\s*)[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+(?'part2'\s*)";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'\s*Company:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*)";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'\s*Copyright:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*)";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case ".TXT":
				{
					search = @"(?'part1'^\s*\*{1}\s*Version:\s*)[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+(?'part2'\s*\*{1}\s*$)";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*\*{1}\s*Company:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*\*{1}\s*$)";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*\*{1}\s*Copyright:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*\*{1}\s*$)";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case ".BAT":
				{
					search = @"(?'part1'set\sbuild_version_major=)[0-9]+";
					replacement = "${part1}" + newVersion.Major.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'set\sbuild_version_minor=)[0-9]+";
					replacement = "${part1}" + newVersion.Minor.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'set\sbuild_version_fix=)[0-9]+";
					replacement = "${part1}" + newVersion.Build.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'set\sbuild_version_revision=)[0-9]+";
					replacement = "${part1}" + newVersion.Revision.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					processed = true;

					break;
				}

				case ".SHFBPROJ":
				{
					search = @"(?'part1'\s*<HelpFileVersion>\s*)[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+(?'part2'\s*</HelpFileVersion>\s*)";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}
			}

			switch (fileName)
			{
				case "APP.MANIFEST":
				{
					search = "(?'part1'assemblyIdentity\\s*version\\s*=\\s*\"\\s*)[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\\s*\")";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case "ASSEMBLYINFO.CS":
				{
					search = "(?'part1'Assembly[a-zA-Z]*?Version(Attribute)*\\s*\\(\\s*\")[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = "(?'part1'AssemblyCompany(Attribute)*\\s*\\(\\s*\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = "(?'part1'AssemblyCopyright(Attribute)*\\s*\\(\\s*\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case "ASSEMBLYINFO.CPP":
				{
					search = "(?'part1'Assembly[a-zA-Z]*?Version(Attribute)*\\s*\\(\\s*L\")[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = "(?'part1'AssemblyCompany(Attribute)*\\s*\\(\\s*L\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = "(?'part1'AssemblyCopyright(Attribute)*\\s*\\(\\s*L\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\"\\s*\\))";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case "ASSEMBLYINFO.RC":
				{
					search = @"(?'part1'FILEVERSION\s+)[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+";
					replacement = "${part1}" + newVersionComma;
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'PRODUCTVERSION\s+)[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+";
					replacement = "${part1}" + newVersionComma;
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'\"FileVersion\"\\s*,\\s*\")[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\")";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'\"ProductVersion\"\\s*,\\s*\")[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+(?'part2'\")";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'\"CompanyName\"\\s*,\\s*\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\")";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = "(?'part1'\"LegalCopyright\"\\s*,\\s*\")[\\w\\s\\(\\)\\.,-_]*(?'part2'\")";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					processed = true;

					break;
				}

				case "[CONTENT_INFO].TXT":
				{
					search = @"(?'part1'^\s*Version:\s*)[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+(?'part2'\s*$)";
					replacement = "${part1}" + newVersionDot + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*Company:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*$)";
					replacement = "${part1}" + newCompany + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					search = @"(?'part1'^\s*Copyright:\s*)[\w\s\(\)\.,-_]*(?'part2'\s*$)";
					replacement = "${part1}" + newCopyright + "${part2}";
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}

				case "SYSTEM_VERSION.H":
				{
					search = @"(?'part1'\s*CPU_SW_VERSION_MAJOR\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Major.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'\s*CPU_SW_VERSION_MINOR\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Minor.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'\s*CPU_SW_VERSION_FIX\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Build.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					search = @"(?'part1'\s*CPU_SW_VERSION_REVISION\s*)[0-9]+";
					replacement = "${part1}" + newVersion.Revision.ToString(CultureInfo.InvariantCulture);
					VersionUpdater.PerformReplacementRegex(file, search, replacement, Encoding.Default);

					processed = true;

					break;
				}
			}

			if (processed)
			{
				Console.WriteLine("File: " + file);
			}
		}

		private static void ProcessFileToken (string inFile, string outFile, Version newVersion, string newCompany, string newCopyright)
		{
			File.Copy(inFile, outFile, true);

			string extension = Path.GetExtension(inFile).ToUpperInvariant();

			string newVersionDot = newVersion.ToString(4);

			bool processed = false;

			switch (extension)
			{
				case ".HTML":
				case ".HTM":
				{
					string search = @"($version$)";
					string replacement = newVersionDot;

					VersionUpdater.PerformReplacementToken(outFile, search, replacement, Encoding.UTF8);

					search = @"($company$)";
					replacement = newCompany;
					VersionUpdater.PerformReplacementToken(outFile, search, replacement, Encoding.UTF8);

					search = @"($copyright$)";
					replacement = newCopyright;
					VersionUpdater.PerformReplacementToken(outFile, search, replacement, Encoding.UTF8);

					processed = true;

					break;
				}
			}

			if (processed)
			{
				Console.WriteLine("File: " + inFile + " -> " + outFile);
			}
		}

		#endregion
	}
}
