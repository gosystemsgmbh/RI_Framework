using System;
using System.Globalization;




namespace RI.Test.Framework.Mocks
{
	public sealed class VENP : IEquatable<VENP>
	{
		#region Static Methods

		public static bool operator == (VENP x, VENP y)
		{
			if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
			{
				return true;
			}

			if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
			{
				return false;
			}

			return x.Equals(y);
		}

		public static implicit operator VENP (string value)
		{
			return new VENP(value);
		}

		public static implicit operator VENP (int number)
		{
			return new VENP(number);
		}

		public static bool operator != (VENP x, VENP y)
		{
			return !(x == y);
		}

		#endregion




		#region Instance Constructor/Destructor

		public VENP ()
		{
			this.Value = null;
		}

		public VENP (string value)
			: this()
		{
			this.Value = value;
		}

		public VENP (int number)
			: this()
		{
			this.Number = number;
		}

		#endregion




		#region Instance Properties/Indexer

		public int Number
		{
			get
			{
				return int.Parse(this.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
			}
			set
			{
				this.Value = value.ToString("D", CultureInfo.InvariantCulture);
			}
		}

		public string Value { get; set; }

		#endregion




		#region Overrides

		public override bool Equals (object obj)
		{
			return this.Equals(obj as VENP);
		}

		public override int GetHashCode ()
		{
			if (this.Value == null)
			{
				return 0;
			}

			return this.Value.GetHashCode();
		}

		public override string ToString ()
		{
			return this.Value;
		}

		#endregion




		#region Interface: IEquatable<VENP>

		public bool Equals (VENP other)
		{
			if (other == null)
			{
				return false;
			}

			return string.Equals(this.Value, other.Value, StringComparison.Ordinal);
		}

		#endregion
	}
}
