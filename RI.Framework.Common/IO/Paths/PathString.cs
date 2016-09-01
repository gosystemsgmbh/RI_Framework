using System;

using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.IO.Paths
{
	/// <summary>
	///     Base class for specialized path objects (<see cref="FilePath" />, <see cref="DirectoryPath" />).
	/// </summary>
	[Serializable]
	public abstract class PathString : IEquatable<PathString>,
	                                   IComparable,
	                                   IComparable<PathString>,
	                                   ICloneable,
	                                   ICloneable<PathString>
	{
		#region Static Methods

		/// <summary>
		///     Compares two <see cref="PathString" />s for order.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The order of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="CompareTo(PathString)" /> for more details.
		/// </remarks>
		public static int Compare (PathString x, PathString y)
		{
			if (( x == null ) && ( y == null ))
			{
				return 0;
			}

			if (x == null)
			{
				return 1;
			}

			if (y == null)
			{
				return -1;
			}

			return x.CompareTo(y);
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for equality.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The equality of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="Equals(PathString)" /> for more details.
		/// </remarks>
		public static bool Equals (PathString x, PathString y)
		{
			if (( x == null ) && ( y == null ))
			{
				return true;
			}

			if (( x == null ) || ( y == null ))
			{
				return false;
			}

			return x.Equals(y);
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for equality.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The equality of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="Equals(PathString)" /> for more details.
		/// </remarks>
		public static bool operator == (PathString x, PathString y)
		{
			return ( PathString.Equals(x, y) );
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for order.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The order of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="CompareTo(PathString)" /> for more details.
		/// </remarks>
		public static bool operator > (PathString x, PathString y)
		{
			return ( PathString.Compare(x, y) > 0 );
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for order.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The order of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="CompareTo(PathString)" /> for more details.
		/// </remarks>
		public static bool operator >= (PathString x, PathString y)
		{
			return ( PathString.Compare(x, y) >= 0 );
		}

		/// <summary>
		///     Implicit conversion of a <see cref="PathString" /> to <see cref="string" />.
		/// </summary>
		/// <param name="path"> The path to convert to a string. </param>
		public static implicit operator string (PathString path)
		{
			if (path == null)
			{
				return null;
			}

			return path.PathResolved;
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for equality.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The equality of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="Equals(PathString)" /> for more details.
		/// </remarks>
		public static bool operator != (PathString x, PathString y)
		{
			return ( !PathString.Equals(x, y) );
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for order.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The order of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="CompareTo(PathString)" /> for more details.
		/// </remarks>
		public static bool operator < (PathString x, PathString y)
		{
			return ( PathString.Compare(x, y) < 0 );
		}

		/// <summary>
		///     Compares two <see cref="PathString" />s for order.
		/// </summary>
		/// <param name="x"> The first <see cref="PathString" />. </param>
		/// <param name="y"> The second <see cref="PathString" />. </param>
		/// <returns> The order of <paramref name="y" /> compared to <paramref name="x" />. </returns>
		/// <remarks>
		///     See <see cref="CompareTo(PathString)" /> for more details.
		/// </remarks>
		public static bool operator <= (PathString x, PathString y)
		{
			return ( PathString.Compare(x, y) <= 0 );
		}

		#endregion




		#region Instance Constructor/Destructor

		internal PathString (PathProperties path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (!path.IsValid)
			{
				throw new InvalidPathArgumentException(nameof(path), path.Error);
			}

			this.PathInternal = path;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <inheritdoc cref="PathProperties.HasRelatives" />
		public bool HasRelatives
		{
			get
			{
				return this.PathInternal.HasRelatives;
			}
		}

		/// <inheritdoc cref="PathProperties.HasWildcards" />
		public bool HasWildcards
		{
			get
			{
				return this.PathInternal.HasWildcards;
			}
		}

		/// <inheritdoc cref="PathProperties.IsRoot" />
		public bool IsRoot
		{
			get
			{
				return this.PathInternal.IsRoot;
			}
		}

		/// <inheritdoc cref="PathProperties.IsRooted" />
		public bool IsRooted
		{
			get
			{
				return this.PathInternal.IsRooted;
			}
		}

		/// <inheritdoc cref="PathProperties.PathNormalized" />
		public string PathNormalized
		{
			get
			{
				return this.PathInternal.PathNormalized;
			}
		}

		/// <inheritdoc cref="PathProperties.PathOriginal" />
		public string PathOriginal
		{
			get
			{
				return this.PathInternal.PathOriginal;
			}
		}

		/// <inheritdoc cref="PathProperties.PathResolved" />
		public string PathResolved
		{
			get
			{
				return this.PathInternal.PathResolved;
			}
		}

		/// <inheritdoc cref="PathProperties.Root" />
		public DirectoryPath Root
		{
			get
			{
				return new DirectoryPath(this.PathInternal.Root);
			}
		}

		/// <inheritdoc cref="PathProperties.Type" />
		public PathType Type
		{
			get
			{
				return this.PathInternal.Type;
			}
		}

		internal PathProperties PathInternal { get; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="PathString.CompareTo(PathString)" />
		public int CompareTo (string other)
		{
			if (other == null)
			{
				return this.CompareTo((PathProperties)null);
			}

			return this.CompareTo(PathProperties.FromPath(other, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.PathInternal.Type));
		}

		/// <inheritdoc cref="PathString.CompareTo(PathString)" />
		public int CompareTo (PathProperties other)
		{
			return this.PathInternal.CompareTo(other);
		}

		/// <inheritdoc cref="PathString.Equals(PathString)" />
		public bool Equals (string other)
		{
			if (other == null)
			{
				return this.Equals((PathProperties)null);
			}

			return this.Equals(PathProperties.FromPath(other, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.PathInternal.Type));
		}

		/// <inheritdoc cref="PathString.Equals(PathString)" />
		public bool Equals (PathProperties other)
		{
			return this.PathInternal.Equals(other);
		}

		/// <summary>
		///     Gets the properties of the path represented by this <see cref="PathString" />.
		/// </summary>
		/// <returns>
		///     The properties of the path represented by this <see cref="PathString" />
		/// </returns>
		public PathProperties GetPathProperties ()
		{
			return this.PathInternal.Clone();
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Clones this <see cref="PathString" /> instance.
		/// </summary>
		/// <returns>
		///     The clone of this <see cref="PathString" /> instance.
		/// </returns>
		protected abstract PathString CloneInternal ();

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override bool Equals (object obj)
		{
			return this.Equals(obj as PathString);
		}

		/// <inheritdoc />
		public override int GetHashCode ()
		{
			return this.PathInternal.GetHashCode();
		}

		/// <inheritdoc />
		public override string ToString ()
		{
			return this.PathInternal.ToString();
		}

		#endregion




		#region Interface: ICloneable

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.CloneInternal();
		}

		#endregion




		#region Interface: ICloneable<PathString>

		/// <inheritdoc />
		PathString ICloneable<PathString>.Clone ()
		{
			return this.CloneInternal();
		}

		#endregion




		#region Interface: IComparable

		/// <inheritdoc />
		public int CompareTo (object obj)
		{
			return this.CompareTo(obj as PathString);
		}

		#endregion




		#region Interface: IComparable<PathString>

		/// <inheritdoc />
		public int CompareTo (PathString other)
		{
			return this.CompareTo(other?.PathInternal);
		}

		#endregion




		#region Interface: IEquatable<PathString>

		/// <inheritdoc />
		public bool Equals (PathString other)
		{
			return this.Equals(other?.PathInternal);
		}

		#endregion
	}
}
