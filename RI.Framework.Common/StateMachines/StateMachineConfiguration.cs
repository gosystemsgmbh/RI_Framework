namespace RI.Framework.StateMachines
{
	public sealed class StateMachineConfiguration
	{
		public StateMachineConfiguration ()
		{
			this.Dispatcher = null;
			this.Resolver = null;

			this.Cache = new StateCache();

			this.EnableAutomaticCaching = true;
		}

		public IStateDispatcher Dispatcher { get; set; }

		public IStateResolver Resolver { get; set; }

		public IStateCache Cache { get; set; }

		public bool EnableAutomaticCaching { get; set; }
	}
}