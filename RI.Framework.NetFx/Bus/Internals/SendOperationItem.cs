using System.Threading.Tasks;

namespace RI.Framework.Bus.Internals
{
	public sealed class SendOperationItem
	{
		public SendOperationItem(SendOperation sendOperation)
		{
			this.SendOperation = sendOperation;
			this.Task = new TaskCompletionSource<object>(this.SendOperation, TaskCreationOptions.DenyChildAttach);
		}

		public SendOperation SendOperation { get; }

		public TaskCompletionSource<object> Task { get; }
	}
}