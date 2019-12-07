using System;
using System.Collections.Generic;

using UnityEngine;




namespace RI.Framework.Utilities
{
    /// <summary>
    ///     Provides utility/extension methods for the <c> GameObject </c> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class GameObjectExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets all children of a game object.
        /// </summary>
        /// <param name="gameObject"> The game object. </param>
        /// <returns>
        ///     The array of children of the game object.
        ///     If the game object has no children, an empty array is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null. </exception>
        public static GameObject[] GetChildren (this GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            GameObject[] children = new GameObject[gameObject.transform.childCount];
            int index = 0;
            foreach (Transform transform in gameObject.transform)
            {
                children[index] = transform.gameObject;
                index++;
            }
            return children;
        }

        /// <summary>
        ///     Gets all children of a game object which satisfy a given condition.
        /// </summary>
        /// <param name="gameObject"> The game object. </param>
        /// <param name="condition"> The condition. </param>
        /// <returns>
        ///     The array of children of the game object which satisfy the condition.
        ///     If the game object has no children or no children satisfy the condition, an empty array is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> or <paramref name="condition" /> is null. </exception>
        public static GameObject[] SelectChildren (this GameObject gameObject, Predicate<GameObject> condition)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            List<GameObject> children = new List<GameObject>(gameObject.transform.childCount);
            foreach (Transform transform in gameObject.transform)
            {
                GameObject currentChildren = transform.gameObject;
                if (condition(currentChildren))
                {
                    children.Add(currentChildren);
                }
            }
            return children.ToArray();
        }

        #endregion
    }
}
