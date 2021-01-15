using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition
{
    /// <summary>
    ///     Provides extension methods to allow simple usage of <see cref="CompositionContainer" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="CompositionContainer" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public static class CompositionExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Exports an object under its default name using the composition containers singleton instance.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <remarks>
        ///     <para>
        ///         <typeparamref name="T" /> is used to determine the default name.
        ///     </para>
        ///     <para>
        ///         <see cref="CompositionContainer.CreateSingletonWithEverything" /> is used to retrieve or create the composition containers singleton instance.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,Type)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            CompositionContainer.CreateSingletonWithEverything().AddInstance(instance, typeof(T));
        }

        /// <summary>
        ///     Exports an object under the specified types default name using the composition containers singleton instance.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <param name="exportType"> The type under whose default name the object is exported. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="CompositionContainer.CreateSingletonWithEverything" /> is used to retrieve or create the composition containers singleton instance.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,Type)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance, Type exportType)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            CompositionContainer.CreateSingletonWithEverything().AddInstance(instance, exportType);
        }

        /// <summary>
        ///     Exports an object under the specified name using the composition containers singleton instance.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <param name="exportName"> The name under which the object is exported. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="CompositionContainer.CreateSingletonWithEverything" /> is used to retrieve or create the composition containers singleton instance.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,string)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance, string exportName)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportName == null)
            {
                throw new ArgumentNullException(nameof(exportName));
            }

            if (exportName.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(exportName));
            }

            CompositionContainer.CreateSingletonWithEverything().AddInstance(instance, exportName);
        }

        /// <summary>
        ///     Exports an object under its default name using a specified composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <param name="container"> The used composition container. </param>
        /// <remarks>
        ///     <para>
        ///         <typeparamref name="T" /> is used to determine the default name.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,Type)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" />, or <paramref name="container" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance, CompositionContainer container)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AddInstance(instance, typeof(T));
        }

        /// <summary>
        ///     Exports an object under the specified types default name using a specified composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <param name="exportType"> The type under whose default name the object is exported. </param>
        /// <param name="container"> The used composition container. </param>
        /// <remarks>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,Type)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" />, <paramref name="exportType" />, or <paramref name="container" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance, Type exportType, CompositionContainer container)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AddInstance(instance, exportType);
        }

        /// <summary>
        ///     Exports an object under the specified name using a specified composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="instance"> The object to export. </param>
        /// <param name="exportName"> The name under which the object is exported. </param>
        /// <param name="container"> The used composition container. </param>
        /// <remarks>
        ///     <para>
        ///         See <see cref="CompositionContainer.AddInstance(object,string)" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="instance" />, <paramref name="exportName" />, or <paramref name="container" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public static void Export <T> (this T instance, string exportName, CompositionContainer container)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportName == null)
            {
                throw new ArgumentNullException(nameof(exportName));
            }

            if (exportName.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(exportName));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AddInstance(instance, exportName);
        }

        /// <summary>
        ///     Resolves all imports of an object using the composition containers singleton instance.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="obj"> The object whose imports are resolved. </param>
        /// <returns>
        ///     true if any of the imports of the object were resolved or updated, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" /> is used.
        ///     </para>
        ///     <para>
        ///         <see cref="CompositionContainer.CreateSingletonWithEverything" /> is used to retrieve or create the composition containers singleton instance.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.ResolveImports" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="obj" /> is not a type which can be composed. </exception>
        /// <exception cref="CompositionException"> The imports for <paramref name="obj" /> cannot be resolved. </exception>
        public static bool Import <T> (this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return CompositionContainer.CreateSingletonWithEverything().ResolveImports(obj, CompositionFlags.Normal);
        }

        /// <summary>
        ///     Resolves all imports of an object using the composition containers singleton instance.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="obj"> The object whose imports are resolved. </param>
        /// <param name="composition"> The composition type which is going to be done on the object. </param>
        /// <returns>
        ///     true if any of the imports of the object were resolved or updated, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="CompositionContainer.CreateSingletonWithEverything" /> is used to retrieve or create the composition containers singleton instance.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.ResolveImports" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="obj" /> is not a type which can be composed. </exception>
        /// <exception cref="CompositionException"> The imports for <paramref name="obj" /> cannot be resolved. </exception>
        public static bool Import <T> (this T obj, CompositionFlags composition)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return CompositionContainer.CreateSingletonWithEverything().ResolveImports(obj, composition);
        }

        /// <summary>
        ///     Resolves all imports of an object using a specified composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="obj"> The object whose imports are resolved. </param>
        /// <param name="container"> The used composition container. </param>
        /// <returns>
        ///     true if any of the imports of the object were resolved or updated, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" /> is used.
        ///     </para>
        ///     <para>
        ///         See <see cref="CompositionContainer.ResolveImports" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> or <paramref name="container" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="obj" /> is not a type which can be composed. </exception>
        /// <exception cref="CompositionException"> The imports for <paramref name="obj" /> cannot be resolved. </exception>
        public static bool Import <T> (this T obj, CompositionContainer container)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return container.ResolveImports(obj, CompositionFlags.Normal);
        }

        /// <summary>
        ///     Resolves all imports of an object using a specified composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="obj"> The object whose imports are resolved. </param>
        /// <param name="composition"> The composition type which is going to be done on the object. </param>
        /// <param name="container"> The used composition container. </param>
        /// <returns>
        ///     true if any of the imports of the object were resolved or updated, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         See <see cref="CompositionContainer.ResolveImports" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> or <paramref name="container" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="obj" /> is not a type which can be composed. </exception>
        /// <exception cref="CompositionException"> The imports for <paramref name="obj" /> cannot be resolved. </exception>
        public static bool Import <T> (this T obj, CompositionFlags composition, CompositionContainer container)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return container.ResolveImports(obj, composition);
        }

        #endregion
    }
}
