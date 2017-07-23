using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Services;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;




#if PLATFORM_NETFX
using System.Collections;
using System.Linq.Expressions;
#endif


namespace RI.Framework.Composition
{
	/// <summary>
	///     The main hub for doing composition.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <b> GENERAL </b>
	///     </para>
	///     <para>
	///         Basically, &quot;composition&quot; means to resolve imports according to the known exports of a <see cref="CompositionContainer" />.
	///         The term &quot;container&quot; in <see cref="CompositionContainer" /> already indicates that the exports are contained in the <see cref="CompositionContainer" /> and taken from there to resolve the imports.
	///     </para>
	///     <para>
	///         <b> EXPORTS &amp; IMPORTS </b>
	///     </para>
	///     <para>
	///         The terms &quot;export&quot; and &quot;import&quot; might be confusing at first.
	///         But it is simple:
	///     </para>
	///     <para>
	///         &quot;Export&quot; means that a type or object is being &quot;exported for use&quot; or &quot;provided to a <see cref="CompositionContainer" /> in some way&quot; (manual or model-based export) so that the <see cref="CompositionContainer" /> can resolve the imports.
	///     </para>
	///     <para>
	///         &quot;Import&quot; means that &quot;a value of a given type or name is required or requested in some way&quot; (manual or model-based import) and the <see cref="CompositionContainer" /> provides that value (or &quot;imports it for use&quot;) by searching its known exports for an export of the same name as the import.
	///     </para>
	///     <para>
	///         The search and provisioning of exports (of the same name as the required import) is called &quot;resolving imports&quot; or simply &quot;resolving&quot;.
	///     </para>
	///     <para>
	///         <b> NAMES </b>
	///     </para>
	///     <para>
	///         What also might be unclear at first is the &quot;name&quot; of imports and exports.
	///         Especially because sometimes a type and sometimes a name is mentioned/used.
	///         When exports are managed by a <see cref="CompositionContainer" />, or a <see cref="CompositionCatalog" />, they are always identified using their name.
	///         Resolving of imports is also always done using the name of the import.
	///         Now, when types are used instead of names, the types are simply translated into what is called &quot;the types default name&quot;.
	///         After the translation, the types are ignored and the exports and imports are continued to be handled using their translated names.
	///         For example, the method <see cref="AddExport(object, Type)" /> does nothing else than determine the types default name and then call <see cref="AddExport(object, string)" /> with that name.
	///         This allows you to mix type-based import and export (using their types default names) and also name-based import and export (using any custom names you specify).
	///         Finally, a types default name is simply its namespace and type name, e.g. the name &quot;RI.Framework.Composition.CompositionContainer&quot; for the type <see cref="CompositionContainer" />.
	///     </para>
	///     <para>
	///         Note that names are always case-sensitive.
	///     </para>
	///     <para>
	///         <b> EXPORT COMPOSITION &amp; DEPENDENCY INJECTION (DI) </b>
	///     </para>
	///     <para>
	///         A powerful aspect of the <see cref="CompositionContainer" /> is the fact that all its known/contained exports are composed themselves.
	///         This means that all exports of a <see cref="CompositionContainer" /> can have imports themselves (model-based imports using <see cref="ImportAttribute" />) and that those imports are automatically resolved by the <see cref="CompositionContainer" />.
	///         In other words, when getting an import from a <see cref="CompositionContainer" />, all the imports of the import itself (if any) will be resolved as well (if possible).
	///     </para>
	///     <para>
	///         This is how Dependency Injection is implemented using <see cref="CompositionContainer" />.
	///         If you have a cascade of objects, with all kind of dependencies, you do not need to resolve them all by yourself by creating instances, dealing with singletons, or getting manual imports.
	///         You just make the <see cref="CompositionContainer" /> aware where to find all the possibly required objects or types (using the various concrete implementations of <see cref="CompositionCatalog" />) and then start to pull the objects from the <see cref="CompositionContainer" /> as you need them.
	///     </para>
	///     <para>
	///         It is a dangerous comparison but you could view a <see cref="CompositionContainer" /> as a kind of very advanced singleton manager which also resolves the dependencies of its managed singletons.
	///     </para>
	///     <para>
	///         <b> MANUAL &amp; MODEL-BASED EXPORTING </b>
	///     </para>
	///     <para>
	///         There are two ways of exporting: Manual and model-based.
	///     </para>
	///     <para>
	///         Manual export is done by calling one of the <c> AddExport </c> methods explicitly and stating a type or object and under which name it is exported.
	///         Advantage: A type or object does not need any special preparation in order to be exported manually, any type or object can be exported (restrictions apply, see below), even those which requires complex construction which could not be handled by a <see cref="CompositionContainer" />.
	///         Disadvantage: The type or object to be manually exported must be known and explicitly added to the <see cref="CompositionContainer" />, adding a strong dependency to that type or object and/or a lot of boilerplate code just to discover the type or object.
	///     </para>
	///     <para>
	///         Model-based export is done by using a <see cref="CompositionCatalog" /> and adding it to the <see cref="CompositionContainer" /> using the <see cref="AddCatalog(CompositionCatalog)" /> method.
	///         Advantage: No dependencies or references to the exported types or objects are required at compile time because, depending on the used <see cref="CompositionCatalog" />, the composition catalog can collect all exports by itself (e.g. all eligible types in an <see cref="Assembly" /> when using <see cref="AssemblyCatalog" />).
	///         Disadvantage: A type or object needs special preparation in order to be model-based exported, namely at least one or more <see cref="ExportAttribute" /> applied to it (for some <see cref="CompositionCatalog" />s) or simple constructors.
	///     </para>
	///     <para>
	///         <b> TYPE &amp; OBJECT EXPORTS </b>
	///     </para>
	///     <para>
	///         Two things can be exported: Types and objects.
	///         &quot;Types or objects&quot; means that either a <see cref="Type" /> or an already instantiated <see cref="object" /> of any type can be used.
	///     </para>
	///     <para>
	///         A type can be exported by specifying the <see cref="Type" /> and under which name it is exported.
	///         When an import is resolved to such a type export, a new instance of that type is created (if not yet created) or the previously created instance of that type is used and provided as the import value (except for private exports, see below).
	///         A type can be exported multiple times under different names.
	///         It is important to know that the same type is only instantiated once in a <see cref="CompositionContainer" />.
	///         That means that the one instance of a particular type is used for all exports of that type, even if exported under different names.
	///         Therefore, type exports are always shared, singleton-like exports (except for private exports, see below).
	///         <see cref="ExportConstructorAttribute" /> and <see cref="ExportCreatorAttribute" /> are used to help with the construction of instances for type exports.
	///         <see cref="ExportCreatorAttribute" />s have higher priority than <see cref="ExportConstructorAttribute" />s when determining how an instance of a type export is to be created.
	///         Only if the <see cref="ExportCreatorAttribute" />s yield no usable results or are not used, <see cref="ExportConstructorAttribute" /> is used.
	///     </para>
	///     <para>
	///         An object can be exported by specifying the <see cref="object" /> and under which name it is exported.
	///         When an import is resolved to such an object export, the specified object itself is provided as the import value.
	///         An object can be exported multiple times under different names.
	///         Unlike type exports, object exports using different instances of the same type are possible.
	///         However, object exports cannot be private exports (see below).
	///     </para>
	///     <para>
	///         Although type exports share one instance for the same type, it is possible to have type exports of a particular type and also one or more object exports with instances of that same type.
	///         In such cases, a new shared instance for the type export is still created although there are object exports with instances of the same type.
	///         Or in other words: Type exports and object exports do not share their instances.
	///     </para>
	///     <para>
	///         <b> SHARED &amp; PRIVATE EXPORTS </b>
	///     </para>
	///     <para>
	///         Another distinction for exporting is shared and private exports.
	///     </para>
	///     <para>
	///         Shared exports behave exactly like described above.
	///         For type exports, the instance which is created for the type is used (shared) for all imports of the same type.
	///         For object exports, the instance is used (shared) for all imports of the same name.
	///     </para>
	///     <para>
	///         Private exports behave more or less also the same as described above, with one exception:
	///         For type exports, imports will receive their own (private) instance each time (!) an import is resolved.
	///         Object exports cannot be private and are always shared.
	///     </para>
	///     <para>
	///         An export can be made private through the <see cref="ExportAttribute.Private" /> property of <see cref="ExportAttribute" />.
	///     </para>
	///     <para>
	///         <b> MANUAL &amp; MODEL-BASED IMPORTING </b>
	///     </para>
	///     <para>
	///         There are two ways of importing: Manual and model-based.
	///     </para>
	///     <para>
	///         Manual import is done by calling one of the <c> GetExport </c> or <c> GetExports </c> methods explicitly, stating the name for which the import value needs to be resolved.
	///         This is usually used to retrieve an export from the <see cref="CompositionContainer" /> programmatically.
	///     </para>
	///     <para>
	///         Model-based import is done by decorating properties of composed types or objects with <see cref="ImportAttribute" />.
	///         This is usually used for implementing Dependency Injection (DI) or to retrieve an export from the <see cref="CompositionContainer" /> declaratively.
	///     </para>
	///     <para>
	///         <b> IMPLICIT &amp; EXPLICIT MODEL-BASED IMPORTING </b>
	///     </para>
	///     <para>
	///         Model-based import comes in two flavours: Implicit and explicit.
	///     </para>
	///     <para>
	///         Explicit model-based import is done by passing an object to the <see cref="ResolveImports(object, CompositionFlags)" /> method.
	///         The specified object gets then all its imports resolved and assigned once.
	///         See <see cref="ResolveImports(object, CompositionFlags)" /> for details.
	///     </para>
	///     <para>
	///         Implicit model-based import is done automatically by the <see cref="CompositionContainer" /> itself by resolving the imports of all its known exports.
	///         The imports of the known exports of a <see cref="CompositionContainer" /> are resolved whenever the composition changes (e.g. a manual export method is used or a composition catalog is added or removed) or a recomposition is executed (using the <see cref="Recompose(CompositionFlags)" /> method).
	///     </para>
	///     <para>
	///         <b> SINGLE &amp; MULTIPLE IMPORTING </b>
	///     </para>
	///     <para>
	///         Importing can be used in two ways: Single import and multiple import.
	///     </para>
	///     <para>
	///         A single import is when one of the <c> GetExport </c> methods is used or when an <see cref="ImportAttribute" /> is applied to a normal property (means: a property not of type <see cref="Import" />).
	///         In such cases, the first resolved import value is provided or assigned respectively.
	///         If the import resolves to more than one value, the provided value is one of them but it is not defined which one.
	///     </para>
	///     <para>
	///         A multiple import is when one of the <c> GetExports </c> methods is used or when an <see cref="ImportAttribute" /> is applied to a property of the type <see cref="Import" />.
	///         In such cases, all the resolved import values are provided or assigned respectively.
	///         Therefore, multiple types or objects can be exported under the same name.
	///     </para>
	///     <para>
	///         <b> ELIGIBLE TYPES </b>
	///     </para>
	///     <para>
	///         Only non-abstract class types can be exported.
	///         Only non-generic class or interface types can be imported.
	///     </para>
	///     <para>
	///         <b> RECOMPOSITION &amp; TRACKING </b>
	///     </para>
	///     <para>
	///         Model-based imports can be marked as &quot;recomposable&quot; using <see cref="ImportAttribute" />.<see cref="ImportAttribute.Recomposable" />.
	///         This means that the imports of such properties are reimported or resolved again respectively whenever the composition for the corresponding name changes (e.g. a new export of that name gets added to the <see cref="CompositionContainer" />).
	///         However, this is only done using implicit model-based importing, meaning that only exports known to the <see cref="CompositionContainer" /> get their recomposable imports updated.
	///         Imports resolved using <see cref="ResolveImports(object, CompositionFlags)" /> are not updated during a recomposition.
	///         See <see cref="Recompose(CompositionFlags)" /> and <see cref="ResolveImports(object, CompositionFlags)" /> for more details.
	///     </para>
	///     <para>
	///         Model-based imports which are not recomposable remain their imported value, even if the corresponding export gets removed from the <see cref="CompositionContainer" />.
	///         Therefore, model-based imports which are expected to change must be made recomposable.
	///         <see cref="IImporting" /> can be used to detect when imports have changed.
	///     </para>
	///     <para>
	///         <b> UNDEFINED STATE </b>
	///     </para>
	///     <para>
	///         If a <see cref="CompositionException" /> is thrown during a composition, the state of the <see cref="CompositionContainer" /> and all its compositions are undefined and might remain unusable.
	///         Therefore, a <see cref="CompositionException" /> should always be treated as a serious error which prevents the program from continueing normally.
	///     </para>
	///     <para>
	///         <b> SELF-COMPOSITION </b>
	///     </para>
	///     <para>
	///         A <see cref="CompositionContainer" /> does not add itself as an export by default but can be added like any other manual or model-based object export (the <see cref="CompositionContainer" /> has the <see cref="ExportAttribute" /> applied).
	///         A <see cref="CompositionContainer" /> which has an export of another <see cref="CompositionContainer" /> does only export that other <see cref="CompositionContainer" /> instance as it does any other object or type but does not also export the exports of that other <see cref="CompositionContainer" />.
	///     </para>
	///     <para>
	///         <b> MULTIPLE COMPOSITION CONTAINERS </b>
	///     </para>
	///     <para>
	///         Multiple composition containers can coexist independently side-by-side.
	///         They can also share the same exports and <see cref="CompositionCatalog" />s.
	///     </para>
	///     <para>
	///         <b> LOGGING </b>
	///     </para>
	///     <para>
	///         A <see cref="CompositionContainer" /> logs all composition operations.
	///         <see cref="ILogService" /> is used for logging, obtained through <see cref="LogLocator" />.
	///         If no <see cref="ILogService" /> is available, no logging is performed.
	///         If logging is not desired, it can be disabled using <see cref="LoggingEnabled" />.
	///     </para>
	///     <para>
	///         <b> EXPORT OF ALL TYPES </b>
	///     </para>
	///     <para>
	///         A <see cref="CompositionCatalog" /> could also explicitly export all eligible types, even if they do not have any <see cref="ExportAttribute" /> defined.
	///         The effect is that all types which are visible to the catalog and which can be exported are either exported as described above, respecting their <see cref="ExportAttribute" />, or using default names if a type has no <see cref="ExportAttribute" /> defined.
	///     </para>
	///     <para>
	///         Whether a <see cref="CompositionCatalog" /> provides the functionality to export all types depends on the particular implementation of <see cref="CompositionCatalog" />.
	///         See the description of the <see cref="CompositionCatalog" /> implementations for more details.
	///     </para>
	///     <para>
	///         This can help with exporting/importing types which are not under your control, where you cannot apply a <see cref="ExportAttribute" /> and you do not want to explicitly create instances of those types.
	///     </para>
	///     <para>
	///         <b> PARENT CONTAINER </b>
	///     </para>
	///     <para>
	///         When constructing a <see cref="CompositionContainer" />, a parent container can be specified (see <see cref="CompositionContainer(CompositionContainer)" />).
	///         The constructed <see cref="CompositionContainer" /> then inherits all exports of its parent container and also reflects the changes in composition of its parent container.
	///     </para>
	///     <para>
	///         The only way to detach from a parent container is through <see cref="Dispose" /> but no other parent container can be attached afterwards.
	///     </para>
	///     <para> SIMPLE USAGE / ROOT SINGLETON </para>
	///     <para>
	///         A very simple usage of <see cref="CompositionContainer" /> and the principles of Dependency Injection is the creation of one <see cref="CompositionContainer" /> instance and use it wehere necessary.
	///     </para>
	///     <para>
	///         This can be done using your own mechanism to create, store, and distribute instances of <see cref="CompositionContainer" /> or you can use the built-in singleton mechanism.
	///         By calling the static <see cref="CreateSingleton" /> method, you create a globally available singleton instance of <see cref="CompositionContainer" /> which can be retrieved by the static property <see cref="Singleton" />.
	///         Instead of <see cref="Singleton" /> you can also always use <see cref="CreateSingleton" /> to create the singleton if it does not exis (the first call to <see cref="CreateSingleton" /> will create it and all subsequent calls will retrieve the created singleton).
	///         Therefore, you might want to use a <see cref="CompositionContainer" /> singleton as a sole &quot;root&quot; singleton which is used to retrieve all kind of instances and dependencies you need, practically avoiding the implementation of the singleton pattern except for <see cref="CompositionContainer" /> itself.
	///     </para>
	///     <para>
	///         Note that you can have full control over the creation and management of the singleton.
	///         See <see cref="Singleton" /> and <see cref="CreateSingleton" /> for more details.
	///     </para>
	///     <para>
	///         This approach (simple usage with a root singleton) is supported by the extension methods provided by <see cref="CompositionExtensions" />.
	///         In the simplest case, depending on the used extension methods, you would not need to deal with <see cref="CompositionContainer" /> at all.
	///     </para>
	///     <para> ADVANCED USAGE / BOOTSTRAPPER </para>
	///     <para>
	///         In more advanced scenarios, where the application or game requires bootstrapping of its various components and subsystems, a <see cref="CompositionContainer" /> is one of the core components for bootstrappers, as implemented by <see cref="Bootstrapper" /> / <see cref="IBootstrapper" />.
	///         See <see cref="Bootstrapper" /> and <see cref="IBootstrapper" /> for more information about bootstrappers and their use of <see cref="CompositionContainer" />.
	///     </para>
	///     <para> PERFORMANCE </para>
	///     <para>
	///         Be aware that doing composition is a costly operation.
	///         Therefore, composition is usually done during startup to bring everything in place (or resolving all dependencies respectively).
	///         The performance impact for composition operations depend on various factors, including the used <see cref="CompositionCatalog" />s, and cannot be generally stated.
	///     </para>
	///     <note type="important">
	///         <see cref="CompositionContainer" /> is thread-safe.
	///         It uses exclusive locks for its composition operations.
	///         It is important to know that the <see cref="CompositionCatalog" />s used by a <see cref="CompositionContainer" /> are accessed from inside locks to <see cref="SyncRoot" />!
	///         Be careful when explicitly dealing with catalogs in multithreaded scenarios to not produce deadlocks!
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class CompositionContainer : IDisposable, ISynchronizable, ILogSource
	{
		#region Constants

		internal static readonly StringComparerEx NameComparer = StringComparerEx.Ordinal;

		#endregion




		#region Static Constructor/Destructor

		static CompositionContainer ()
		{
			CompositionContainer.GlobalSyncRoot = new object();
		}

		#endregion




		#region Static Properties/Indexer

		/// <summary>
		///     Gets the current composition container singleton.
		/// </summary>
		/// <value>
		///     The current composition container singleton or null if no singleton exists.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         A <see cref="CompositionContainer" /> singleton is not automatically created when accessing this property.
		///         <see cref="Singleton" /> internally uses <see cref="Singleton{CompositionContainer}" /> to retrieve singleton instances.
		///         Therefore, you can either create and assign an instance yourself using <see cref="Singleton{CompositionContainer}" /> or use <see cref="CreateSingleton" /> to do it for you.
		///     </note>
		/// </remarks>
		public static CompositionContainer Singleton
		{
			get
			{
				lock (CompositionContainer.GlobalSyncRoot)
				{
					return Singleton<CompositionContainer>.Instance;
				}
			}
		}

		private static object GlobalSyncRoot { get; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the current composition container singleton or creates a new one if none exists.
		/// </summary>
		/// <returns>
		///     The current composition container singleton or the newly created one if no singleton exists.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If a singleton instance already exists, that instance is returned without changes.
		///         If no singleton instance exists, one is created, added as an export to itself, and bound to the <see cref="ServiceLocator" />.
		///     </para>
		///     <note type="note">
		///         <see cref="CreateSingleton" /> internally uses <see cref="Singleton{CompositionContainer}" /> to retrieve and create singleton instances.
		///         Therefore, you have full control over the created singleton through <see cref="Singleton{CompositionContainer}" />.
		///         That also means that if you create the singleton instance yourself using <see cref="Singleton{CompositionContainer}" />, you are responsible for adding the <see cref="CompositionContainer" /> as an export to itself and binding it to the <see cref="ServiceLocator" />.
		///     </note>
		/// </remarks>
		public static CompositionContainer CreateSingleton ()
		{
			lock (CompositionContainer.GlobalSyncRoot)
			{
				return CompositionContainer.CreateSingletonInternal(false);
			}
		}

		/// <summary>
		///     Gets the current composition container singleton or creates a new one if none exists.
		/// </summary>
		/// <returns>
		///     The current composition container singleton or the newly created one if no singleton exists.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="CreateSingletonWithEverything" /> does the same as <see cref="CreateSingleton" /> except that, if a singleton instance is created, it also adds an <see cref="AppDomainCatalog" /> to immediately get all types in the current application domain as exports.
		///     </para>
		///     <para>
		///         See <see cref="CreateSingleton" /> for more details.
		///     </para>
		/// </remarks>
		public static CompositionContainer CreateSingletonWithEverything ()
		{
			lock (CompositionContainer.GlobalSyncRoot)
			{
				return CompositionContainer.CreateSingletonInternal(true);
			}
		}

		/// <summary>
		///     Gets all names under which a type is exported (using <see cref="ExportAttribute" />).
		/// </summary>
		/// <param name="type"> The type whose export names are to be determined. </param>
		/// <param name="includeWithoutAttribute"> Specifies whether a type is exported under its default name if it does not have an <see cref="ExportAttribute" />. </param>
		/// <returns>
		///     The set of names the specified type is exported under.
		///     The set is empty if the specified type has no exports defined.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static HashSet<string> GetExportsOfType (Type type, bool includeWithoutAttribute)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			HashSet<string> exports = new HashSet<string>(CompositionContainer.NameComparer);

			CompositionContainer.GetExportsOfTypeInternal(type, includeWithoutAttribute, true, exports);

			List<Type> inheritedTypes = type.GetInheritance(false);
			foreach (Type inheritedType in inheritedTypes)
			{
				CompositionContainer.GetExportsOfTypeInternal(inheritedType, false, false, exports);
			}

			Type[] interfaceTypes = type.GetInterfaces();
			foreach (Type interfaceType in interfaceTypes)
			{
				CompositionContainer.GetExportsOfTypeInternal(interfaceType, includeWithoutAttribute, false, exports);
			}

			return exports;
		}

		/// <summary>
		///     Gets the default name of a type.
		/// </summary>
		/// <param name="type"> The type whose default name is to be determined. </param>
		/// <returns>
		///     The default name of the specified type.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static string GetNameOfType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.FullName;
		}

