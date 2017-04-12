using System;

namespace RI.Framework.Utilities.ObjectModel
{
	/// <summary>
	/// Provides a centralized functionality to implement singletons.
	/// </summary>
	/// <typeparam name="T">The singleton type.</typeparam>
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // gets an existing singleton or null if it does not exist
	/// var playerManager = Singleton<PlayerManager>.Instance;
	/// 
	/// // sets a new or replaces an existing singleton
	/// Singleton<PlayerManager>.Instance = new PlayerManager();
	/// 
	/// // gets an existing singleton or creates one if it does not exist
	/// // (might throw an exception if the type has no default constructor)
	/// var gameRules = Singleton<GameRules>.Ensure();
	/// 
	/// // gets an existing singleton or gets one from a callback if it does not exist
	/// var enemyManager = Singleton<EnemyManager>.Ensure(() => new EnemyManager());
	/// ]]>
	/// </code>
	/// </example>
	public static class Singleton<T>
		where T : class
	{
		/// <summary>
		/// Gets or sets the current instance of the singleton.
		/// </summary>
		/// <value>
		/// The current instance of the singleton or null if there is no current instance set.
		/// </value>
		public static T Instance { get; set; }

		/// <summary>
		/// Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <returns>
		/// The current instance of the singleton.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method uses <see cref="Activator"/> to create a new instance of the singleton type if there is currently no instance set.
		/// </para>
		/// </remarks>
		public static T Ensure ()
		{
			if (Singleton<T>.Instance == null)
			{
				Singleton<T>.Instance = Activator.CreateInstance<T>();
			}
			return Singleton<T>.Instance;
		}

		/// <summary>
		/// Ensures that there is a current instance of the singleton.
		/// </summary>
		/// <param name="creator">The creator delegate used to create the singleton.</param>
		/// <returns>
		/// The current instance of the singleton.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method uses the delegate specified by <paramref name="creator"/> to create a new instance of the singleton type if there is currently no instance set.
		/// This is useful in cases the singleton types constructor has parameters or can only be created through a static method.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="creator"/> is null.</exception>
		/// <exception cref="NotSupportedException"><paramref name="creator"/> did not return a new instance of the singleton type.</exception>
		public static T Ensure (Func<T> creator)
		{
			if (Singleton<T>.Instance == null)
			{
				Singleton<T>.Instance = creator();
				if (Singleton<T>.Instance == null)
				{
					throw new NotSupportedException("The creator delegate did not return a new instance of the singleton delegate.");
				}
			}
			return Singleton<T>.Instance;
		}
	}
}
