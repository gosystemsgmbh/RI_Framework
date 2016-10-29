using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.Linq;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Reflection;




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
	///         The search and provisioning of exports (of the same name as the import) is called &quot;resolving imports&quot; or simply &quot;resolving&quot;.
	///     </para>
	///     <para>
	///         <b> NAMES </b>
	///     </para>
	///     <para>
	///         What also might be confusing a little bit is the &quot;name&quot; of imports and exports.
	///         Especially because sometimes a type and sometimes a name is mentioned.
	///         When exports are managed by a <see cref="CompositionContainer" />, or a <see cref="CompositionCatalog" />, they are always identified using their name.
	///         Resolving of imports is also always done using the name of the import.
	///         Now, when types are used instead of names, the types are simply translated into what is called &quot;the types default name&quot;.
	///         After the translation, the types are ignored and the exports and imports are continued to be handled using their translated names.
	///         For example, the method <see cref="AddExport(object, Type)" /> does nothing else than determining the types default name and then calling <see cref="AddExport(object, string)" /> with that name.
	///         This allows you to mix type-based import and export (using their types default names) and also name-based import and export (using any custom names you specify).
	///         Finally, a types default name is simply its namespace and type name, e.g. the string &quot;<see cref="RI" />.<see cref="RI.Framework" />.<see cref="RI.Framework.Composition" />.<see cref="CompositionContainer" />&quot; for <see cref="CompositionContainer" />.
	///     </para>
	///     <para>
	///         Note that names are case-sensitive.
	///     </para>
	///     <para>
	///         <b> EXPORT COMPOSITION &amp; DEPENDENCY INJECTION (DI) </b>
	///     </para>
	///     <para>
	///         A powerful aspect of the <see cref="CompositionContainer" /> is the fact that all its known exports are composed themselves.
	///         This means that all exports of a <see cref="CompositionContainer" /> can have imports themselves (model-based imports using <see cref="ImportPropertyAttribute" />) and that those imports are automatically resolved by the <see cref="CompositionContainer" />.
	///         In other words, when getting an import from a <see cref="CompositionContainer" />, all its own imports (if any) will be resolved (if possible).
	///     </para>
	///     <para>
	///         This is how Dependency Injection is implemented in the <see cref="CompositionContainer" />.
	///         If you have a cascade of objects, with all kind of dependencies, you do not need to resolve them all by yourself by creating instances, dealing with singletons, or getting manual imports.
	///         You just make the <see cref="CompositionContainer" /> aware where to find all the possibly required objects or types (the simplest way to do this is to use an <see cref="AssemblyCatalog" />) and then start to pull the objects from the <see cref="CompositionContainer" /> as you need them.
	///     </para>
	///     <para>
	///         <b> MANUAL &amp; MODEL-BASED EXPORTING </b>
	///     </para>
	///     <para>
	///         There are two ways of exporting: Manual and model-based.
	///     </para>
	///     <para>
	///         Manual export is done by calling one of the <c> AddExport </c> methods explicitly and stating a type or object and under which name it is exported.
	///         Advantage: A type or object does not need any special preparation in order to be exported manually, any type or object can be exported (restrictions apply, see below).
	///         Disadvantage: The type or object to be manually exported must be known and explicitly added to the <see cref="CompositionContainer" />, adding a strong dependency to that type or object and/or a lot of boilerplate code just to discover the types or objects.
	///     </para>
	///     <para>
	///         Model-based export is done by using a <see cref="CompositionCatalog" /> and adding it to the <see cref="CompositionContainer" /> using the <see cref="AddCatalog(CompositionCatalog)" /> method.
	///         Advantage: No dependencies or references to the exported types or objects are required at compile time because, depending on the used <see cref="CompositionCatalog" />, the composition catalog might collect all exports by itself (e.g. all prepared types in an <see cref="Assembly" /> when using <see cref="AssemblyCatalog" />).
	///         Disadvantage: A type or object needs special preparation in order to be model-based exported, namely at least one or more <see cref="ExportAttribute" /> applied to it.
	///     </para>
	///     <para>
	///         <b> TYPE &amp; OBJECT EXPORTS </b>
	///     </para>
	///     <para>
	///         Two things can be exported: Types and objects.
	///         When it says &quot;types or objects&quot;, it means that either a <see cref="Type" /> or an already instantiated <see cref="object" /> of any type can be used.
	///     </para>
	///     <para>
	///         A type can be exported by specifying the <see cref="Type" /> and under which name it is exported.
	///         When an import is resolved to such a type export, a new instance of that type is created (if not yet created) or the previously created instance of that type is used and provided as the import value.
	///         A type can be exported multiple times under different names.
	///         It is important to know that the same type is only instantiated once in a <see cref="CompositionContainer" />.
	///         That means that the one instance of a particular type is used for all exports of that type, even if exported under different names.
	///         Therefore, type exports are always shared, singleton-like exports.
	///         <see cref="ExportConstructorAttribute" /> and <see cref="ExportCreatorAttribute" /> are used to help with the construction of instances for type exports.
	///         <see cref="ExportCreatorAttribute" />s have higher priority than <see cref="ExportConstructorAttribute" />s when determining how an instance of a type export is to be created.
	///         Only if the <see cref="ExportCreatorAttribute" />s yield no usable results or are not used, <see cref="ExportConstructorAttribute" /> is used.
	///     </para>
	///     <para>
	///         An object can be exported by specifying the <see cref="object" /> and under which name it is exported.
	///         When an import is resolved to such an object export, the specified object itself is provided as the import value.
	///         An object can be exported multiple times under different names.
	///         Unlike type exports, object exports using different instances of the same type are possible.
	///     </para>
	///     <para>
	///         Although type exports share one instance for the same type, it is possible to have type exports of a particular type and also one or more object exports with instances of that same type.
	///         In such cases, a new shared instance for the type export is still created although there are object exports with instances of the same type.
	///         Or in other words: Type exports and object exports do not share their instances.
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
	///         Model-based import is done by decorating properties of composed types or objects with <see cref="ImportPropertyAttribute" />.
	///         This is usually used for implementing Dependency Injection (DI).
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
	///         A single import is when one of the <c> GetExport </c> methods is used or when an <see cref="ImportPropertyAttribute" /> is applied to a normal property (means: a property not of type <see cref="Import" />).
	///         In such cases, the first resolved import value is provided or assigned respectively.
	///         If the import resolves to more than one value, the provided value is one of them but it is not defined which one.
	///     </para>
	///     <para>
	///         A multiple import is when one of the <c> GetExports </c> methods is used or when an <see cref="ImportPropertyAttribute" /> is applied to a property of the type <see cref="Import" />.
	///         In such cases, all the resolved import values are provided or assigned respectively.
	///         Therefore, multiple types or object can be exported under the same name.
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
	///         Model-based imports can be marked as &quot;recomposable&quot; using <see cref="ImportPropertyAttribute" />.<see cref="ImportPropertyAttribute.Recomposable" />.
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
	///     </para>
	/// </remarks>
	/// <example>
	///     <para>
	///         The following example shows how a <see cref="CompositionContainer" /> can be used:
	///     </para>
	///     <code language="cs">
	/// <![CDATA[
	/// // create the container
	/// var container = new CompositionContainer();
	/// 
	/// // automatically dispose all objects which implement <see cref="IDisposable"/> when removed from the container
	/// container.AutoDispose = true;
	/// 
	/// // add a catalog (all types in the current assembly)
	/// container.AddCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
	/// 
	/// // start pulling instances from the container
	/// var sources = container.GetExports<IMySources>();
	/// var service = container.GetExport<MyService>();
	/// ]]>
	/// </code>
	/// </example>
	[Export]
	public sealed class CompositionContainer : IDisposable
	{
		#region Constants

		internal static readonly StringComparer NameComparer = StringComparer.Ordinal;

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets all names under which a type is exported (using <see cref="ExportAttribute" />).
		/// </summary>
		/// <param name="type"> The type whose export names are to be determined. </param>
		/// <returns>
		///     The set of names the specified type is exported under.
		///     The set is empty if the specified type has no exports defined.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static HashSet<string> GetExportsOfType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			HashSet<string> exports = new HashSet<string>(CompositionContainer.NameComparer);

			CompositionContainer.GetExportsOfTypeInternal(type, true, exports);

			List<Type> inheritedTypes = type.GetInheritance(false);
			foreach (Type inheritedType in inheritedTypes)
			{
				CompositionContainer.GetExportsOfTypeInternal(inheritedType, false, exports);
			}

			Type[] interfaceTypes = type.GetInterfaces();
			foreach (Type interfaceType in interfaceTypes)
			{
				CompositionContainer.GetExportsOfTypeInternal(interfaceType, false, exports);
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

		private static void GetExportsOfTypeInternal (Type type, bool isSelf, HashSet<string> exports)
		{
			object[] attributes = type.GetCustomAttributes(typeof(ExportAttribute), false);
			foreach (ExportAttribute attribute in attributes)
			{
				if (attribute.Inherited || isSelf)
				{
					string name = attribute.Name ?? CompositionContainer.GetNameOfType(type);
					exports.Add(name);
				}
			}
		}

		private static bool ValidateImportType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

#if PLATFORM_NET
			Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : type;
			return (typeArgument.IsClass || typeArgument.IsInterface) && ((genericType == null) || (genericType == typeof(IEnumerable<>)));
#else
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
			this.CatalogRecomposeRequestHandler = this.HandleCatalogRecomposeRequest;

			this.AutoDispose = false;

			this.Instances = new List<CompositionCatalogItem>();
			this.Types = new List<CompositionCatalogItem>();
			this.Catalogs = new List<CompositionCatalog>();
			this.Composition = new Dictionary<string, CompositionItem>(CompositionContainer.NameComparer);

			this.UpdateComposition(true);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="CompositionContainer" />.
		/// </summary>
		~CompositionContainer ()
		{
			this.Dispose(false);
		}

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
		///         The default value is false.
		///     </para>
		/// </rermarks>
		public bool AutoDispose { get; set; }

		private Action CatalogRecomposeRequestHandler { get; set; }

		private List<CompositionCatalog> Catalogs { get; set; }

		private Dictionary<string, CompositionItem> Composition { get; set; }

		private List<CompositionCatalogItem> Instances { get; set; }

		private List<CompositionCatalogItem> Types { get; set; }

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
		public event Action CompositionChanged;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Model-based export: Adds a composition catalog to use its exports for composition, using <see cref="ExportAttribute" />.
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

			this.AddCatalogInternal(catalog);
			this.UpdateComposition(true);
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

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.AddInstanceInternal(instance, exportName);
			this.UpdateComposition(true);
		}

		/// <summary>
		///     Manual export: Adds a type and exports it under the specified types default name for composition.
		/// </summary>
		/// <param name="type"> The type to export. </param>
		/// <param name="exportType"> The type under whose default name the type is exported. </param>
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
		public void AddExport (Type type, Type exportType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(type, CompositionContainer.GetNameOfType(exportType));
		}

		/// <summary>
		///     Manual export: Adds a type and exports it under the specified name for composition.
		/// </summary>
		/// <param name="type"> The type to export. </param>
		/// <param name="exportName"> The name under which the type is exported. </param>
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
		public void AddExport (Type type, string exportName)
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

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.AddTypeInternal(type, exportName);
			this.UpdateComposition(true);
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
			foreach (CompositionCatalog catalog in this.Catalogs)
			{
				catalog.RecomposeRequested -= this.CatalogRecomposeRequestHandler;
			}

			this.Catalogs.Clear();
			this.Instances.Clear();
			this.Types.Clear();

			this.UpdateComposition(true);
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

			foreach (CompositionCatalogItem item in batch.ItemsToAdd)
			{
				if (item.Value != null)
				{
					this.AddInstanceInternal(item.Value, item.Name);
				}
				else if (item.Type != null)
				{
					this.AddTypeInternal(item.Type, item.Name);
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

			this.RaiseCompositionChanged();

			foreach (KeyValuePair<object, CompositionFlags> obj in batch.ObjectsToSatisfy)
			{
				this.ResolveImports(obj.Key, obj.Value);
			}
		}

		/// <summary>
		///     Manual import: Gets the first resolved value for the specified types default name.
		/// </summary>
		/// <typeparam name="T"> The type whose default name is resolved. </typeparam>
		/// <returns>
		///     The first resolved value which is exported under the specified types default name and which is of type <typeparamref name="T" />, null if no such value could be resolved.
		/// </returns>
		/// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
		public T GetExport <T> () where T : class
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
		public T GetExport <T> (Type exportType) where T : class
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
		public T GetExport <T> (string exportName) where T : class
		{
			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			List<object> instances = this.GetInstancesInternal(exportName, typeof(T));
			return instances.Count == 0 ? null : (T)instances[0];
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
		public List<T> GetExports <T> () where T : class
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
		public List<T> GetExports <T> (Type exportType) where T : class
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
		public List<T> GetExports <T> (string exportName) where T : class
		{
			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			List<object> instances = this.GetInstancesInternal(exportName, typeof(T));
			return IEnumerableExtensions.Select(instances, x => (T)x);
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

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			return this.Composition.ContainsKey(exportName);
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
			bool recomposed = false;
			List<object> instances = this.GetAllInstancesInternal();
			foreach (object instance in instances)
			{
				if (this.ResolveImports(instance, composition))
				{
					recomposed = true;
				}
			}
			return recomposed;
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

			this.RemoveCatalogInternal(catalog);
			this.UpdateComposition(true);
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

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.RemoveInstanceInternal(instance, exportName);
			this.UpdateComposition(true);
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

			if (exportName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.RemoveTypeInternal(type, exportName);
			this.UpdateComposition(true);
		}

		/// <summary>
		///     Model-based import: Resolves the imports of the specified object, using <see cref="ImportPropertyAttribute" />.
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
				object[] attributes = property.GetCustomAttributes(typeof(ImportPropertyAttribute), false);
				foreach (ImportPropertyAttribute attribute in attributes)
				{
					bool canRecompose = attribute.Recomposable;
					object oldValue = property.GetGetMethod(true).Invoke(obj, null);

					if (isConstructing || (updateMissing && (oldValue == null)) || (updateRecomposable && canRecompose) || (updateComposed && (oldValue != null)))
					{
						object newValue = null;
						bool updateValue = false;
						bool currentComposed = false;
						bool asArray = false;

						Type propertyType = property.PropertyType;
						if (!CompositionContainer.ValidateImportType(propertyType))
						{
							throw new CompositionException("Invalid import type for property: " + propertyType.Name + " @ " + property.Name + " @ " + type.FullName);
						}

#if PLATFORM_NET
						Type genericType = propertyType.IsGenericType ? propertyType.GetGenericTypeDefinition() : null;
						Type typeArgument = propertyType.IsGenericType ? propertyType.GetGenericArguments()[0] : propertyType;
						propertyType = typeArgument;
						asArray = genericType != null;
#endif

						if (propertyType == typeof(Import))
						{
							string importName = attribute.Name;
							if (importName == null)
							{
								throw new CompositionException("Missing export type or export name for property: " + propertyType.Name + " @ " + property.Name + " @ " + type.FullName);
							}

							List<object> instances = this.GetInstancesInternal(importName, typeof(object));

							newValue = oldValue ?? new Import();
							((Import)newValue).Instances = instances.Count == 0 ? null : instances.ToArray();

							Import oldImport = oldValue as Import;
							Import newImport = newValue as Import;
							IEnumerable<object> oldValues = oldImport?.Instances;
							IEnumerable<object> newValues = newImport?.Instances;

							updateValue = !CollectionComparer<object>.ReferenceEquality.Equals(oldValues, newValues);
							currentComposed = updateValue;
						}
						else
						{
							string importName = attribute.Name ?? CompositionContainer.GetNameOfType(propertyType);

							List<object> instances = this.GetInstancesInternal(importName, propertyType);

							if (instances.Count == 0)
							{
								newValue = null;
							}
							else
							{
								if (asArray)
								{
									Array array = Array.CreateInstance(propertyType, instances.Count);
									((ICollection)instances).CopyTo(array, 0);
									newValue = array;
								}
								else
								{
									newValue = instances[0];
								}
							}

							updateValue = !object.ReferenceEquals(oldValue, newValue);
							currentComposed = updateValue;
						}

						if (updateValue)
						{
							MethodInfo setMethod = property.GetSetMethod(true);
							if (setMethod == null)
							{
								throw new CompositionException("Cannot set value for property because set accessor is missing/unavailable: " + propertyType.Name + " @ " + property.Name + " @ " + type.FullName);
							}
							else
							{
								this.Log("Updating import ({0}): {1} @ {2}", composition, property.Name, obj.GetType().FullName);

								setMethod.Invoke(obj, new[]
								                 {
									                 newValue
								                 });
							}
						}

						if (currentComposed)
						{
							composed = true;
						}
					}
				}
			}

			importing?.ImportsResolved(composition, composed);

			return composed;
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

		private void AddInstanceInternal (object instance, string name)
		{
			if (IEnumerableExtensions.Any(this.Instances, x => object.ReferenceEquals(x.Value, instance) && CompositionContainer.NameComparer.Equals(x.Name, name)))
			{
				return;
			}

			this.Instances.Add(new CompositionCatalogItem(name, instance));
		}

		private void AddTypeInternal (Type type, string name)
		{
			if (IEnumerableExtensions.Any(this.Types, x => (x.Type == type) && CompositionContainer.NameComparer.Equals(x.Name, name)))
			{
				return;
			}

			this.Types.Add(new CompositionCatalogItem(name, type));
		}

		[SuppressMessage ("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			this.Clear();
		}

		private List<object> GetAllInstancesInternal ()
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

			return instances;
		}

		private List<object> GetInstancesInternal (string name, Type compatibleType)
		{
			if (!this.Composition.ContainsKey(name))
			{
				return new List<object>();
			}

			CompositionItem item = this.Composition[name];

			HashSet<object> instances = new HashSet<object>();
			HashSet<Type> types = new HashSet<Type>();

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

			List<object> newInstances = new List<object>();
			foreach (Type type in types)
			{
				object newInstance = null;
				{
					MethodInfo[] allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					List<MethodInfo> methods = IEnumerableExtensions.Where(allMethods, x => (x.GetCustomAttributes(typeof(ExportCreatorAttribute), true).Length > 0) && (x.ReturnType != typeof(void)) && (x.GetParameters().Length >= 1) && (x.GetParameters()[0].ParameterType == typeof(Type)));

					if (methods.Count > 1)
					{
						throw new CompositionException("Too many import methods defined for type: " + type.FullName);
					}

					if (methods.Count == 1)
					{
						MethodInfo method = methods[0];
						ParameterInfo[] methodParameters = method.GetParameters();
						object[] parameters = new object[methodParameters.Length];

						parameters[0] = type;

						for (int i1 = 1; i1 < methodParameters.Length; i1++)
						{
							ParameterInfo methodParameter = methodParameters[i1];
							Type parameterType = methodParameter.ParameterType;

							if (!CompositionContainer.ValidateImportType(parameterType))
							{
								throw new CompositionException("Invalid import type in import method parameter: " + parameterType.Name + " @ " + methodParameter.Name + " @ " + type.FullName);
							}
							if (parameterType == typeof(Import))
							{
								throw new CompositionException("Import method cannot have parameter of type " + typeof(Import).FullName + ": " + parameterType.Name + " @ " + methodParameter.Name + " @ " + type.FullName);
							}

							List<object> parameterValues = this.GetInstancesInternal(CompositionContainer.GetNameOfType(parameterType), parameterType);
							parameters[i1] = parameterValues.Count == 0 ? null : parameterValues[0];
						}

						newInstance = method.Invoke(null, parameters);
					}
				}

				if (newInstance == null)
				{
					ConstructorInfo[] allConstructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					List<ConstructorInfo> constructors = allConstructors.Length == 1 ? new List<ConstructorInfo>(allConstructors) : IEnumerableExtensions.Where(allConstructors, x => x.GetCustomAttributes(typeof(ExportConstructorAttribute), false).Length > 0);

					if (constructors.Count > 1)
					{
						throw new CompositionException("Too many import constructors defined for type: " + type.FullName);
					}

					if (constructors.Count == 0)
					{
						throw new CompositionException("No import method or constructor defined for type: " + type.FullName);
					}

					ConstructorInfo constructor = constructors[0];
					ParameterInfo[] constructorParameters = constructor.GetParameters();
					object[] parameters = new object[constructorParameters.Length];

					for (int i1 = 0; i1 < constructorParameters.Length; i1++)
					{
						ParameterInfo constructorParameter = constructorParameters[i1];
						Type parameterType = constructorParameter.ParameterType;

						if (!CompositionContainer.ValidateImportType(parameterType))
						{
							throw new CompositionException("Invalid import type in import constructor parameter: " + parameterType.Name + " @ " + constructorParameter.Name + " @ " + type.FullName);
						}
						if (parameterType == typeof(Import))
						{
							throw new CompositionException("Import constructor cannot have parameter of type " + typeof(Import).FullName + ": " + parameterType.Name + " @ " + constructorParameter.Name + " @ " + type.FullName);
						}

						List<object> parameterValues = this.GetInstancesInternal(CompositionContainer.GetNameOfType(parameterType), parameterType);
						parameters[i1] = parameterValues.Count == 0 ? null : parameterValues[0];
					}

					newInstance = constructor.Invoke(parameters);
				}

				if (newInstance != null)
				{
					newInstances.Add(newInstance);

					foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
					{
						foreach (CompositionTypeItem typeItem in compositionItem.Value.Types)
						{
							if ((typeItem.Type == type) && (typeItem.Instance == null))
							{
								typeItem.Instance = newInstance;
							}
						}
					}
				}
			}

			foreach (object newInstance in newInstances)
			{
				this.Log("Type added to container: {0} / {1}", name, newInstance.GetType().FullName);

				IExporting exportingInstance = newInstance as IExporting;
				exportingInstance?.AddedToContainer(name, this);
			}

			foreach (object newInstance in newInstances)
			{
				this.ResolveImports(newInstance, CompositionFlags.Constructing);
			}

			HashSetExtensions.AddRange(instances, newInstances);

			return IEnumerableExtensions.Where(instances, compatibleType.IsInstanceOfType);
		}

		private void HandleCatalogRecomposeRequest ()
		{
			this.UpdateComposition(true);
		}

		private void Log (string format, params object[] args)
		{
			LogLocator.LogDebug(this.GetType().Name, format, args);
		}

		private void RaiseCompositionChanged ()
		{
			Action handler = this.CompositionChanged;
			handler?.Invoke();
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

		private void RemoveInstanceInternal (object instance, string name)
		{
			this.Instances.RemoveAll(x => object.ReferenceEquals(x.Value, instance) && CompositionContainer.NameComparer.Equals(x.Name, name));
		}

		private void RemoveTypeInternal (Type type, string name)
		{
			this.Types.RemoveAll(x => (x.Type == type) && CompositionContainer.NameComparer.Equals(x.Name, name));
		}

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

				foreach (KeyValuePair<string, List<CompositionCatalogItem>> item in catalog.Items)
				{
					items.AddRange(item.Value);
				}
			}

			Dictionary<object, HashSet<string>> newInstances = new Dictionary<object, HashSet<string>>();
			foreach (CompositionCatalogItem item in items)
			{
				CompositionItem compositionItem = null;
				if (this.Composition.ContainsKey(item.Name))
				{
					compositionItem = this.Composition[item.Name];
				}
				else
				{
					this.Log("Adding export: {0}", item.Name);
					compositionItem = new CompositionItem(item.Name);
					this.Composition.Add(compositionItem.Name, compositionItem);
				}

				if (item.Value != null)
				{
					if (CompositionContainer.ValidateExportInstance(item.Value))
					{
						CompositionInstanceItem instanceItem = IEnumerableExtensions.FirstOrDefault(compositionItem.Instances, x => object.ReferenceEquals(x.Instance, item.Value));
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
						CompositionTypeItem typeItem = IEnumerableExtensions.FirstOrDefault(compositionItem.Types, x => (x.Type == item.Type));
						if (typeItem == null)
						{
							typeItem = new CompositionTypeItem(item.Type);
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
					                                                                     this.Log("Instance removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.FullName ?? "[null]");
					                                                                     (x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
					                                                                     if (this.AutoDispose)
					                                                                     {
						                                                                     (x?.Instance as IDisposable)?.Dispose();
					                                                                     }
				                                                                     });
				compositionItem.Value.Types.RemoveWhere(x => !x.Checked).ForEach(x =>
				                                                                 {
					                                                                 this.Log("Type removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.FullName ?? "[null]");
					                                                                 (x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
					                                                                 if (this.AutoDispose)
					                                                                 {
						                                                                 (x?.Instance as IDisposable)?.Dispose();
					                                                                 }
				                                                                 });
				compositionItem.Value.ResetChecked();
			}

			IDictionaryExtensions.RemoveWhere(this.Composition, x => (x.Value.Instances.Count == 0) && (x.Value.Types.Count == 0)).ForEach(x => this.Log("Removing export: {0}", x));

			foreach (KeyValuePair<object, HashSet<string>> newInstance in newInstances)
			{
				foreach (string name in newInstance.Value)
				{
					this.Log("Instance added to container: {0} / {1}", name, newInstance.Key.GetType().FullName);

					IExporting exportingInstance = newInstance.Key as IExporting;
					exportingInstance?.AddedToContainer(name, this);
				}
			}

			if (recompose)
			{
				this.Recompose(CompositionFlags.Normal);

				this.RaiseCompositionChanged();
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Dispose(true);
		}

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

			public CompositionTypeItem (Type type)
			{
				this.Checked = false;
				this.Type = type;
				this.Instance = null;
			}

			#endregion




			#region Instance Properties/Indexer

			public bool Checked { get; set; }

			public object Instance { get; set; }

			public Type Type { get; private set; }

			#endregion
		}

		#endregion
	}
}