		/// <summary>
		///     Gets whether a type is exported privately (using <see cref="ExportAttribute" />).
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <returns>
		///     true if the type is exported privately, false if the type is exported shared, or null if the specified type has no exports defined.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="CompositionException"> The inheritance of <paramref name="type" /> defines multiple <see cref="ExportAttribute" /> with conflicting values for <see cref="ExportAttribute.Private" />. </exception>
		public static bool? IsExportPrivate (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			HashSet<bool> privates = new HashSet<bool>();

			CompositionContainer.IsExportPrivateInternal(type, true, privates);

			List<Type> inheritedTypes = type.GetInheritance(false);
			foreach (Type inheritedType in inheritedTypes)
			{
				CompositionContainer.IsExportPrivateInternal(inheritedType, false, privates);
			}

			Type[] interfaceTypes = type.GetInterfaces();
			foreach (Type interfaceType in interfaceTypes)
			{
				CompositionContainer.IsExportPrivateInternal(interfaceType, false, privates);
			}

			if (privates.Count == 0)
			{
				return null;
			}

			if ((privates.Count(x => x) != privates.Count) && (privates.Count(x => !x) != privates.Count))
			{
				throw new CompositionException("Conflicting private exports defined. All exports in a types hierarchy must be either private or shared.");
			}

			return privates.FirstOrDefault(false);
		}

