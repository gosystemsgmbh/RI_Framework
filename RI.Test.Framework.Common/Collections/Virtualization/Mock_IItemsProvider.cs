using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Collections.Virtualization;




namespace RI.Test.Framework.Collections.Virtualization
{
	public sealed class Mock_IItemsProvider : IItemsProvider<int>
	{
		private List<int> TestValues { get; set; }

		public int GetCounter { get; private set; }

		public Mock_IItemsProvider ()
		{
			this.TestValues = new List<int>
			{
				1, 2, 3, 4, 5, 6, 7, 8, 9, 10
			};

			this.GetCounter = 0;
		}

		public int GetCount() => this.TestValues.Count;

		public IEnumerable<int> GetItems(int start, int count)
		{
			this.GetCounter++;

			for (int i1 = start; i1 < Math.Min(start + count, this.TestValues.Count); i1++)
			{
				yield return this.TestValues[i1];
			}
		}

		public int Search(int item) => this.TestValues.IndexOf(item);
	}
}
