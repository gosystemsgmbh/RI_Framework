using System;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Concurrent
{
    public sealed class RequestResponseItem <TRequest, TResponse> : ISynchronizable, IDisposable
    {
        #region Instance Constructor/Destructor

        internal RequestResponseItem (TaskCreationOptions completionCreationOptions, TRequest request)
        {
            this.SyncRoot = new object();

            this.CompletionCreationOptions = completionCreationOptions;
            this.Request = request;

            this.StillNeeded = true;
            this.Response = default(TResponse);

            this.ResponseCompletion = new TaskCompletionSource<TResponse>(request, this.CompletionCreationOptions);
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        #endregion




        #region Instance Fields

        private TResponse _response;

        private bool _stillNeeded;

        #endregion




        #region Instance Properties/Indexer

        public CancellationToken CancelationToken => this.CancellationTokenSource.Token;

        public TaskCreationOptions CompletionCreationOptions { get; }

        public TRequest Request { get; }

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

        internal Task<TResponse> ResponseTask => this.ResponseCompletion.Task;

        private CancellationTokenSource CancellationTokenSource { get; }

        private TaskCompletionSource<TResponse> ResponseCompletion { get; }

        #endregion




        #region Instance Methods

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

        public void Cancel ()
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
                this.Response = default(TResponse);
                this.ResponseCompletion.TrySetCanceled();
            }
        }

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

        internal void NoLongerNeeded ()
        {
            lock (this.SyncRoot)
            {
                this.StillNeeded = false;
                this.Response = default(TResponse);
                this.CancellationTokenSource.Cancel();
            }
        }

        #endregion




        #region Interface: IDisposable

        void IDisposable.Dispose () => this.Respond();

        #endregion




        #region Interface: ISynchronizable

        bool ISynchronizable.IsSynchronized => true;

        public object SyncRoot { get; }

        #endregion
    }
}
