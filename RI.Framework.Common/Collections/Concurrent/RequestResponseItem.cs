using System;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Concurrent
{
    public sealed class RequestResponseItem<TRequest, TResponse> : ISynchronizable, IDisposable
    {
        private TResponse _response;

        internal RequestResponseItem(TaskCreationOptions completionCreationOptions, TRequest request)
        {
            this.SyncRoot = new object();
            this.CompletionCreationOptions = completionCreationOptions;
            this.Request = request;
            this.Response = default(TResponse);
            this.ResponseCompletion = new TaskCompletionSource<TResponse>(request, this.CompletionCreationOptions);
        }

        ~RequestResponseItem () => this.Cancel();

        void IDisposable.Dispose() => this.Respond();

        bool ISynchronizable.IsSynchronized => true;

        public TaskCreationOptions CompletionCreationOptions { get; }

        public object SyncRoot { get; }

        public TRequest Request { get; }

        private TaskCompletionSource<TResponse> ResponseCompletion { get; }

        internal Task<TResponse> ResponseTask => this.ResponseCompletion.Task;

        public TResponse Response
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._response;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._response = value;
                }
            }
        }

        public void Respond ()
        {
            lock (this.SyncRoot)
            {
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetResult(this.Response);
            }
        }

        public void Respond (TResponse response)
        {
            lock (this.SyncRoot)
            {
                this.Response = response;
                this.ResponseCompletion.TrySetResult(response);
            }
        }

        public void Cancel ()
        {
            lock (this.SyncRoot)
            {
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetCanceled();
            }
        }

        public void Abort (Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (this.SyncRoot)
            {
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetException(exception);
            }
        }
    }
}