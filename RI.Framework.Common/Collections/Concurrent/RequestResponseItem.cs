using System;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Concurrent
{
    public sealed class RequestResponseItem<TRequest, TResponse> : ISynchronizable, IDisposable
    {
        private TResponse _response;

        private bool _stillNeeded;

        internal RequestResponseItem(TaskCreationOptions completionCreationOptions, TRequest request)
        {
            this.SyncRoot = new object();

            this.CompletionCreationOptions = completionCreationOptions;
            this.Request = request;

            this.StillNeeded = true;
            this.Response = default(TResponse);

            this.ResponseCompletion = new TaskCompletionSource<TResponse>(request, this.CompletionCreationOptions);
            this.CancellationTokenSource = new CancellationTokenSource();
        }

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

        public bool StillNeeded
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._stillNeeded;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._stillNeeded = value;
                }
            }
        }

        private CancellationTokenSource CancellationTokenSource { get; }

        public CancellationToken CancelationToken => this.CancellationTokenSource.Token;

        public void Respond ()
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetResult(this.Response);
            }
        }

        public void Respond (TResponse response)
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
                this.Response = response;
                this.ResponseCompletion.TrySetResult(response);
            }
        }

        public void Cancel ()
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
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
                this.StillNeeded = false;
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetException(exception);
            }
        }

        internal void NoLongerNeeded ()
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
                this.Response = default(TResponse);
                this.CancellationTokenSource.Cancel();
            }
        }
    }
}