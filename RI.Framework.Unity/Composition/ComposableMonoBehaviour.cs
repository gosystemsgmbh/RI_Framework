using System;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Extends <c> MonoBehaviour </c> so that it can be used as a base class for composable types.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         Instances of <see cref="ComposableMonoBehaviour" />s are not created using their constructor (as this would be the wrong way how to instantiate anything <c> MonoBehaviour </c>).
	///         Instead, <see cref="CreateInstance" /> is used.
	///     </note>
	/// </remarks>
	[Export]
	public abstract class ComposableMonoBehaviour : MonoBehaviour, ILogSource
	{
		#region Static Methods

		/// <summary>
		///     Creates an instance of the specified <see cref="ComposableMonoBehaviour" /> type.
		/// </summary>
		/// <param name="type"> The type of which an instance is to be created. </param>
		/// <returns> The created instance. </returns>
		/// <remarks>
		///     <para>
		///         To instantiate a <see cref="ComposableMonoBehaviour" />, a new <c> GameObject </c> is created to which the <see cref="ComposableMonoBehaviour" /> is added as a component using <c> AddComponent </c>.
		///         The created <c> GameObject </c> has also called <c> Object.DontDestroyOnLoad </c> on it.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		[ExportCreator]
		public static ComposableMonoBehaviour CreateInstance (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			GameObject gameObject = new GameObject();
			gameObject.name = type.Name;
			ComposableMonoBehaviour instance = gameObject.AddComponent(type) as ComposableMonoBehaviour;
			Object.DontDestroyOnLoad(gameObject);
			return instance;
		}


		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		/// <inheritdoc />
		public Utilities.Logging.ILogger Logger { get; set; } = LogLocator.Logger;

		#endregion
	}
}