		/// <summary>
		///     Validates whether an object can be exported.
		/// </summary>
		/// <param name="instance"> The object to validate. </param>
		/// <returns>
		///     true if the object can be exported by a <see cref="CompositionContainer" />, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         An object can be exported if it is an instance of a class.
		///     </para>
		/// </remarks>
		public static bool ValidateExportInstance (object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			return CompositionContainer.ValidateExportType(instance.GetType());
		}

		/// <summary>
		///     Validates whether a type can be exported.
		/// </summary>
		/// <param name="type"> The type to validate. </param>
		/// <returns>
		///     true if the type can be exported by a <see cref="CompositionContainer" />, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A type can be exported if it is a non-abstract class type.
		///     </para>
		/// </remarks>
		public static bool ValidateExportType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.IsClass && (!type.IsAbstract);
		}

		private static CompositionContainer CreateSingletonInternal (bool addAppDomainCatalog)
		{
			CompositionContainer container = Singleton<CompositionContainer>.Instance;
			if (container == null)
			{
				container = Singleton<CompositionContainer>.Ensure();
				container.AddExport(container, typeof(CompositionContainer));
				if (addAppDomainCatalog)
				{
					container.AddCatalog(new AppDomainCatalog(true, true));
				}
				ServiceLocator.BindToCompositionContainer(container);
			}
			return container;
		}

		private static void GetExportsOfTypeInternal (Type type, bool includeWithoutAttribute, bool isSelf, HashSet<string> exports)
		{
			object[] attributes = type.GetCustomAttributes(typeof(ExportAttribute), false);
			if (attributes.Length > 0)
			{
				foreach (ExportAttribute attribute in attributes)
				{
					if (attribute.Inherited || isSelf)
					{
						string name = attribute.Name ?? CompositionContainer.GetNameOfType(type);
						exports.Add(name);
					}
				}
			}
			else if (includeWithoutAttribute)
			{
				string name = CompositionContainer.GetNameOfType(type);
				exports.Add(name);
			}
		}

#if PLATFORM_NETFX
		private static Type GetEnumerableType (Type type)
		{
			Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
			return (genericType == typeof(IEnumerable<>)) ? typeArgument : null;
		}

		private static Type GetLazyLoadFuncType (Type type)
		{
			Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
			int typeArgumentCount = type.IsGenericType ? type.GetGenericArguments().Length : 0;
			return ((genericType == typeof(Func<>)) && (typeArgumentCount == 1)) ? typeArgument : null;
		}

		private static Type GetLazyLoadObjectType(Type type)
		{
			Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
			return (genericType == typeof(Lazy<>)) ? typeArgument : null;
		}
