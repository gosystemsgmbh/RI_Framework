using System;
using System.Globalization;




namespace RI.Test.Framework.Mocks
{
	public sealed class RENP : IEquatable<RENP>
	{
		#region Static Methods

		public static bool operator == (RENP x, RENP y)
		{
			return object.ReferenceEquals(x, y);
		}

		public static implicit operator RENP (string value)
		{
			return new RENP(value);
		}

		public static implicit operator RENP (int number)
		{
			return new RENP(number);
		}

		public static bool operator != (RENP x, RENP y)
		{
			return !object.ReferenceEquals(x, y);
		}

		#endregion




		#region Instance Constructor/Destructor

		public RENP ()
		{
			this.Value = null;
		}

		public RENP (string value)
			: this()
		{
			this.Value = value;
		}

		public RENP (int number)
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
			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}

		#endregion




		#region Interface: IEquatable<RENP>

		public bool Equals (RENP other)
		{
			return object.ReferenceEquals(this, other);
		}

		#endregion
	}
}
