using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.Linq;
using RI.Framework.Utilities.Comparison;
using RI.Test.Framework.Mocks;




namespace RI.Test.Framework.Collections.Comparison
{
	[TestClass]
	public sealed class Test_CollectionComparer
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Func<List<VENP>> creator1 = () => new List<VENP>
			{
				1,
				2,
				3,
				4,
				5,
			};

			Func<List<VENP>> creator2 = () => new List<VENP>
			{
				4,
				5,
				6,
				7,
				8,
			};

			Func<List<VENP>> creator3 = () => new List<VENP>
			{
				10,
				11,
				12,
				13,
				14,
			};

			List<VENP> empty1 = new List<VENP>();
			List<VENP> empty2 = new List<VENP>();

			List<VENP> filled = new List<VENP>(creator1());
			List<VENP> filled_sameref_sameorder = new List<VENP>(filled);
			List<VENP> filled_sameref_difforder = new List<VENP>(filled).AsEnumerable().Reverse();
			List<VENP> filled_diffref_sameorder = new List<VENP>(creator1());
			List<VENP> filled_diffref_difforder = new List<VENP>(creator1()).AsEnumerable().Reverse();

			List<VENP> other = new List<VENP>(creator2());
			List<VENP> other_sameref_sameorder = new List<VENP>(other);
			List<VENP> other_sameref_difforder = new List<VENP>(other).AsEnumerable().Reverse();
			List<VENP> other_diffref_sameorder = new List<VENP>(creator2());
			List<VENP> other_diffref_difforder = new List<VENP>(creator2()).AsEnumerable().Reverse();

			List<VENP> nomatch = new List<VENP>(creator3());
			List<VENP> nomatch_sameref_sameorder = new List<VENP>(nomatch);
			List<VENP> nomatch_sameref_difforder = new List<VENP>(nomatch).AsEnumerable().Reverse();
			List<VENP> nomatch_diffref_sameorder = new List<VENP>(creator3());
			List<VENP> nomatch_diffref_difforder = new List<VENP>(creator3()).AsEnumerable().Reverse();

