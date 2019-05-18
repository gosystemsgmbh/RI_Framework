using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections.Generic;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Concurrent
{
    public sealed class RequestResponseQueue<TRequest, TResponse> : ISynchronizable
    {
        public RequestResponseQueue ()
        : this(TaskCreationOptions.RunContinuationsAsynchronously)
        {
        }

        public RequestResponseQueue (TaskCreationOptions completionCreationOptions)
        {
            this.SyncRoot = new object();
            this.CompletionCreationOptions = completionCreationOptions;
            this.Queue = new Queue<RequestResponseItem<TRequest, TResponse>>();
            this.WakeUps = new Queue<TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>>();
        }

        bool ISynchronizable.IsSynchronized => true;

        public object SyncRoot { get; }

        public TaskCreationOptions CompletionCreationOptions { get; }

        private Queue<RequestResponseItem<TRequest, TResponse>> Queue { get; }

        private Queue<TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>> WakeUps { get; }

        public int WaitingRequests
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Queue.Count;
                }
            }
        }

        public int WaitingConsumers
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.WakeUps.Count;
                }
            }
        }

        public Task<TResponse> EnqueueAsync (TRequest request) => this.EnqueueAsync(request, Timeout.InfiniteTimeSpan, CancellationToken.None);

        public Task<TResponse> EnqueueAsync(TRequest request, TimeSpan timeout) => this.EnqueueAsync(request, timeout, CancellationToken.None);

        public Task<TResponse> EnqueueAsync(TRequest request, CancellationToken ct) => this.EnqueueAsync(request, Timeout.InfiniteTimeSpan, ct);

        public Task<TResponse> EnqueueAsync (TRequest request, TimeSpan timeout, CancellationToken ct)
        {
            Task<TResponse> responseTask;

            lock (this.SyncRoot)
            {
                RequestResponseItem<TRequest, TResponse> item = new RequestResponseItem<TRequest, TResponse>(this.CompletionCreationOptions, request);
                responseTask = item.ResponseTask;

                if (this.WakeUps.Count > 0)
                {
                    this.WakeUps.Dequeue().SetResult(item);
                }
                else
                {
                    this.Queue.Enqueue(item);
                }
            }

            Task timeoutTask = Task.Delay(timeout);
            Task cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, ct);

            Task<Task> result = Task.WhenAny(responseTask, timeoutTask, cancelTask);
            Task<TResponse> waitTask = result.ContinueWith<TResponse>(task =>
            {
                if (object.ReferenceEquals(result.Result, timeoutTask))
                {
                    throw new TimeoutException();
                }
                if (object.ReferenceEquals(result.Result, cancelTask))
                {
                    throw new OperationCanceledException();
                }
                return responseTask.Result;

            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation);

            return waitTask;
        }

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync () => this.DequeueAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync(TimeSpan timeout) => this.DequeueAsync(timeout, CancellationToken.None);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync(CancellationToken ct) => this.DequeueAsync(Timeout.InfiniteTimeSpan, ct);

        public Task<RequestResponseItem<TRequest, TResponse>> DequeueAsync (TimeSpan timeout, CancellationToken ct)
        {
            Task<RequestResponseItem<TRequest, TResponse>> wakeUpTask;

            lock (this.SyncRoot)
            {
                if (this.Queue.Count > 0)
                {
                    RequestResponseItem<TRequest, TResponse> item = this.Queue.Dequeue();
                    return Task.FromResult(item);
                }

                TaskCompletionSource<RequestResponseItem<TRequest, TResponse>> tcs = new TaskCompletionSource<RequestResponseItem<TRequest, TResponse>>(this.CompletionCreationOptions);
                wakeUpTask = tcs.Task;
                this.WakeUps.Enqueue(tcs);
            }

            Task timeoutTask = Task.Delay(timeout);
            Task cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, ct);

            Task<Task> result = Task.WhenAny(wakeUpTask, timeoutTask, cancelTask);
            Task<RequestResponseItem<TRequest, TResponse>> waitTask = result.ContinueWith<RequestResponseItem<TRequest, TResponse>>(task =>
            {
                if (object.ReferenceEquals(result.Result, timeoutTask))
                {
                    throw new TimeoutException();
                }
                if (object.ReferenceEquals(result.Result, cancelTask))
                {
                    throw new OperationCanceledException();
                }
                return wakeUpTask.Result;

            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation);

            return waitTask;
        }

        public bool RespondRequests (TResponse response)
        {
            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.Queue.Count > 0)
                {
                    dequeued = true;
                    this.Queue.Dequeue().Respond(response);
                }
                return dequeued;
            }
        }

        public bool RespondRequests()
        {
            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.Queue.Count > 0)
                {
                    dequeued = true;
                    this.Queue.Dequeue().Respond();
                }
                return dequeued;
            }
        }

        public bool CancelRequests()
        {
            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.Queue.Count > 0)
                {
                    dequeued = true;
                    this.Queue.Dequeue().Cancel();
                }
                return dequeued;
            }
        }

        public bool AbortRequests(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.Queue.Count > 0)
                {
                    dequeued = true;
                    this.Queue.Dequeue().Abort(exception);
                }
                return dequeued;
            }
        }

        public bool DismissConsumers ()
        {
            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.WakeUps.Count > 0)
                {
                    dequeued = true;
                    this.WakeUps.Dequeue().TrySetCanceled();
                }
                return dequeued;
            }
        }

        public bool DismissConsumers (Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (this.SyncRoot)
            {
                bool dequeued = false;
                while (this.WakeUps.Count > 0)
                {
                    dequeued = true;
                    this.WakeUps.Dequeue().TrySetException(exception);
                }
                return dequeued;
            }
        }
    }
}
