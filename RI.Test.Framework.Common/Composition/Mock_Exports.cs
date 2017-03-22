using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Exports_1
	{
	}

	[Export]
	public sealed class Mock_Exports_2
	{
	}

	[Export("E3")]
	public sealed class Mock_Exports_3A
	{
	}

	[Export("E3")]
	public sealed class Mock_Exports_3B
	{
	}

	[Export("E4")]
	public sealed class Mock_Exports_4A
	{
	}

	[Export("E4")]
	public sealed class Mock_Exports_4B
	{
	}

	[Export(Inherited = false)]
	public interface Mock_Exports_5
	{
	}

	public sealed class Mock_Exports_5A : Mock_Exports_5
	{
	}

	public sealed class Mock_Exports_5B : Mock_Exports_5
	{
	}

	[Export(Inherited = true)]
	public interface Mock_Exports_6
	{
	}

	public sealed class Mock_Exports_6A : Mock_Exports_6
	{
	}

	public sealed class Mock_Exports_6B : Mock_Exports_6
	{
	}

	[Export]
	public sealed class Mock_Exports_7
	{
		[ExportConstructor]
		public Mock_Exports_7 ()
		{
		}

		public Mock_Exports_7 (string value)
		{
		}
	}

	[Export]
	public sealed class Mock_Exports_8
	{
		public Mock_Exports_7 Value { get; }

		[ExportConstructor]
		public Mock_Exports_8(Mock_Exports_7 value)
		{
			this.Value = value;
		}

		public Mock_Exports_8(string value)
		{
		}
	}

	[Export]
	public sealed class Mock_Exports_9
	{
		[ExportCreator]
		public static Mock_Exports_9 Create (Type type)
		{
			return new Mock_Exports_9
			{
				Value = "Test123"
			};
		}

		public string Value { get; private set; }
	}
}