#endif

		private static void IsExportPrivateInternal (Type type, bool isSelf, HashSet<bool> privates)
		{
			object[] attributes = type.GetCustomAttributes(typeof(ExportAttribute), false);
			foreach (ExportAttribute attribute in attributes)
			{
				if (attribute.Inherited || isSelf)
				{
					privates.Add(attribute.Private);
				}
			}
		}

		private static bool ValidateImportType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

#if PLATFORM_NETFX
			Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : type;
			int typeArgumentCount = type.IsGenericType ? type.GetGenericArguments().Length : 0;
			return (typeArgument.IsClass || typeArgument.IsInterface) && ((genericType == null) || (genericType == typeof(IEnumerable<>)) || ((genericType == typeof(Func<>)) && (typeArgumentCount == 1)) || (genericType == typeof(Lazy<>)));
#endif
#if PLATFORM_UNITY
			return (type.IsClass || type.IsInterface) && (!type.IsGenericType);
#endif
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainer" />.
		/// </summary>
		public CompositionContainer ()
		{
			this.SyncRoot = new object();

			this.CatalogRecomposeRequestHandler = this.HandleCatalogRecomposeRequest;
			this.ParentContainerCompositionChangedHandler = this.HandleParentContainerCompositionChanged;

			this.AutoDispose = true;
			this.LoggingEnabled = true;

			this.ParentContainer = null;

			this.Instances = new List<CompositionCatalogItem>();
			this.Types = new List<CompositionCatalogItem>();
			this.Catalogs = new List<CompositionCatalog>();
			this.Creators = new HashSet<CompositionCreator>();
			this.Composition = new Dictionary<string, CompositionItem>(CompositionContainer.NameComparer);
			this.LazyInvokers = new Dictionary<string, Dictionary<Type, LazyInvoker>>(CompositionContainer.NameComparer);

			this.UpdateComposition(true);
		}

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainer" />.
		/// </summary>
		/// <param name="parentContainer"> The used parent composition container. </param>
		/// <remarks>
		///     <para>
		///         The created composition container will be a child composition container with <paramref name="parentContainer" /> as its parent composition container.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="parentContainer" /> is null. </exception>
		public CompositionContainer (CompositionContainer parentContainer)
			: this()
		{
			if (parentContainer == null)
			{
				throw new ArgumentNullException(nameof(parentContainer));
			}

			this.ParentContainer = parentContainer;
			this.ParentContainer.CompositionChanged += this.ParentContainerCompositionChangedHandler;

			this.UpdateComposition(true);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="CompositionContainer" />.
		/// </summary>
		~CompositionContainer ()
		{
			this.Dispose();
		}

		#endregion




		#region Instance Fields

		private bool _autoDispose;
		private bool _loggingEnabled;
		private CompositionContainer _parentContainer;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether exports have <see cref="IDisposable.Dispose" /> called when removed.
		/// </summary>
		/// <value>
		///     true if <see cref="IDisposable.Dispose" /> is called when an export which implements <see cref="IDisposable" /> is removed, false otherwise.
		/// </value>
		/// <rermarks>
		///     <para>
		///         This setting applies to all instances which are exported, regardless whether they are object exports or instances created from type exports.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </rermarks>
		public bool AutoDispose
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._autoDispose;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._autoDispose = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets whether logging, using <see cref="LogLocator" />, is enabled.
		/// </summary>
		/// <value>
		///     true if logging is enabled, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool LoggingEnabled
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._loggingEnabled;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._loggingEnabled = value;
				}
			}
		}

		/// <summary>
		///     Gets the parent composition container if this composition container is a child composition container.
		/// </summary>
		/// <value>
		///     The parent composition container or null if this composition container is not a child composition container.
		/// </value>
		public CompositionContainer ParentContainer
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._parentContainer;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._parentContainer = value;
				}
			}
		}

		private EventHandler CatalogRecomposeRequestHandler { get; }

		private List<CompositionCatalog> Catalogs { get; }

		private Dictionary<string, CompositionItem> Composition { get; }

		private HashSet<CompositionCreator> Creators { get; }

		private List<CompositionCatalogItem> Instances { get; }

		private EventHandler ParentContainerCompositionChangedHandler { get; }

		private List<CompositionCatalogItem> Types { get; }

		[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
		private Dictionary<string, Dictionary<Type, LazyInvoker>> LazyInvokers { get; }

		#endregion




		#region Instance Events

		/// <summary>
		///     Raised when the composition has changed.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This event is raised after recomposition is done.
		///     </para>
		/// </remarks>
		public event EventHandler CompositionChanged;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Model-based export: Adds a composition catalog to use its exports for composition.
		/// </summary>
		/// <param name="catalog"> The composition catalog to add. </param>
		/// <remarks>
		///     <para>
		///         If the specified catalog is already added, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="catalog" /> is null. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void AddCatalog (CompositionCatalog catalog)
		{
			if (catalog == null)
			{
				throw new ArgumentNullException(nameof(catalog));
			}

			lock (this.SyncRoot)
			{
				this.AddCatalogInternal(catalog);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Adds a composition creator to use it to create instances of exported types.
		/// </summary>
		/// <param name="creator"> The composition creator to add. </param>
		/// <remarks>
		///     <para>
		///         If the specified creator is already added, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		public void AddCreator (CompositionCreator creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			lock (this.SyncRoot)
			{
				this.AddCreatorInternal(creator);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Manual export: Adds an object and exports it under the specified types default name for composition.
		/// </summary>
		/// <param name="instance"> The object to export. </param>
		/// <param name="exportType"> The type under whose default name the object is exported. </param>
		/// <remarks>
		///     <para>
		///         If the specified object is already exported under the specified type, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportType" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void AddExport (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual export: Adds an object and exports it under the specified name for composition.
		/// </summary>
		/// <param name="instance"> The object to export. </param>
		/// <param name="exportName"> The name under which the object is exported. </param>
		/// <remarks>
		///     <para>
		///         If the specified object is already exported under the specified name, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="instance" /> is not of a type which can be exported. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void AddExport (object instance, string exportName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (!CompositionContainer.ValidateExportInstance(instance))
			{
				throw new InvalidTypeArgumentException(nameof(instance));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				this.AddInstanceInternal(instance, exportName);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Manual export: Adds a type and exports it under the specified types default name for composition.
		/// </summary>
		/// <param name="type"> The type to export. </param>
		/// <param name="exportType"> The type under whose default name the type is exported. </param>
		/// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
		/// <remarks>
		///     <para>
		///         If the specified type is already exported under the specified name, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="exportType" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="type" /> is not of a type which can be exported. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void AddExport (Type type, Type exportType, bool privateExport)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(type, CompositionContainer.GetNameOfType(exportType), privateExport);
		}

		/// <summary>
		///     Manual export: Adds a type and exports it under the specified name for composition.
		/// </summary>
		/// <param name="type"> The type to export. </param>
		/// <param name="exportName"> The name under which the type is exported. </param>
		/// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
		/// <remarks>
		///     <para>
		///         If the specified type is already exported under the specified type, the composition remains unchanged.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="type" /> is not of a type which can be exported. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void AddExport (Type type, string exportName, bool privateExport)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!CompositionContainer.ValidateExportType(type))
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				this.AddTypeInternal(type, exportName, privateExport);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Removes all exports.
		/// </summary>
		/// <remarks>
		///     <para>
		///         All exports of all catalogs, objects, and types are removed.
		///     </para>
		/// </remarks>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void Clear ()
		{
			lock (this.SyncRoot)
			{
				this.ClearInternal();
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Executes all composition actions in a <see cref="CompositionBatch" />.
		/// </summary>
		/// <param name="batch"> The <see cref="CompositionBatch" /> to execute. </param>
		/// <remarks>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
		/// <exception cref="CompositionException"> One or more imports cannot be resolved. </exception>
		public void Compose (CompositionBatch batch)
		{
			if (batch == null)
			{
				throw new ArgumentNullException(nameof(batch));
			}

			lock (this.SyncRoot)
			{
				foreach (CompositionCreator creator in batch.CreatorsToAdd)
				{
					this.AddCreatorInternal(creator);
				}

				foreach (CompositionCreator creator in batch.CreatorsToRemove)
				{
					this.RemoveCreatorInternal(creator);
				}

				foreach (CompositionCatalogItem item in batch.ItemsToAdd)
				{
					if (item.Value != null)
					{
						this.AddInstanceInternal(item.Value, item.Name);
					}
					else if (item.Type != null)
					{
						this.AddTypeInternal(item.Type, item.Name, item.PrivateExport);
					}
				}

				foreach (CompositionCatalogItem item in batch.ItemsToRemove)
				{
					if (item.Value != null)
					{
						this.RemoveInstanceInternal(item.Value, item.Name);
					}
					else if (item.Type != null)
					{
						this.RemoveTypeInternal(item.Type, item.Name);
					}
				}

				foreach (CompositionCatalog catalog in batch.CatalogsToAdd)
				{
					this.AddCatalogInternal(catalog);
				}

				foreach (CompositionCatalog catalog in batch.CatalogsToRemove)
				{
					this.RemoveCatalogInternal(catalog);
				}

				this.UpdateComposition(false);

				this.Recompose(batch.Composition | CompositionFlags.Normal);

				foreach (KeyValuePair<object, CompositionFlags> obj in batch.ObjectsToSatisfy)
				{
					this.ResolveImports(obj.Key, obj.Value);
				}
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Creates a new <see cref="CompositionContainer" /> with this container as its parent container.
		/// </summary>
		/// <returns> </returns>
		public CompositionContainer CreateChildContainer ()
		{
			return new CompositionContainer(this);
		}

		/// <summary>
		///     Manual import: Gets the first resolved value for the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type whose default name is resolved. </typeparam>
		/// <returns>
		///     The first resolved value which is exported under the specified types default name and which is of type <typeparamref name="T" />, null if no such value could be resolved.
		/// </returns>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public T GetExport <T> ()
			where T : class
		{
			return this.GetExport<T>(CompositionContainer.GetNameOfType(typeof(T)));
		}

		/// <summary>
		///     Manual import: Gets the first resolved value for the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type the resolved value must be compatible with. </typeparam>
		/// <param name="exportType"> The type whose default name is resolved. </param>
		/// <returns>
		///     The first resolved value which is exported under the specified types default name and which is of type <typeparamref name="T" />, null if no such value could be resolved.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="exportType" /> is null. </exception>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public T GetExport <T> (Type exportType)
			where T : class
		{
			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			return this.GetExport<T>(CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual import: Gets the first resolved value for the specified name.
		/// </summary>
		/// <typeparam name="T"> The type the resolved value must be compatible with. </typeparam>
		/// <param name="exportName"> The name which is resolved. </param>
		/// <returns>
		///     The first resolved value which is exported under the specified name and which is of type <typeparamref name="T" />, null if no such value could be resolved.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public T GetExport <T> (string exportName)
			where T : class
		{
			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				List<object> instances = this.GetOrCreateInstancesInternal(exportName, typeof(T));
				return instances.Count == 0 ? null : (T)instances[0];
			}
		}

		/// <summary>
		///     Manual import: Gets all resolved values for the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type whose default name is resolved. </typeparam>
		/// <returns>
		///     The list containing the resolved values.
		///     The list is empty if no values could be resolved or none of the values are of type <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public List<T> GetExports <T> ()
			where T : class
		{
			return this.GetExports<T>(CompositionContainer.GetNameOfType(typeof(T)));
		}

		/// <summary>
		///     Manual import: Gets all resolved values for the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type the resolved values must be compatible with. </typeparam>
		/// <param name="exportType"> The type whose default name is resolved. </param>
		/// <returns>
		///     The list containing the resolved values.
		///     The list is empty if no values could be resolved or none of the values are of type <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="exportType" /> is null. </exception>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public List<T> GetExports <T> (Type exportType)
			where T : class
		{
			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			return this.GetExports<T>(CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual import: Gets all resolved values for the specified name.
		/// </summary>
		/// <typeparam name="T"> The type the resolved values must be compatible with. </typeparam>
		/// <param name="exportName"> The name which is resolved. </param>
		/// <returns>
		///     The list containing the resolved values.
		///     The list is empty if no values could be resolved or none of the values are of type <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public List<T> GetExports <T> (string exportName)
			where T : class
		{
			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				List<object> instances = this.GetOrCreateInstancesInternal(exportName, typeof(T));
				return DirectLinqExtensions.Select(instances, x => (T)x);
			}
		}

		/// <summary>
		///     Determines whether there is at least one value for importing of the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type to check whether its default name can be resolved to at least one value for importing. </typeparam>
		/// <returns>
		///     true if there is at least one value for the specified types default name, false otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The imports for the specified types default name are not actually resolved, e.g. type exports are not instantiated.
		///     </note>
		/// </remarks>
		public bool HasExport <T> ()
			where T : class
		{
			return this.HasExport(typeof(T));
		}

		/// <summary>
		///     Determines whether there is at least one value for importing of the specified types default name.
		/// </summary>
		/// <param name="exportType"> The type to check whether its default name can be resolved to at least one value for importing. </param>
		/// <returns>
		///     true if there is at least one value for the specified types default name, false otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The imports for the specified types default name are not actually resolved, e.g. type exports are not instantiated.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="exportType" /> is null. </exception>
		public bool HasExport (Type exportType)
		{
			return this.HasExport(CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Determines whether there is at least one value for importing of the specified name.
		/// </summary>
		/// <param name="exportName"> The name to check whether it can be resolved to at least one value for importing. </param>
		/// <returns>
		///     true if there is at least one value for the specified name, false otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The imports for the specified name are not actually resolved, e.g. type exports are not instantiated.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		public bool HasExport (string exportName)
		{
			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				if (this.ParentContainer != null)
				{
					if (this.ParentContainer.HasExport(exportName))
					{
						return true;
					}
				}

				return this.Composition.ContainsKey(exportName);
			}
		}

		/// <summary>
		///     Model-based import: Resolves the imports of all the exports currently managed by this <see cref="CompositionContainer" />.
		/// </summary>
		/// <param name="composition"> The composition type which is going to be done on the exports. </param>
		/// <returns>
		///     true if any of the imports of at least one export were resolved or updated, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Internally, this method does nothing else than calling <see cref="ResolveImports" /> for each export which is currently contained in this <see cref="CompositionContainer" />.
		///         However, it is only done for object exports or instantiated type exports.
		///         Nothing happens for type exports which are not instantiated (e.g. they were never resolved so far).
		///     </para>
		/// </remarks>
		/// <exception cref="CompositionException"> One or more imports cannot be resolved. </exception>
		public bool Recompose (CompositionFlags composition)
		{
			lock (this.SyncRoot)
			{
				bool recomposed = this.ParentContainer?.Recompose(composition) ?? false;
				List<object> instances = this.GetExistingInstancesInternal(false);
				foreach (object instance in instances)
				{
					if (this.ResolveImports(instance, composition))
					{
						recomposed = true;
					}
				}
				return recomposed;
			}
		}

		/// <summary>
		///     Model-based export: Removes a composition catalog so that its exports are no longer used for composition.
		/// </summary>
		/// <param name="catalog"> The composition catalog to remove. </param>
		/// <remarks>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="catalog" /> is null. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void RemoveCatalog (CompositionCatalog catalog)
		{
			if (catalog == null)
			{
				throw new ArgumentNullException(nameof(catalog));
			}

			lock (this.SyncRoot)
			{
				this.RemoveCatalogInternal(catalog);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Removes a composition creator so taht it is no longer used to create instances of exported types.
		/// </summary>
		/// <param name="creator"> The composition creator to remove. </param>
		/// <remarks>
		///     <note type="note">
		///         Instances of exported types which were created with the composition creator to remove are not removed from the composition container and are continued to be used for composition.
		///     </note>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		public void RemoveCreator (CompositionCreator creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			lock (this.SyncRoot)
			{
				this.RemoveCreatorInternal(creator);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Manual export: Removes an object exported under the specified types default name so that it is no longer used for composition.
		/// </summary>
		/// <param name="instance"> The exported object. </param>
		/// <param name="exportType"> The type under whose default name the object is exported. </param>
		/// <remarks>
		///     <para>
		///         Only the export matching the specified object and type is removed.
		///         If the same object is also exported under other types or names, those exports remain intact.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportType" /> is null. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void RemoveExport (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveExport(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual export: Removes an object exported under the specified name so that it is no longer used for composition.
		/// </summary>
		/// <param name="instance"> The exported object. </param>
		/// <param name="exportName"> The name under which the object is exported. </param>
		/// <remarks>
		///     <para>
		///         Only the export matching the specified object and name is removed.
		///         If the same object is also exported under other types or names, those exports remain intact.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="instance" /> or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void RemoveExport (object instance, string exportName)
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

			lock (this.SyncRoot)
			{
				this.RemoveInstanceInternal(instance, exportName);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Manual export: Removes a type exported under the specified types default name so that it is no longer used for composition.
		/// </summary>
		/// <param name="type"> The exported type. </param>
		/// <param name="exportType"> The type under whose default name the type is exported. </param>
		/// <remarks>
		///     <para>
		///         Only the export matching the specified types is removed.
		///         If the same type is also exported under other types or names, those exports remain intact.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="exportType" /> is null. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void RemoveExport (Type type, Type exportType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveExport(type, CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual export: Removes a type exported under the specified name so that it is no longer used for composition.
		/// </summary>
		/// <param name="type"> The exported type. </param>
		/// <param name="exportName"> The name under which the type is exported. </param>
		/// <remarks>
		///     <para>
		///         Only the export matching the specified type and name is removed.
		///         If the same type is also exported under other types or names, those exports remain intact.
		///     </para>
		///     <para>
		///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
		///         See <see cref="Recompose(CompositionFlags)" /> for details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		/// <exception cref="CompositionException"> The internal recomposition failed. </exception>
		public void RemoveExport (Type type, string exportName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			lock (this.SyncRoot)
			{
				this.RemoveTypeInternal(type, exportName);
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		/// <summary>
		///     Model-based import: Resolves the imports of the specified object, using <see cref="ImportAttribute" />.
		/// </summary>
		/// <param name="obj"> The object whose imports are resolved. </param>
		/// <param name="composition"> The composition type which is going to be done on the object. </param>
		/// <returns>
		///     true if any of the imports of the object were resolved or updated, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Note that resolving imports using the <see cref="ResolveImports" /> method is a one-time, import-and-forget action from the point-of-view of the <see cref="CompositionContainer" />.
		///         The specified object whose imports are resolved is not added to the <see cref="CompositionContainer" /> as an export (this must be done separately if desired).
		///         This means that a recomposition, which is only done on existing exports of the <see cref="CompositionContainer" /> (e.g. using the <see cref="Recompose(CompositionFlags)" /> method or adding or removing exports), has no effect on the specified object or its imports respectively (unless it is added to the <see cref="CompositionContainer" /> as an export).
		///         In such cases, <see cref="ResolveImports" /> must be called again on the specified object if its imports need to be resolved again or updated respectively.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="obj" /> is not a type which can be composed. </exception>
		/// <exception cref="CompositionException"> The imports for <paramref name="obj" /> cannot be resolved. </exception>
		[SuppressMessage("ReSharper", "RedundantAssignment")]
		public bool ResolveImports (object obj, CompositionFlags composition)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (!CompositionContainer.ValidateExportInstance(obj))
			{
				throw new InvalidTypeArgumentException(nameof(obj));
			}

			lock (this.SyncRoot)
			{
				IImporting importing = obj as IImporting;

				importing?.ImportsResolving(composition);

				bool composed = false;

				bool isConstructing = (composition & CompositionFlags.Constructing) == CompositionFlags.Constructing;
				bool updateMissing = (composition & CompositionFlags.Missing) == CompositionFlags.Missing;
				bool updateRecomposable = (composition & CompositionFlags.Recomposable) == CompositionFlags.Recomposable;
				bool updateComposed = (composition & CompositionFlags.Composed) == CompositionFlags.Composed;

				Type type = obj.GetType();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo property in properties)
				{
					List<ImportAttribute> attributes = property.GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().ToList();
					bool canRecompose = attributes.Any(x => x.Recomposable);
					object oldValue = property.GetGetMethod(true).Invoke(obj, null);

					if ((isConstructing || (updateMissing && (oldValue == null)) || (updateRecomposable && canRecompose) || (updateComposed && (oldValue != null))) && (attributes.Count > 0))
					{
						string importName = attributes.FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
						Type importType = property.PropertyType;

						ImportKind importKind;
						object newValue = this.GetImportValueFromNameOrType(importName, importType, out importKind);

						bool updateValue = false;
						if (importKind == ImportKind.Special)
						{
							Import oldImport = oldValue as Import;
							Import newImport = newValue as Import;
							List<object> oldValues = oldImport?.GetInstancesSnapshot();
							List<object> newValues = newImport?.GetInstancesSnapshot();
							updateValue = !CollectionComparer<object>.ReferenceEquality.Equals(oldValues, newValues);
						}
#if PLATFORM_NETFX
						else if (importKind == ImportKind.Enumerable)
						{
							List<object> oldValues = (oldValue as IEnumerable)?.ToList();
							List<object> newValues = (newValue as IEnumerable)?.ToList();
							updateValue = !CollectionComparer<object>.ReferenceEquality.Equals(oldValues, newValues);
						}
						else if ((importKind == ImportKind.LazyFunc) || (importKind == ImportKind.LazyObject))
						{
							updateValue = oldValue == null;
						}
#endif
						else if (importKind == ImportKind.Single)
						{
							updateValue = !object.ReferenceEquals(oldValue, newValue);
						}

						if (updateValue)
						{
							MethodInfo setMethod = property.GetSetMethod(true);
							if (setMethod == null)
							{
								throw new CompositionException("Cannot set value for property because set accessor is missing/unavailable: " + type.FullName + "." + property.Name);
							}
							else
							{
								this.Log(LogLevel.Debug, "Updating import ({0}): {1}", composition, type.FullName + "." + property.Name);
								setMethod.Invoke(obj, new[] {newValue});
							}

							composed = true;
						}
					}
				}

				importing?.ImportsResolved(composition, composed);

				return composed;
			}
		}

		internal Dictionary<string, List<CompositionCatalogItem>> GetCompositionSnapshot ()
		{
			lock (this.SyncRoot)
			{
				Dictionary<string, List<CompositionCatalogItem>> snapshot = new Dictionary<string, List<CompositionCatalogItem>>(CompositionContainer.NameComparer);
				foreach (KeyValuePair<string, CompositionItem> composition in this.Composition)
				{
					string name = composition.Value.Name;

					List<CompositionCatalogItem> items = new List<CompositionCatalogItem>();
					snapshot.Add(name, items);

					foreach (CompositionTypeItem type in composition.Value.Types)
					{
						if (type.Instance != null)
						{
							items.Add(new CompositionCatalogItem(name, type.Instance));
						}
						else
						{
							items.Add(new CompositionCatalogItem(name, type.Type, type.PrivateExport));
						}
					}

					foreach (CompositionInstanceItem instance in composition.Value.Instances)
					{
						items.Add(new CompositionCatalogItem(name, instance.Instance));
					}
				}
				return snapshot;
			}
		}

		private void AddCatalogInternal (CompositionCatalog catalog)
		{
			if (this.Catalogs.Contains(catalog))
			{
				return;
			}

			this.Catalogs.Add(catalog);

			catalog.RecomposeRequested += this.CatalogRecomposeRequestHandler;
		}

		private void AddCreatorInternal (CompositionCreator creator)
		{
			if (this.Creators.Contains(creator))
			{
				return;
			}

			this.Creators.Add(creator);
		}

		private void AddInstanceInternal (object instance, string name)
		{
			if (DirectLinqExtensions.Any(this.Instances, x => object.ReferenceEquals(x.Value, instance) && CompositionContainer.NameComparer.Equals(x.Name, name)))
			{
				return;
			}

			this.Instances.Add(new CompositionCatalogItem(name, instance));
		}

		private void AddTypeInternal (Type type, string name, bool privateExport)
		{
			if (DirectLinqExtensions.Any(this.Types, x => (x.Type == type) && CompositionContainer.NameComparer.Equals(x.Name, name)))
			{
				return;
			}

			this.Types.Add(new CompositionCatalogItem(name, type, privateExport));
		}

		private void ClearInternal ()
		{
			foreach (CompositionCatalog catalog in this.Catalogs)
			{
				catalog.RecomposeRequested -= this.CatalogRecomposeRequestHandler;
			}

			this.Catalogs.Clear();
			this.Instances.Clear();
			this.Types.Clear();

			this.UpdateComposition(true);
		}

#if PLATFORM_NETFX

		private object CreateArray(Type type, List<object> content)
		{
			Array array = Array.CreateInstance(type, content.Count);
			((ICollection)content).CopyTo(array, 0);
			return array;
		}

		private object CreateGenericLazyLoadFunc (string name, Type type)
		{
			LazyInvoker invoker = null;
			if (this.LazyInvokers.ContainsKey(name))
			{
				if (this.LazyInvokers[name].ContainsKey(type))
				{
					invoker = this.LazyInvokers[name][type];
				}
				else
				{
					this.LazyInvokers[name].Add(type, null);
				}
			}
			else
			{
				this.LazyInvokers.Add(name, new Dictionary<Type, LazyInvoker>());
				this.LazyInvokers[name].Add(type, null);
			}

			if (invoker == null)
			{
				invoker = new LazyInvoker(this, name, type);
				this.LazyInvokers[name][type] = invoker;
			}

			return invoker.Resolver;
		}

		private object CreateGenericLazyLoadObject (string name, Type type)
		{
			object lazyLoadFunc = this.CreateGenericLazyLoadFunc(name, type);
			Type genericType = typeof(Lazy<>);
			Type concreteType = genericType.MakeGenericType(type);
			object lazyLoadObject = Activator.CreateInstance(concreteType, BindingFlags.Default, null, lazyLoadFunc);
			return lazyLoadObject;
		}

#endif

		private List<object> GetExistingInstancesInternal (bool includeParentInstances)
		{
			List<object> instances = new List<object>();

			foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
			{
				foreach (CompositionInstanceItem instance in compositionItem.Value.Instances)
				{
					if (instance.Instance != null)
					{
						instances.Add(instance.Instance);
					}
				}

				foreach (CompositionTypeItem type in compositionItem.Value.Types)
				{
					if (type.Instance != null)
					{
						instances.Add(type.Instance);
					}
				}
			}

			if (includeParentInstances && (this.ParentContainer != null))
			{
				lock (this.ParentContainer.SyncRoot)
				{
					instances.AddRange(this.ParentContainer.GetExistingInstancesInternal(true));
				}
			}

			return instances;
		}

		private enum ImportKind
		{
			Special,
			Enumerable,
			LazyFunc,
			LazyObject,
			Single,
		}

		private Type GetImportTypeFromType (Type type, out ImportKind kind)
		{
			if (type == typeof(Import))
			{
				kind = ImportKind.Special;
				return typeof(object);
			}

#if PLATFORM_NETFX
			Type enumerableType = CompositionContainer.GetEnumerableType(type);
			if (enumerableType != null)
			{
				kind = ImportKind.Enumerable;
				return enumerableType;
			}

			Type lazyLoadFuncType = CompositionContainer.GetLazyLoadFuncType(type);
			if (lazyLoadFuncType != null)
			{
				kind = ImportKind.LazyFunc;
				return lazyLoadFuncType;
			}
			
			Type lazyLoadObjectType = CompositionContainer.GetLazyLoadObjectType(type);
			if (lazyLoadObjectType != null)
			{
				kind = ImportKind.LazyObject;
				return lazyLoadObjectType;
			}
#endif

			kind = ImportKind.Single;
			return type;
		}

		private object GetImportValueFromNameOrType (string name, Type type, out ImportKind kind)
		{
			Type importType = this.GetImportTypeFromType(type, out kind);

			if ((kind == ImportKind.Special) && name.IsNullOrEmptyOrWhitespace())
			{
				throw new CompositionException("No import name specified for import using the " + nameof(Import) + " type.");
			}

			if (!CompositionContainer.ValidateImportType(importType))
			{
				throw new CompositionException("Invalid import type: " + importType.Name);
			}

			string importName = name.IsNullOrEmptyOrWhitespace() ? CompositionContainer.GetNameOfType(importType) : name;

			List<object> importValues = null;
			if ((kind != ImportKind.LazyFunc) && (kind != ImportKind.LazyObject))
			{
				importValues = this.GetOrCreateInstancesInternal(importName, importType);
			}

			if (kind == ImportKind.Special)
			{
				return new Import(importValues?.Count == 0 ? null : importValues?.ToArray());
			}

#if PLATFORM_NETFX
			if (kind == ImportKind.Enumerable)
			{
				return this.CreateArray(importType, importValues);
			}

			if (kind == ImportKind.LazyFunc)
			{
				return this.CreateGenericLazyLoadFunc(importName, importType);
			}

			if (kind == ImportKind.LazyObject)
			{
				return this.CreateGenericLazyLoadObject(importName, importType);
			}
#endif

			if (importValues?.Count > 0)
			{
				return importValues[0];
			}

			return null;
		}

		private List<object> GetOrCreateInstancesInternal (string name, Type compatibleType)
		{
			if (!this.HasExport(name))
			{
				return new List<object>();
			}

			HashSet<object> instances = new HashSet<object>();
			HashSet<Type> types = new HashSet<Type>();

			if (this.Composition.ContainsKey(name))
			{
				CompositionItem item = this.Composition[name];

				foreach (CompositionInstanceItem instanceItem in item.Instances)
				{
					if (instanceItem.Instance != null)
					{
						instances.Add(instanceItem.Instance);
					}
				}

				foreach (CompositionTypeItem typeItem in item.Types)
				{
					if (typeItem.Instance != null)
					{
						instances.Add(typeItem.Instance);
					}
					else
					{
						types.Add(typeItem.Type);
					}
				}
			}

			if (this.ParentContainer != null)
			{
				lock (this.ParentContainer.SyncRoot)
				{
					List<object> parentInstances = this.ParentContainer.GetOrCreateInstancesInternal(name, compatibleType);
					instances.AddRange(parentInstances);
				}
			}

			List<object> newInstances = new List<object>();
			foreach (Type type in types)
			{
				bool supportedByCreators = this.Creators.Any(x => x.CanCreateInstance(this, type, compatibleType, name));

				object newInstance = null;
				{
					MethodInfo[] allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

					List<MethodInfo> methods = DirectLinqExtensions.Where(allMethods, x => (x.GetCustomAttributes(typeof(ExportCreatorAttribute), true).Length > 0) && (x.ReturnType != typeof(void)) && (x.GetParameters().Length >= 1) && (x.GetParameters()[0].ParameterType == typeof(Type)));

					if (methods.Count > 1)
					{
						throw new CompositionException("Too many export creators defined for type: " + type.FullName);
					}

					if (methods.Count > 0)
					{
						MethodInfo method = methods[0];
						ParameterInfo[] methodParameters = method.GetParameters();
						object[] parameters = new object[methodParameters.Length];

						parameters[0] = type;

						for (int i1 = 1; i1 < methodParameters.Length; i1++)
						{
							string importName = methodParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
							Type importType = methodParameters[i1].ParameterType;

							ImportKind importKind;
							parameters[i1] = this.GetImportValueFromNameOrType(importName, importType, out importKind);
						}

						newInstance = method.Invoke(null, parameters);
					}
				}

				if (newInstance == null)
				{
					ConstructorInfo[] allConstructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					List<ConstructorInfo> declaredConstructors = DirectLinqExtensions.Where(allConstructors, x => x.GetCustomAttributes(typeof(ExportConstructorAttribute), false).Length > 0);

					if (declaredConstructors.Count > 1)
					{
						throw new CompositionException("Too many export constructors defined for type: " + type.FullName);
					}

					List<ConstructorInfo> greedyConstructors = supportedByCreators ? new List<ConstructorInfo>() : new List<ConstructorInfo>(allConstructors);
					greedyConstructors.RemoveAll(x =>
					{
						ParameterInfo[] parameterCandidates = x.GetParameters();
						foreach (ParameterInfo parameterCandidate in parameterCandidates)
						{
							string importName = parameterCandidate.GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, y => !y.Name.IsNullOrEmptyOrWhitespace())?.Name;
							ImportKind importKind;
							Type importType = this.GetImportTypeFromType(parameterCandidate.ParameterType, out importKind);

							if ((importKind == ImportKind.Special) && importName.IsNullOrEmptyOrWhitespace())
							{
								return true;
							}

							if ((!importName.IsNullOrEmptyOrWhitespace()) && (!this.HasExport(importName)))
							{
								return true;
							}

							if ((importKind != ImportKind.Special) && (!this.HasExport(importType)))
							{
								return true;
							}
						}
						return false;
					});
					greedyConstructors.Sort((x, y) => x.GetParameters().Length.CompareTo(y.GetParameters().Length));
					greedyConstructors.Reverse();

					List<ConstructorInfo> constructors = new List<ConstructorInfo>();
					constructors.AddRange(declaredConstructors);
					constructors.AddRange(greedyConstructors);

					if (constructors.Count > 0)
					{
						ConstructorInfo constructor = constructors[0];
						ParameterInfo[] constructorParameters = constructor.GetParameters();
						object[] parameters = new object[constructorParameters.Length];

						for (int i1 = 0; i1 < constructorParameters.Length; i1++)
						{
							string importName = constructorParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
							Type importType = constructorParameters[i1].ParameterType;

							ImportKind importKind;
							parameters[i1] = this.GetImportValueFromNameOrType(importName, importType, out importKind);
						}

						newInstance = constructor.Invoke(parameters);
					}
				}

				if ((newInstance == null) && supportedByCreators)
				{
					foreach (CompositionCreator creator in this.Creators)
					{
						if (creator.CanCreateInstance(this, type, compatibleType, name))
						{
							newInstance = creator.CreateInstance(this, type, compatibleType, name);
							if (newInstance != null)
							{
								break;
							}
						}
					}
				}

				if (newInstance == null)
				{
					throw new CompositionException("No export creators, export constructors, or composition creators could be resolved for type: " + type.FullName);
				}

				newInstances.Add(newInstance);

				foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
				{
					foreach (CompositionTypeItem typeItem in compositionItem.Value.Types)
					{
						if ((typeItem.Type == type) && (typeItem.Instance == null) && (!typeItem.PrivateExport))
						{
							typeItem.Instance = newInstance;
						}
					}
				}
			}

			foreach (object newInstance in newInstances)
			{
				this.ResolveImports(newInstance, CompositionFlags.Constructing);
			}

			foreach (object newInstance in newInstances)
			{
				this.Log(LogLevel.Debug, "Type added to container: {0} / {1}", name, newInstance.GetType().FullName);

				IExporting exportingInstance = newInstance as IExporting;
				exportingInstance?.AddedToContainer(name, this);
			}

			HashSetExtensions.AddRange(instances, newInstances);

			return DirectLinqExtensions.Where(instances, x => compatibleType.IsAssignableFrom(x.GetType()));
		}

		private void HandleCatalogRecomposeRequest (object sender, EventArgs e)
		{
			lock (this.SyncRoot)
			{
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		private void HandleParentContainerCompositionChanged (object sender, EventArgs e)
		{
			lock (this.SyncRoot)
			{
				this.UpdateComposition(true);
			}

			this.RaiseCompositionChanged();
		}

		private void RaiseCompositionChanged ()
		{
			this.CompositionChanged?.Invoke(this, EventArgs.Empty);
		}

		private void RemoveCatalogInternal (CompositionCatalog catalog)
		{
			if (!this.Catalogs.Contains(catalog))
			{
				return;
			}

			catalog.RecomposeRequested -= this.CatalogRecomposeRequestHandler;

			ICollectionExtensions.RemoveAll(this.Catalogs, catalog);
		}

		private void RemoveCreatorInternal (CompositionCreator creator)
		{
			if (!this.Creators.Contains(creator))
			{
				return;
			}

			ICollectionExtensions.RemoveAll(this.Creators, creator);
		}

		private void RemoveInstanceInternal (object instance, string name)
		{
			this.Instances.RemoveAll(x => object.ReferenceEquals(x.Value, instance) && CompositionContainer.NameComparer.Equals(x.Name, name));
		}

		private void RemoveTypeInternal (Type type, string name)
		{
			this.Types.RemoveAll(x => (x.Type == type) && CompositionContainer.NameComparer.Equals(x.Name, name));
		}

		[SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
		private void UpdateComposition (bool recompose)
		{
			foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
			{
				compositionItem.Value.ResetChecked();
			}

			List<CompositionCatalogItem> items = new List<CompositionCatalogItem>();

			items.AddRange(this.Instances);
			items.AddRange(this.Types);

			foreach (CompositionCatalog catalog in this.Catalogs)
			{
				catalog.UpdateItems();

				foreach (KeyValuePair<string, List<CompositionCatalogItem>> item in catalog.GetItemsSnapshot())
				{
					items.AddRange(item.Value);
				}
			}

			Dictionary<object, HashSet<string>> newInstances = new Dictionary<object, HashSet<string>>();
			foreach (CompositionCatalogItem item in items)
			{
				CompositionItem compositionItem;
				if (this.Composition.ContainsKey(item.Name))
				{
					compositionItem = this.Composition[item.Name];
				}
				else
				{
					this.Log(LogLevel.Debug, "Adding export: {0}", item.Name);
					compositionItem = new CompositionItem(item.Name);
					this.Composition.Add(compositionItem.Name, compositionItem);
				}

				if (item.Value != null)
				{
					if (CompositionContainer.ValidateExportInstance(item.Value))
					{
						CompositionInstanceItem instanceItem = DirectLinqExtensions.FirstOrDefault(compositionItem.Instances, x => object.ReferenceEquals(x.Instance, item.Value));
						if (instanceItem == null)
						{
							instanceItem = new CompositionInstanceItem(item.Value);
							compositionItem.Instances.Add(instanceItem);

							if (!newInstances.ContainsKey(item.Value))
							{
								newInstances.Add(item.Value, new HashSet<string>(CompositionContainer.NameComparer));
							}
							newInstances[item.Value].Add(compositionItem.Name);
						}
						instanceItem.Checked = true;
					}
				}
				else if (item.Type != null)
				{
					if (CompositionContainer.ValidateExportType(item.Type))
					{
						CompositionTypeItem typeItem = DirectLinqExtensions.FirstOrDefault(compositionItem.Types, x => (x.Type == item.Type));
						if (typeItem == null)
						{
							typeItem = new CompositionTypeItem(item.Type, item.PrivateExport);
							compositionItem.Types.Add(typeItem);
						}
						typeItem.Checked = true;
					}
				}
			}

			foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
			{
				compositionItem.Value.Instances.RemoveWhere(x => !x.Checked).ForEach(x =>
				{
					this.Log(LogLevel.Debug, "Instance removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.FullName ?? "[null]");
					(x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
					if (this.AutoDispose && (!object.ReferenceEquals(x?.Instance, this)))
					{
						(x?.Instance as IDisposable)?.Dispose();
					}
				});
				compositionItem.Value.Types.RemoveWhere(x => !x.Checked).ForEach(x =>
				{
					this.Log(LogLevel.Debug, "Type removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.FullName ?? "[null]");
					(x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
					if (this.AutoDispose && (!object.ReferenceEquals(x?.Instance, this)))
					{
						(x?.Instance as IDisposable)?.Dispose();
					}
				});
				compositionItem.Value.ResetChecked();
			}

			IDictionaryExtensions.RemoveWhere(this.Composition, x => (x.Value.Instances.Count == 0) && (x.Value.Types.Count == 0)).ForEach(x => this.Log(LogLevel.Debug, "Removing export: {0}", x));

			foreach (KeyValuePair<object, HashSet<string>> newInstance in newInstances)
			{
				foreach (string name in newInstance.Value)
				{
					this.Log(LogLevel.Debug, "Instance added to container: {0} / {1}", name, newInstance.Key.GetType().FullName);

					IExporting exportingInstance = newInstance.Key as IExporting;
					exportingInstance?.AddedToContainer(name, this);
				}
			}

			if (recompose)
			{
				this.Recompose(CompositionFlags.Normal);
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		public void Dispose ()
		{
			lock (this.SyncRoot)
			{
				if (this.ParentContainer != null)
				{
					this.ParentContainer.CompositionChanged -= this.ParentContainerCompositionChangedHandler;
					this.ParentContainer = null;
				}

				this.ClearInternal();

				this.LazyInvokers.Clear();
			}

			this.RaiseCompositionChanged();
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion




		#region Type: CompositionInstanceItem

		private sealed class CompositionInstanceItem
		{
			#region Instance Constructor/Destructor

			public CompositionInstanceItem (object instance)
			{
				this.Checked = false;
				this.Instance = instance;
			}

			#endregion




			#region Instance Properties/Indexer

			public bool Checked { get; set; }

			public object Instance { get; private set; }

			#endregion
		}

		#endregion




		#region Type: CompositionItem

		private sealed class CompositionItem
		{
			#region Instance Constructor/Destructor

			public CompositionItem (string name)
			{
				this.Name = name;
				this.Instances = new List<CompositionInstanceItem>();
				this.Types = new List<CompositionTypeItem>();
			}

			#endregion




			#region Instance Properties/Indexer

			public List<CompositionInstanceItem> Instances { get; private set; }

			public string Name { get; private set; }

			public List<CompositionTypeItem> Types { get; private set; }

			#endregion




			#region Instance Methods

			public void ResetChecked ()
			{
				foreach (CompositionInstanceItem instanceItem in this.Instances)
				{
					instanceItem.Checked = false;
				}

				foreach (CompositionTypeItem typeItem in this.Types)
				{
					typeItem.Checked = false;
				}
			}

			#endregion
		}

		#endregion




		#region Type: CompositionTypeItem

		private sealed class CompositionTypeItem
		{
			#region Instance Constructor/Destructor

			public CompositionTypeItem (Type type, bool privateExport)
			{
				this.Type = type;
				this.PrivateExport = privateExport;

				this.Checked = false;
				this.Instance = null;
			}

			#endregion




			#region Instance Properties/Indexer

			public bool Checked { get; set; }

			public object Instance { get; set; }

			public bool PrivateExport { get; private set; }

			public Type Type { get; private set; }

			#endregion
		}

		#endregion




		private sealed class LazyInvoker
		{
			#region Instance Constructor/Destructor

			public LazyInvoker (CompositionContainer container, string name, Type type)
			{
				this.Container = container;
				this.Name = name;
				this.Type = type;
				this.Resolver = null;

#if PLATFORM_NETFX
				bool useName = !name.IsNullOrEmptyOrWhitespace();

				Type enumerableType = CompositionContainer.GetEnumerableType(type);
				string resolveName = enumerableType == null ? nameof(CompositionContainer.GetExport) : nameof(CompositionContainer.GetExports);

				MethodInfo genericMethod = container.GetType().GetMethod(resolveName, useName ? new [] { typeof(string) } : new Type[] { });
				MethodInfo resolveMethod = genericMethod.MakeGenericMethod(type);

				MethodCallExpression resolveCall = useName ? Expression.Call(Expression.Constant(this.Container), resolveMethod, Expression.Constant(this.Name)) : Expression.Call(Expression.Constant(this.Container), resolveMethod);
				Delegate resolveLambda = Expression.Lambda(resolveCall).Compile();

				this.Resolver = resolveLambda;
#endif
			}

			#endregion




			#region Instance Properties/Indexer

			public Delegate Resolver { get; }

			private CompositionContainer Container { get; }

			private string Name { get; }

			private Type Type { get; }

			#endregion
		}
	}
}
