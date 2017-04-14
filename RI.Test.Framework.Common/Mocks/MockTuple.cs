namespace RI.Test.Framework.Mocks
{
	public sealed class MockTuple <T1, T2, T3, T4, T5>
	{
		#region Instance Constructor/Destructor

		public MockTuple (T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
		}

		#endregion




		#region Instance Properties/Indexer

		public T1 Item1 { get; set; }

		public T2 Item2 { get; set; }

		public T3 Item3 { get; set; }

		public T4 Item4 { get; set; }

		public T5 Item5 { get; set; }

		#endregion
	}
}