			List<ComparisonCase<VENP>> cases = new List<ComparisonCase<VENP>>
			{
				new ComparisonCase<VENP>(111, CollectionComparer<VENP>.Default, empty1, empty1, true),
				new ComparisonCase<VENP>(112, CollectionComparer<VENP>.Default, empty1, empty2, true),
				new ComparisonCase<VENP>(113, CollectionComparer<VENP>.Default, empty1, filled, false),
				new ComparisonCase<VENP>(121, CollectionComparer<VENP>.Default, filled, empty1, false),
				new ComparisonCase<VENP>(122, CollectionComparer<VENP>.Default, filled, filled, true),
				new ComparisonCase<VENP>(131, CollectionComparer<VENP>.Default, filled, filled_sameref_sameorder, true),
				new ComparisonCase<VENP>(132, CollectionComparer<VENP>.Default, filled, filled_sameref_difforder, false),
				new ComparisonCase<VENP>(133, CollectionComparer<VENP>.Default, filled, filled_diffref_sameorder, true),
				new ComparisonCase<VENP>(134, CollectionComparer<VENP>.Default, filled, filled_diffref_difforder, false),
				new ComparisonCase<VENP>(141, CollectionComparer<VENP>.Default, filled, other_sameref_sameorder, false),
				new ComparisonCase<VENP>(142, CollectionComparer<VENP>.Default, filled, other_sameref_difforder, false),
				new ComparisonCase<VENP>(143, CollectionComparer<VENP>.Default, filled, other_diffref_sameorder, false),
				new ComparisonCase<VENP>(144, CollectionComparer<VENP>.Default, filled, other_diffref_difforder, false),
				new ComparisonCase<VENP>(151, CollectionComparer<VENP>.Default, filled, nomatch_sameref_sameorder, false),
				new ComparisonCase<VENP>(152, CollectionComparer<VENP>.Default, filled, nomatch_sameref_difforder, false),
				new ComparisonCase<VENP>(153, CollectionComparer<VENP>.Default, filled, nomatch_diffref_sameorder, false),
				new ComparisonCase<VENP>(154, CollectionComparer<VENP>.Default, filled, nomatch_diffref_difforder, false),
				new ComparisonCase<VENP>(211, CollectionComparer<VENP>.DefaultIgnoreOrder, empty1, empty1, true),
				new ComparisonCase<VENP>(212, CollectionComparer<VENP>.DefaultIgnoreOrder, empty1, empty2, true),
				new ComparisonCase<VENP>(213, CollectionComparer<VENP>.DefaultIgnoreOrder, empty1, filled, false),
				new ComparisonCase<VENP>(221, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, empty1, false),
				new ComparisonCase<VENP>(222, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, filled, true),
				new ComparisonCase<VENP>(231, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, filled_sameref_sameorder, true),
				new ComparisonCase<VENP>(232, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, filled_sameref_difforder, true),
				new ComparisonCase<VENP>(233, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, filled_diffref_sameorder, true),
				new ComparisonCase<VENP>(234, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, filled_diffref_difforder, true),
				new ComparisonCase<VENP>(241, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, other_sameref_sameorder, false),
				new ComparisonCase<VENP>(242, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, other_sameref_difforder, false),
				new ComparisonCase<VENP>(243, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, other_diffref_sameorder, false),
				new ComparisonCase<VENP>(244, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, other_diffref_difforder, false),
				new ComparisonCase<VENP>(251, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, nomatch_sameref_sameorder, false),
				new ComparisonCase<VENP>(252, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, nomatch_sameref_difforder, false),
				new ComparisonCase<VENP>(253, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, nomatch_diffref_sameorder, false),
				new ComparisonCase<VENP>(254, CollectionComparer<VENP>.DefaultIgnoreOrder, filled, nomatch_diffref_difforder, false),
				new ComparisonCase<VENP>(311, CollectionComparer<VENP>.ReferenceEquality, empty1, empty1, true),
				new ComparisonCase<VENP>(312, CollectionComparer<VENP>.ReferenceEquality, empty1, empty2, true),
				new ComparisonCase<VENP>(313, CollectionComparer<VENP>.ReferenceEquality, empty1, filled, false),
				new ComparisonCase<VENP>(321, CollectionComparer<VENP>.ReferenceEquality, filled, empty1, false),
				new ComparisonCase<VENP>(322, CollectionComparer<VENP>.ReferenceEquality, filled, filled, true),
				new ComparisonCase<VENP>(331, CollectionComparer<VENP>.ReferenceEquality, filled, filled_sameref_sameorder, true),
				new ComparisonCase<VENP>(332, CollectionComparer<VENP>.ReferenceEquality, filled, filled_sameref_difforder, false),
				new ComparisonCase<VENP>(333, CollectionComparer<VENP>.ReferenceEquality, filled, filled_diffref_sameorder, false),
				new ComparisonCase<VENP>(334, CollectionComparer<VENP>.ReferenceEquality, filled, filled_diffref_difforder, false),
				new ComparisonCase<VENP>(341, CollectionComparer<VENP>.ReferenceEquality, filled, other_sameref_sameorder, false),
				new ComparisonCase<VENP>(342, CollectionComparer<VENP>.ReferenceEquality, filled, other_sameref_difforder, false),
				new ComparisonCase<VENP>(343, CollectionComparer<VENP>.ReferenceEquality, filled, other_diffref_sameorder, false),
				new ComparisonCase<VENP>(344, CollectionComparer<VENP>.ReferenceEquality, filled, other_diffref_difforder, false),
				new ComparisonCase<VENP>(351, CollectionComparer<VENP>.ReferenceEquality, filled, nomatch_sameref_sameorder, false),
				new ComparisonCase<VENP>(352, CollectionComparer<VENP>.ReferenceEquality, filled, nomatch_sameref_difforder, false),
				new ComparisonCase<VENP>(353, CollectionComparer<VENP>.ReferenceEquality, filled, nomatch_diffref_sameorder, false),
				new ComparisonCase<VENP>(354, CollectionComparer<VENP>.ReferenceEquality, filled, nomatch_diffref_difforder, false),
				new ComparisonCase<VENP>(411, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, empty1, empty1, true),
				new ComparisonCase<VENP>(412, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, empty1, empty2, true),
				new ComparisonCase<VENP>(413, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, empty1, filled, false),
				new ComparisonCase<VENP>(421, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, empty1, false),
				new ComparisonCase<VENP>(422, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, filled, true),
				new ComparisonCase<VENP>(431, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, filled_sameref_sameorder, true),
				new ComparisonCase<VENP>(432, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, filled_sameref_difforder, true),
				new ComparisonCase<VENP>(433, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, filled_diffref_sameorder, false),
				new ComparisonCase<VENP>(434, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, filled_diffref_difforder, false),
				new ComparisonCase<VENP>(441, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, other_sameref_sameorder, false),
				new ComparisonCase<VENP>(442, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, other_sameref_difforder, false),
				new ComparisonCase<VENP>(443, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, other_diffref_sameorder, false),
				new ComparisonCase<VENP>(444, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, other_diffref_difforder, false),
				new ComparisonCase<VENP>(451, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, nomatch_sameref_sameorder, false),
				new ComparisonCase<VENP>(452, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, nomatch_sameref_difforder, false),
				new ComparisonCase<VENP>(453, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, nomatch_diffref_sameorder, false),
				new ComparisonCase<VENP>(454, CollectionComparer<VENP>.ReferenceEqualityIgnoreOrder, filled, nomatch_diffref_difforder, false),
				new ComparisonCase<VENP>(511, new CollectionComparer<VENP>(), filled, filled_sameref_difforder, false),
				new ComparisonCase<VENP>(512, new CollectionComparer<VENP>(CollectionComparerFlags.IgnoreOrder), filled, filled_sameref_difforder, true),
				new ComparisonCase<VENP>(513, new CollectionComparer<VENP>(new EqualityComparison<VENP>((x, y) => x.Number == y.Number)), filled, filled_diffref_difforder, false),
				new ComparisonCase<VENP>(514, new CollectionComparer<VENP>((x, y) => x.Number == y.Number), filled, filled_diffref_difforder, false),
				new ComparisonCase<VENP>(515, new CollectionComparer<VENP>(CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x.Number == y.Number)), filled, filled_diffref_difforder, true),
				new ComparisonCase<VENP>(516, new CollectionComparer<VENP>(CollectionComparerFlags.IgnoreOrder, (x, y) => x.Number == y.Number), filled, filled_diffref_difforder, true),
				new ComparisonCase<VENP>(611, CollectionComparer<VENP>.Default, null, null, true),
				new ComparisonCase<VENP>(612, CollectionComparer<VENP>.Default, filled, null, false),
			};

			foreach (ComparisonCase<VENP> compCase in cases)
			{
				if (compCase.Comparer.Equals(compCase.X, compCase.Y) != compCase.ExpectedResult)
				{
					throw new TestAssertionException("CollectionComparer test failed; ID: " + compCase.Id);
				}
			}
		}

		#endregion




		#region Type: ComparisonCase

		private sealed class ComparisonCase <T>
		{
			#region Instance Constructor/Destructor

			public ComparisonCase (int id, CollectionComparer<T> comparer, List<VENP> x, List<VENP> y, bool expectedResult)
			{
				this.Id = id;
				this.Comparer = comparer;
				this.X = x;
				this.Y = y;
				this.ExpectedResult = expectedResult;
			}

			#endregion




			#region Instance Properties/Indexer

			public CollectionComparer<T> Comparer { get; private set; }

			public bool ExpectedResult { get; private set; }

			public int Id { get; private set; }

			public List<VENP> X { get; private set; }

			public List<VENP> Y { get; private set; }

			#endregion
		}

		#endregion
	}
}
