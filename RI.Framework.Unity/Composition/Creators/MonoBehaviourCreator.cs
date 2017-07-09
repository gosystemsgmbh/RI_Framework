using System;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Composition.Creators
{
	public class MonoBehaviourCreator : CompositionCreator
	{
		/// <inheritdoc />
		protected internal override bool CanCreateInstance (Type type, Type compatibleType, string exportName)
		{
			return typeof(MonoBehaviour).IsAssignableFrom(type);
		}

		/// <inheritdoc />
		protected internal override object CreateInstance (Type type, Type compatibleType, string exportName)
		{
			if (!this.CanCreateInstance(type, compatibleType, exportName))
			{
				return null;
			}

			GameObject gameObject = new GameObject();
			gameObject.name = type.Name;
			MonoBehaviour instance = gameObject.AddComponent(type) as MonoBehaviour;
			Object.DontDestroyOnLoad(gameObject);
			return instance;
		}
	}
}
