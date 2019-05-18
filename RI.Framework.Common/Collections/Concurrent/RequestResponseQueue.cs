using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Concurrent
{
    public sealed class RequestResponseQueue <TRequest, TResponse> : ISynchronizable
    {
        #region Instance Constructor/Destructor

        public RequestResponseQueue ()
            : this(TaskCreationOptions.RunContinuationsAsynchronously)
        {
        }

        public RequestResponseQueue (TaskCreationOptions completionCreationOptions)
        {
            this.SyncRoot = new object();

            this.CompletionCreationOptions = completionCreationOptions;

            this.Requests = new List<RequestResponseItem<TRequest, TResponse>>();
            this.Consumers = new List<TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>>();
        }

        #endregion




        #region Instance Properties/Indexer

        public TaskCreationOptions CompletionCreationOptions { get; }

        public int WaitingConsumers
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Consumers.Count;
                }
            }
        }

        public int WaitingRequests
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Requests.Count;
                }
            }
        }

        private List<TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>> Consumers { get; }

        private List<RequestResponseItem<TRequest, TResponse>> Requests { get; }

        #endregion




        #region Instance Methods

        public int AbortRequests (Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (this.SyncRoot)
            {
                int requests = this.Requests.Count;
                this.Requests.ForEach(x => x.Abort(exception));
                this.Requests.Clear();
                return requests;
            }
        }

        public int CancelRequests ()
        {
            lock (this.SyncRoot)
            {
                int requests = this.Requests.Count;
                this.Requests.ForEach(x => x.Cancel());
                this.Requests.Clear();
                return requests;
            }
        }

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync () => this.DequeueAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync (TimeSpan timeout) => this.DequeueAsync(timeout, CancellationToken.None);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync (CancellationToken ct) => this.DequeueAsync(Timeout.InfiniteTimeSpan, ct);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync (TimeSpan timeout, CancellationToken ct)
        {
            TaskCompletionSource<RequestResponseItem<TRequest, TResponse>> tcs;
            Task<RequestResponseItem<TRequest, TResponse>> consumerTask;

            lock (this.SyncRoot)
            {
                while (this.Requests.Count > 0)
                {
                    RequestResponseItem<TRequest, TResponse> item = this.Requests[0];
                    this.Requests.RemoveAt(0);

                    if (item.StillNeeded)
                    {
                        return Task.FromResult(item);
                    }
                }

                tcs = new TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>(this.CompletionCreationOptions);
                consumerTask = tcs.Task;

                this.Consumers.Add(tcs);
            }

            Task timeoutTask = Task.Delay(timeout);
            Task cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, ct);

            Task<Task> result = Task.WhenAny(consumerTask, timeoutTask, cancelTask);
            Task<RequestResponseItem<TRequest, TResponse>> waitTask = result.ContinueWith(task =>
            {
                lock (this.SyncRoot)
                {
                    this.Consumers.Remove(tcs);
                    if (consumerTask.IsCompleted)
                    {
                        return consumerTask.Result;
                    }

                    if (object.ReferenceEquals(result.Result, timeoutTask))
                    {
                        throw new TimeoutException();
                    }

                    if (object.ReferenceEquals(result.Result, cancelTask))
                    {
                        throw new OperationCanceledException();
                    }

                    return consumerTask.Result;
                }
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation);

            return waitTask;
        }

        public int DismissConsumers ()
        {
            lock (this.SyncRoot)
            {
                int consumers = this.Consumers.Count;
                this.Consumers.ForEach(x => x.TrySetCanceled());
                this.Consumers.Clear();
                return consumers;
            }
        }

        public Task<TResponse> EnqueueAsync (TRequest request) => this.EnqueueAsync(request, Timeout.InfiniteTimeSpan, CancellationToken.None);

        public Task<TResponse> EnqueueAsync (TRequest request, TimeSpan timeout) => this.EnqueueAsync(request, timeout, CancellationToken.None);

        public Task<TResponse> EnqueueAsync (TRequest request, CancellationToken ct) => this.EnqueueAsync(request, Timeout.InfiniteTimeSpan, ct);

        public Task<TResponse> EnqueueAsync (TRequest request, TimeSpan timeout, CancellationToken ct)
        {
            RequestResponseItem<TRequest, TResponse> item;
            Task<TResponse> responseTask;

            lock (this.SyncRoot)
            {
                item = new RequestResponseItem<TRequest, TResponse>(this.CompletionCreationOptions, request);
                responseTask = item.ResponseTask;

                bool alreadyHandedToConsumer = false;

                if (this.Consumers.Count > 0)
                {
                    alreadyHandedToConsumer = this.Consumers[0].TrySetResult(item);
                    this.Consumers.RemoveAt(0);
                }

                if (!alreadyHandedToConsumer)
                {
                    this.Requests.Add(item);
                }
            }

            Task timeoutTask = Task.Delay(timeout);
            Task cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, ct);

            Task<Task> result = Task.WhenAny(responseTask, timeoutTask, cancelTask);
            Task<TResponse> waitTask = result.ContinueWith(task =>
            {
                lock (this.SyncRoot)
                {
                    this.Requests.Remove(item);
                    if (responseTask.IsCompleted)
                    {
                        return responseTask.Result;
                    }

                    if (object.ReferenceEquals(result.Result, timeoutTask))
                    {
                        item.NoLongerNeeded();
                        throw new TimeoutException();
                    }

                    if (object.ReferenceEquals(result.Result, cancelTask))
                    {
                        item.NoLongerNeeded();
                        throw new OperationCanceledException();
                    }

                    return responseTask.Result;
                }
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation);

            return waitTask;
        }

        public int RespondRequests (TResponse response)
        {
            lock (this.SyncRoot)
            {
                int requests = this.Requests.Count;
                this.Requests.ForEach(x => x.Respond(response));
                this.Requests.Clear();
                return requests;
            }
        }

        public int RespondRequests ()
        {
            lock (this.SyncRoot)
            {
                int requests = this.Requests.Count;
                this.Requests.ForEach(x => x.Respond());
                this.Requests.Clear();
                return requests;
            }
        }

        #endregion




        #region Interface: ISynchronizable

        bool ISynchronizable.IsSynchronized => true;

        public object SyncRoot { get; }

        #endregion
    }
}
