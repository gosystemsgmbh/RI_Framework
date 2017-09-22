
#if !TEMPLATE_RUNNER

using System;
using System.Threading.Tasks;

// ReSharper disable RedundantCast

namespace RI.Framework.Threading.Dispatcher
{
	public static partial class IThreadDispatcherExtensions
	{

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post (this IThreadDispatcher dispatcher, Action action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<TResult> (this IThreadDispatcher dispatcher, Func<TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post (this IThreadDispatcher dispatcher, int priority, Action action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<TResult> (this IThreadDispatcher dispatcher, int priority, Func<TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send (this IThreadDispatcher dispatcher, Action action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<TResult> (this IThreadDispatcher dispatcher, Func<TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send (this IThreadDispatcher dispatcher, int priority, Action action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<TResult> (this IThreadDispatcher dispatcher, int priority, Func<TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync (this IThreadDispatcher dispatcher, Action action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<TResult> (this IThreadDispatcher dispatcher, Func<TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync (this IThreadDispatcher dispatcher, int priority, Action action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<TResult> (this IThreadDispatcher dispatcher, int priority, Func<TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, Action action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, Action action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1> (this IThreadDispatcher dispatcher, Action<T1> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,TResult> (this IThreadDispatcher dispatcher, Func<T1,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1> (this IThreadDispatcher dispatcher, int priority, Action<T1> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1> (this IThreadDispatcher dispatcher, Action<T1> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,TResult> (this IThreadDispatcher dispatcher, Func<T1,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1> (this IThreadDispatcher dispatcher, int priority, Action<T1> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1> (this IThreadDispatcher dispatcher, Action<T1> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,TResult> (this IThreadDispatcher dispatcher, Func<T1,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1> (this IThreadDispatcher dispatcher, int priority, Action<T1> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2> (this IThreadDispatcher dispatcher, Action<T1,T2> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2> (this IThreadDispatcher dispatcher, Action<T1,T2> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2> (this IThreadDispatcher dispatcher, Action<T1,T2> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3> (this IThreadDispatcher dispatcher, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3> (this IThreadDispatcher dispatcher, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3> (this IThreadDispatcher dispatcher, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Post((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Post(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherOperation Post<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Post(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Send((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Send(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.Send(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static object Send<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.Send(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.SendAsync((Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcher.SendAsync(int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static Task<object> SendAsync<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.SendAsync(priority, options, (Delegate)action, parameters);



		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int milliseconds, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int milliseconds, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(milliseconds, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, TimeSpan delay, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);

		/// <inheritdoc cref="IThreadDispatcherExtensions.PostDelayed(IThreadDispatcher,int,int,ThreadDispatcherOptions,Delegate,object[])"/>
		public static ThreadDispatcherTimer PostDelayed<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult> action, params object[] parameters) => dispatcher.PostDelayed(delay, priority, options, (Delegate)action, parameters);
	}
}

#endif