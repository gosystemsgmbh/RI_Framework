using System;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Composition.Creators
{
	/// <summary>
	///     Implements a composition creator which creates instances of <c> MonoBehaviour </c> types.
	/// </summary>
	/// <remarks>
	///     <para>
	///     </para>
	///     <para>
	///         <c> MonoBehaviour </c>s are not created using their constructor (as this would be the wrong way how to instantiate anything <c> MonoBehaviour </c>).
	///         Instead, to instantiate a <c> MonoBehaviour </c>, a new <c> GameObject </c> is created to which the <c> MonoBehaviour </c> is added as a component using <c> AddComponent </c>.
	///         The created <c> GameObject </c> has also called <c> Object.DontDestroyOnLoad </c> on it.
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	public class MonoBehaviourCreator : CompositionCreator
	{
		#region Overrides

		/// <inheritdoc />
		protected internal override bool CanCreateInstance (CompositionContainer container, Type type, Type compatibleType, string exportName)
		{
			return typeof(MonoBehaviour).IsAssignableFrom(type);
		}

		/// <inheritdoc />
		protected internal override object CreateInstance (CompositionContainer container, Type type, Type compatibleType, string exportName)
		{
			if (!this.CanCreateInstance(container, type, compatibleType, exportName))
			{
				return null;
			}

			GameObject gameObject = new GameObject();
			gameObject.name = type.Name;
			MonoBehaviour instance = gameObject.AddComponent(type) as MonoBehaviour;
			Object.DontDestroyOnLoad(gameObject);
			return instance;
		}

		#endregion
	}
}
