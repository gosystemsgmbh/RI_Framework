﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Linq.Expressions;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.ComponentModel;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;
using RI.Framework.Utilities.Runtime;




namespace RI.Framework.Composition
{
    /// <summary>
    ///     The main hub for doing composition or Dependency Injection (DI) / Inversion-of-Control (IoC) respectively.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b> PLATFORMS &amp; AVAILABILITY OF FEATURES </b>
    ///     </para>
    ///     <note type="important">
    ///         Not all features of the <see cref="CompositionContainer" /> are available on all platforms.
    ///         On platforms which use AOT, like IL2CPP, the following features are not available:
    ///         Multiple imports using <see cref="IEnumerable{T}" /> (use <see cref="Import" /> instead), lazy imports using <see cref="Lazy{T}" />, lazy imports using <see cref="Func{TResult}" />, import of any open or closed generic type.
    ///         Exceptions will be thrown when using those features on unsupported platforms.
    ///     </note>
    ///     <para>
    ///         <b> GENERAL </b>
    ///     </para>
    ///     <para>
    ///         Basically, &quot;composition&quot; means to resolve dependencies (called &quot;imports&quot;) according to the known &quot;exports&quot; of a <see cref="CompositionContainer" />.
    ///         The term &quot;container&quot; in <see cref="CompositionContainer" /> already indicates that the exports are contained in the <see cref="CompositionContainer" /> and taken from there to resolve the imports and so satisfy the dependencies.
    ///     </para>
    ///     <para>
    ///         <b> EXPORTS &amp; IMPORTS </b>
    ///     </para>
    ///     <para>
    ///         The terms &quot;export&quot; and &quot;import&quot; might be confusing at first.
    ///         But it is simple:
    ///     </para>
    ///     <para>
    ///         &quot;Export&quot; means that a type or object is being &quot;exported for use&quot; or &quot;provided to a <see cref="CompositionContainer" /> in some way&quot; so that the <see cref="CompositionContainer" /> can use it to resolve imports.
    ///     </para>
    ///     <para>
    ///         &quot;Import&quot; means that &quot;a value of a given type or name is required or requested in some way&quot; and the <see cref="CompositionContainer" /> provides that value (or &quot;imports it for use&quot;) by searching its known exports for an export of the same name as the import.
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
    ///         When exports are managed by a <see cref="CompositionContainer" /> (or a <see cref="CompositionCatalog" />) they are always identified using their name.
    ///         Resolving of imports is also always done using the name of the import.
    ///         Now, when types are used instead of names, the types are simply translated into what is called &quot;the types default name&quot;.
    ///         After the translation, the types are ignored and the exports and imports are continued to be handled using their translated names.
    ///         For example, the method <see cref="AddInstance(object, Type)" /> does nothing else than determine the types default name and then call <see cref="AddInstance(object, string)" /> with that name.
    ///         This allows you to mix type-based import and export (using their types default names) and also name-based import and export (using any custom names you specify).
    ///         Finally, a types default name is simply its namespace and type name, e.g. the name &quot;RI.Framework.Composition.CompositionContainer&quot; for the type <see cref="CompositionContainer" />.
    ///     </para>
    ///     <para>
    ///         In short: Types are never compared or looked-up, only names or the types default names in cases types are used.
    ///     </para>
    ///     <para>
    ///         To get the default name of a type (e.g. for creating a custom <see cref="CompositionCatalog" />), use the method <see cref="GetNameOfType" />.
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
    ///         If you have a graph or cascade of objects, with all kind of dependencies, you do not need to resolve them all by yourself by creating instances, dealing with singletons, or getting manual imports.
    ///         You just make the <see cref="CompositionContainer" /> aware where to find all the possibly required objects or types (using the various concrete implementations of <see cref="CompositionCatalog" />) and then start to pull the objects from the <see cref="CompositionContainer" /> as you need them.
    ///         They will have their imports resolved and filled in.
    ///     </para>
    ///     <para>
    ///         It is a dangerous (and partially wrong!) comparison but you could view a <see cref="CompositionContainer" /> as a kind of very advanced singleton manager which also resolves the dependencies of the singletons it manages.
    ///     </para>
    ///     <para>
    ///         <b> MANUAL &amp; MODEL-BASED EXPORTING </b>
    ///     </para>
    ///     <para>
    ///         There are two ways of exporting: Manual and model-based.
    ///     </para>
    ///     <para>
    ///         Manual export is done by calling one of the <c> AddType </c>, <c> AddInstance </c>, or <c> AddFactory </c> methods of <see cref="CompositionContainer" /> explicitly and stating a type, object, or factory and under which name it is exported.
    ///         Advantage: A type or object does not need any special preparation in order to be exported manually. Any type or object can be exported manually (restrictions apply, see below), even those which requires complex construction which could not be handled by a <see cref="CompositionContainer" />.
    ///         Disadvantage: The type or object to be manually exported must be known and explicitly added to the <see cref="CompositionContainer" />, adding a strong dependency to the corresponding type and/or boilerplate code to discover the type or construct the object.
    ///     </para>
    ///     <para>
    ///         Model-based export is done by using a <see cref="CompositionCatalog" /> and adding it to the <see cref="CompositionContainer" /> using the <see cref="AddCatalog(CompositionCatalog)" /> method.
    ///         Advantage: No dependencies or references to the exported types are required at compile time because, depending on the used <see cref="CompositionCatalog" />, the composition catalog can collect all exports by itself (e.g. all eligible types in an <see cref="Assembly" /> when using <see cref="AssemblyCatalog" />). This makes the things truly &quot;decoupled&quot;!
    ///         Disadvantage: A type needs to fulfill some rules in order to be model-based exportable. This could be: An applied <see cref="ExportAttribute" /> (it depends on the used <see cref="CompositionCatalog" /> whether this attribute is optional or not) and/or simple constructors (either parameterless or only with parameters whose types can be resolved by the <see cref="CompositionContainer" /> when constructing an instance of the type for export).
    ///     </para>
    ///     <para>
    ///         <b> MODEL-BASED EXPORTING &amp; ATTRIBUTES </b>
    ///     </para>
    ///     <para>
    ///         When doing model-based exporting, the <see cref="ExportAttribute" /> is used to declare a type explicitly as exported.
    ///         This might be necessary, depending on the used <see cref="CompositionCatalog" />.
    ///     </para>
    ///     <para>
    ///         Some <see cref="CompositionCatalog" />s can be configured to use all available types, including those without an <see cref="ExportAttribute" />, by using their types default names for exporting.
    ///         The effect is that all types which are visible to the catalog, and which can be exported, are either exported as described above, respecting their <see cref="ExportAttribute" />, or using default names if a type has no <see cref="ExportAttribute" /> defined.
    ///         This can help with exporting/importing types which are not under your control, where you cannot apply a <see cref="ExportAttribute" /> and you do not want to explicitly create instances of those types (where you would lose the loose coupling)).
    ///         Whether a <see cref="CompositionCatalog" /> provides the functionality to export all types depends on the particular implementation of <see cref="CompositionCatalog" />.
    ///         See the description of the <see cref="CompositionCatalog" /> implementations for more details.
    ///     </para>
    ///     <para>
    ///         <see cref="ExportAttribute" /> can also have optional parameters to specify under which type or name a type is export.
    ///         Using the parameterless version of <see cref="ExportAttribute" /> uses the types default name.
    ///     </para>
    ///     <para>
    ///         Multiple <see cref="ExportAttribute" />s can be used on a type to export the same type under multiple types or names.
    ///         In such cases, for non-private exports, the eventually exported instance of the type is shared by all export definitions or in other words: All <see cref="ExportAttribute" />s of a type will point to the same instance.
    ///     </para>
    ///     <para>
    ///         If a type has a <see cref="RI.Framework.Composition.Model.NoExportAttribute" /> on itself or anywhere in its inheritance hierarchy (including interfaces), the type is not exported at all.
    ///     </para>
    ///     <para>
    ///         <b> TYPE / OBJECT / FACTORY EXPORTS </b>
    ///     </para>
    ///     <para>
    ///         Three things can be exported: Types, objects, factories.
    ///         This means that either a <see cref="Type" />, an already instantiated <see cref="object" /> of any type (as long as its a closs or reference type respectively), or a <see cref="Delegate" /> can be used as an export.
    ///     </para>
    ///     <para>
    ///         A type can be exported by specifying the <see cref="Type" /> and optionally under which name it is exported (otherwise using the types default name).
    ///         When an import is resolved to such a type export, a new instance of that type is created (if not already done) or the previously created instance of that type is used and then provided as the import value (except for private exports, see below).
    ///         A type can be exported multiple times under different names.
    ///         It is important to know that the exactly same type is only instantiated once in a <see cref="CompositionContainer" />.
    ///         That means that the one instance of a particular type is used for all exports of that type, even if exported under different names.
    ///         Therefore, type exports are always shared, singleton-like exports (except for private exports, see below).
    ///     </para>
    ///     <para>
    ///         An object can be exported by specifying the <see cref="object" /> and under which name it is exported.
    ///         When an import is resolved to such an object export, the specified object itself is provided as the import value.
    ///         An object can be exported multiple times under different names.
    ///         Unlike type exports, object exports using different instances of the same type are possible.
    ///         However, object exports cannot be private exports (see below).
    ///     </para>
    ///     <para>
    ///         A factory can be used for exporting by providing a <see cref="Delegate" /> which is used to build the eventually exported instance of the type.
    ///         A <see cref="Delegate" /> is specified and also under which name the instance, which is created by the factory delegate, is exported.
    ///         When an import is resolved to such a factory export, the factory delegate is called and its return value is used as the exported value.
    ///         Factory delegates can have parameters, which are treated the same way as type constructor parameters when using type exporting (see section &quot;instance construction&quot; below).
    ///         A factory can be exported multiple times under different names.
    ///         It is important to know that the exactly same factory is only called once in a <see cref="CompositionContainer" />.
    ///         That means that the one instance created by a particular factory is used for all exports which use the same factory, even if exported under different names.
    ///         Therefore, factory exports are always shared, singleton-like exports (except for private exports, see below).
    ///     </para>
    ///     <para>
    ///         Although type and factory exports share one single instance for the same type or factory, it is possible to have type or factory exports of a particular type and also one or more object exports with instances of that same type.
    ///         In such cases, a new shared instance for the type or factory export is still created although there are object exports with instances of the same type.
    ///         Or in other words: Type exports, factory exports, and object exports do not share their instances.
    ///         Type exports share their instances only with other type exports.
    ///         Factory exports share their instances only with other factory exports.
    ///     </para>
    ///     <para>
    ///         <b> INSTANCE CONSTRUCTION </b>
    ///     </para>
    ///     <para>
    ///         A <see cref="CompositionContainer" /> creates new instances for type exports to get the actual exported value.
    ///         Basically, those instances are created in a normal way by using type constructors, with some exceptions.
    ///     </para>
    ///     <para>
    ///         <see cref="ExportConstructorAttribute" />, <see cref="ExportCreatorAttribute" />, and <see cref="CompositionCreator" /> are used to help with the construction of instances for type exports.
    ///         If, for a particlar type, more than one approach is available to get an instance, the priority is as follows: creator methods (<see cref="ExportCreatorAttribute" />, then constructors (with or without <see cref="ExportConstructorAttribute" />), then <see cref="CompositionCreator" />.
    ///         If no creator method is specified or a specified creator method yields no usable results (means: returns null), constructors are tried.
    ///         If no suitable constructors are available AND there is a <see cref="CompositionCreator" /> supporting the type, that <see cref="CompositionCreator" /> is used.
    ///         Only one constructor per type is allowed to have the <see cref="ExportConstructorAttribute" />.
    ///         Only one method per type is allowed to have the <see cref="ExportCreatorAttribute" />.
    ///     </para>
    ///     <para>
    ///         The rules used to determine which constructor to use, in case there are multiple constructors defined for a type, are as follows:
    ///         All constructors, public and non-public, with and without parameters, with or without <see cref="ExportConstructorAttribute" />, are considered.
    ///         The one constructor marked with <see cref="ExportConstructorAttribute" /> has first priority (basically saying &quot;this is the one, regardless what other constructors are there&quot;).
    ///         All constructors for which ALL of their parameters can be satisfied with import values have second priority.
    ///         All constructors for which NONE or NOT ALL of their parameters can be satisfied with import values have third priority.
    ///         Within the same priority, the one constructor with the most parameters has the highest sub-priority.
    ///     </para>
    ///     <para>
    ///         <see cref="ExportCreatorAttribute" />s can be used on a type to specify a static method which creates instances of that type.
    ///         This can be helpful in cases the instance creation itself is too complex so it cannot be handled by the <see cref="CompositionContainer" /> itself.
    ///         Any static method of the type which is to be created can be declared the types creator method by applying <see cref="ExportCreatorAttribute" />.
    ///         The return value of the method is taken as the constructed instance of the type (even if the type is not the same type as the method belongs to).
    ///     </para>
    ///     <para>
    ///         <see cref="CompositionCreator" />s can be used to provide a mechanism which is more decoupled from the constructed type than an <see cref="ExportCreatorAttribute" /> would be (the <see cref="CompositionCreator" /> knows the type to construct, but not vice-versa).
    ///         This can be helpful in cases the instance creation itself is too complex so it cannot be handled by the <see cref="CompositionContainer" /> itself.
    ///         <see cref="CompositionCreator" />s are added to the <see cref="CompositionContainer" /> (using <see cref="AddCreator" />).
    ///         A <see cref="CompositionCreator" /> decides by itself whether it is responsible for creating a particular type (using the <see cref="CompositionCreator.CanCreateInstance" /> method).
    ///         If a <see cref="CompositionCreator" /> decides to be responsible for creating an instance, the <see cref="CompositionCreator.CreateInstance" /> method is used.
    ///     </para>
    ///     <para>
    ///         The same principles now described for the constructor parameters also apply to factory delegate parameters and parameters for constructor methods specified by <see cref="ExportCreatorAttribute" />:
    ///     </para>
    ///     <para>
    ///         A parameterless constructor is simple: It is just called and if the constructor itself does not crash, there are no side-effects.
    ///         A constructor with parameters is more complex: There are three kinds of parameters which are handled differently: <see cref="Type" />, <see cref="string" />, and any other type.
    ///         A <see cref="Type" /> parameter is always satisfied with the type currently being under construction.
    ///         A <see cref="string" /> parameter is always satisfied with the name under which the currently exported type being under construction is experted.
    ///         Any other type is satisfied by attempting a &quot;manual&quot; import based on the parameter type, similar as if one of the <c> GetExport </c> methods would be used to get the value for the parameter.
    ///         This also includes the ability to have parameters which use &quot;multiple importing&quot; or &quot;lazy importing&quot;.
    ///     </para>
    ///     <para>
    ///         <b> SHARED &amp; PRIVATE EXPORTS </b>
    ///     </para>
    ///     <para>
    ///         Another distinction for exporting is shared and private exports.
    ///     </para>
    ///     <para>
    ///         Shared exports behave exactly like described above.
    ///         For type exports, the instance which is created for a type is used (shared) for all imports of the same type.
    ///         For factory exports, the instance which is created for a factory delegate is used (shared) for all imports of the same factory delegate.
    ///         For object exports, the instance is used (shared) for all imports of the same name.
    ///     </para>
    ///     <para>
    ///         Private exports behave more or less also the same as described above, with three exceptions:
    ///         For type exports, imports will receive their own (private) instance each time (!) an import is resolved, meaning: a new instance of the type is constructed for each import.
    ///         For factory exports, imports will receive their own (private) instance each time (!) an import is resolved, meaning: the factory delegate is called for each import.
    ///         Object exports cannot be private and are always shared.
    ///     </para>
    ///     <para>
    ///         Therefore, only the export side can control whether an export or import respectively is shared or private.
    ///         The import side has no control over whether its import value is shared or private.
    ///     </para>
    ///     <para>
    ///         An export can be made private through the <see cref="ExportAttribute.Private" /> property of <see cref="ExportAttribute" /> or by specifying so when using one of the methods for manual exporting.
    ///     </para>
    ///     <para>
    ///         <b> CLOSED &amp; OPEN GENERICS </b>
    ///     </para>
    ///     <para>
    ///         <see cref="CompositionContainer" /> supports both closed and open generics as exports.
    ///     </para>
    ///     <para>
    ///         Closed generic: A concrete type of a generic type is exported.
    ///         Example: <c> container.AddType(typeof(MyType&lt;string&gt;), typeof(MyType&lt;string&gt;), false); </c>.
    ///         Here, an instance of <c> MyType&lt;string&gt; </c> is resolved and created when an import for <c> MyType&lt;string&gt; </c> is requested.
    ///         If an import for <c> MyType&lt;int&gt; </c> would be requested, nothing would be resolved and no instance is created.
    ///     </para>
    ///     <para>
    ///         Open generic: An open type of a generic type is exported, not specifying the type parameters.
    ///         Example: <c> container.AddType(typeof(MyType&lt;&gt;), typeof(MyType&lt;&gt;), false); </c>.
    ///         Here, an instance of <c> MyType&lt;string&gt; </c> is resolved and created when an import for <c> MyType&lt;string&gt; </c> is requested.
    ///         But then if an import for <c> MyType&lt;int&gt; </c> would be requested, <c> MyType&lt;int&gt; </c> would be resolved and created.
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
    ///         Model-based import is done by decorating properties of composed types with <see cref="ImportAttribute" />s.
    ///         The <see cref="CompositionContainer" /> then automatically fills in the values for those properties, based on the <see cref="ImportAttribute" />.
    ///         This is usually used for implementing Dependency Injection (DI) or to retrieve an export from the <see cref="CompositionContainer" /> declaratively.
    ///     </para>
    ///     <para>
    ///         <b> EXPLICIT &amp; IMPLICIT MODEL-BASED IMPORTING </b>
    ///     </para>
    ///     <para>
    ///         Model-based import comes in two flavours: Explicit and implicit.
    ///     </para>
    ///     <para>
    ///         Explicit model-based importing is done by passing an object to the <see cref="ResolveImports(object, CompositionFlags)" /> method.
    ///         The specified object then gets all its imports, which have an <see cref="ImportAttribute" /> applied, resolved and assigned once.
    ///         The passed object is not tracked by the <see cref="CompositionContainer" />.
    ///         See <see cref="ResolveImports(object, CompositionFlags)" /> for details.
    ///     </para>
    ///     <para>
    ///         Implicit model-based import is done automatically by the <see cref="CompositionContainer" /> itself by resolving the imports of all its known exports.
    ///         The imports of the known exports of a <see cref="CompositionContainer" /> are resolved whenever the composition changes (e.g. a manual export method is used or a composition catalog is added or removed) or a recomposition is executed (using the <see cref="Recompose(CompositionFlags)" /> method).
    ///         In other words: All exports in a <see cref="CompositionContainer" /> get their imports, using <see cref="ImportAttribute" />, automatically resolved and filled in.
    ///     </para>
    ///     <para>
    ///         <b> MODEL-BASED IMPORTING &amp; ATTRIBUTES </b>
    ///     </para>
    ///     <para>
    ///         When doing model-based importing, the <see cref="ImportAttribute" /> is used to declare what is eventually to be assigned to the property as its imported value.
    ///     </para>
    ///     <para>
    ///         Using the parameterless version of <see cref="ImportAttribute" /> uses the default name of the property type for importing.
    ///     </para>
    ///     <para>
    ///         Other versions of <see cref="ImportAttribute" /> allows to specify a type or name which is to be imported.
    ///         In such cases, the imported value assigned to the property must be compatible with the property type.
    ///         An exception will occur if the imported value can not be assigned to the type of the property (e.g. when the import value type and the property type are not the same or the property type is not in the inheritance hierarchy of the import value type).
    ///     </para>
    ///     <para>
    ///         <b> RECOMPOSITION &amp; TRACKING </b>
    ///     </para>
    ///     <para>
    ///         Model-based imports can be marked as &quot;recomposable&quot; using <see cref="ImportAttribute" />.<see cref="ImportAttribute.Recomposable" />.
    ///         This means that the imports of such properties are reimported or resolved again respectively whenever the composition for the corresponding name changes (e.g. a new export of that name gets added to the <see cref="CompositionContainer" />).
    ///         However, this is only done for implicit model-based importing, meaning that only exports known to the <see cref="CompositionContainer" /> get their recomposable imports updated.
    ///         Imports resolved using <see cref="ResolveImports(object, CompositionFlags)" /> are not updated during a recomposition because they are not implicitly part of the <see cref="CompositionContainer" /> and therefore not tracked.
    ///         See <see cref="Recompose(CompositionFlags)" /> and <see cref="ResolveImports(object, CompositionFlags)" /> for more details.
    ///     </para>
    ///     <para>
    ///         Model-based imports which are not recomposable remain their imported value, even if the corresponding export gets removed from the <see cref="CompositionContainer" />.
    ///         Therefore, model-based imports which are expected to change must be made recomposable.
    ///         <see cref="IImporting" /> can be used to detect when imports have changed.
    ///     </para>
    ///     <para>
    ///         <b> IMPORT KINDS </b>
    ///     </para>
    ///     <para>
    ///         Five fundamentally different kinds of providing import values are available: one normal way (providing a single value), two ways providing multiple values, and two ways providing the values in a &quot;lazy&quot; style.
    ///         The used kind is determined by the target type where the import value goes, therefore applicable to model-based importing using <see cref="ImportAttribute" /> and parameters of factory delegates, constructors, and methods specified by <see cref="ExportCreatorAttribute" />.
    ///     </para>
    ///     <para>
    ///         <b> Single </b>:
    ///         The target type is of any reference type, except one of the other types which define the other import kinds.
    ///         For example a property of the type <see cref="ILogger" />.
    ///         The property gets a single instance of <see cref="ILogger" /> or null if the <see cref="CompositionContainer" /> has no matching export.
    ///         If there are multiple matching exports, one of them is used but it is undefined which one.
    ///     </para>
    ///     <para>
    ///         <b> Multiple </b>:
    ///         The target type is <see cref="IEnumerable{T}" />.
    ///         For example a property of the type <see cref="IEnumerable{ILogger}" />.
    ///         The <see cref="CompositionContainer" /> internally creates an array of <see cref="ILogger" />s and assigns it to the property so that it gets all matching exports.
    ///         If there no matching exports, an empty array would be assigned.
    ///         Note that only <see cref="IEnumerable{T}" /> is used to detect multiple imports so you cannot use the array type directly.
    ///         You should also never cast back to an array as this is only considered an internal mechanism which might change in a future version (the only thing guaranteed is that the value is of a type which implements <see cref="IEnumerable{T}" />).
    ///     </para>
    ///     <para>
    ///         <b> Special Multiple </b>:
    ///         The target type is <see cref="Import" />.
    ///         For example a property of the type <see cref="Import" />.
    ///         The <see cref="CompositionContainer" /> internally creates an instance of <see cref="Import" /> and fills it with the import values of all matching exports, then assigns it to the property so that it gets all these matching exports indirectly through the <see cref="Import" /> instance.
    ///         There will be always an <see cref="Import" /> instance, even when there are no matching exports.
    ///         To retrieve the actual imported values from <see cref="Import" />, the extension methods from <see cref="ImportExtensions" /> are used.
    ///         The reason why this kind of import exists is to provide an AOT-compliant way of providing more than one import values (as the <see cref="IEnumerable{T}" /> approach cannot be used with AOT).
    ///         Note that for this kind of import, the corresponding property or parameter must be assigned with an <see cref="ImportAttribute" /> which specifies the name or type to be imported (otherwise there would be no indication about what is actually to be imported, no name or type).
    ///     </para>
    ///     <para>
    ///         <b> Lazy Method </b>:
    ///         The target type is <see cref="Func{TResult}" />.
    ///         For example a property of the type <see cref="Func{ILogger}" />.
    ///         The property gets a delegate which can be called when the actual instance of <see cref="ILogger" /> is really needed, only resolving the import value on-demand (or &quot;lazy&quot; respectively).
    ///         So the resolving of the import VALUE happens at the time the delegate is called and not at the time the import ITSELF is resolved (including any potential construction of instances).
    ///     </para>
    ///     <para>
    ///         <b> Lazy Object </b>:
    ///         The target type is <see cref="Lazy{TResult}" />.
    ///         For example a property of the type <see cref="Lazy{ILogger}" />.
    ///         The property gets an instance of <see cref="Lazy{ILogger}" /> where the property <see cref="Lazy{TResult}.Value" /> is used when the actual instance of <see cref="ILogger" /> is really needed, only resolving the import value on-demand (or &quot;lazy&quot; respectively).
    ///         So the resolving of the import VALUE happens at the time the <see cref="Lazy{TResult}.Value" /> is used and not at the time the import ITSELF is resolved (including any potential construction of instances).
    ///     </para>
    ///     <para>
    ///         Some of the import kinds can be cascaded.
    ///         If the the lazy method or the lazy object kind is used, the type resolved by <see cref="Func{TResult}" /> or <see cref="Lazy{T}" /> can be used to define the single kind (e.g. <c> Func&lt;ILogger&gt; </c>) or multiple kind (e.g. <c> Func&lt;IEnumerable&lt;ILogger&gt;&gt; </c>) (but not special multiple kind).
    ///     </para>
    ///     <para>
    ///         <b> ELIGIBLE TYPES </b>
    ///     </para>
    ///     <para>
    ///         Only non-abstract reference types can be exported.
    ///     </para>
    ///     <para>
    ///         Only class or interface types can be imported.
    ///     </para>
    ///     <para>
    ///         <b> COMPOSITION AWARENESS THROUGH INTERFACES </b>
    ///     </para>
    ///     <para>
    ///         Types which are managed by the <see cref="CompositionContainer" /> as exports and types into which the <see cref="CompositionContainer" /> imports can be made composition-aware by implementing the proper interfaces.
    ///     </para>
    ///     <para>
    ///         <see cref="IExporting" />: Allows exported types and objects to be informed when they are added to or removed from a <see cref="CompositionContainer" />.
    ///     </para>
    ///     <para>
    ///         <see cref="IImporting" />: Allows types and objects which use model-based importing (using <see cref="ImportAttribute" />) to be informed when their imports are being resolved or updated.
    ///         This interface is very helpful if you have a decoupled environment where exports/imports can change at any time and a type needs to be informed when that happens.
    ///     </para>
    ///     <note type="note">
    ///         Be careful when using those interfaces, especially with <see cref="IExporting" />.
    ///         Using them in the wrong way could lead to tight structural and/or tight dynamic coupling.
    ///     </note>
    ///     <para>
    ///         <b> BATCHES </b>
    ///     </para>
    ///     <para>
    ///         Multiple composition and <see cref="CompositionContainer" /> operations can be combined into a single batch.
    ///         <see cref="CompositionBatch" /> can be used to collect multiple operations which are then executed in one batch, using <see cref="Compose" />.
    ///     </para>
    ///     <para>
    ///         Using batches, a recomposition is only triggered once for the whole batch, instead of doing a recomposition for each operation.
    ///     </para>
    ///     <para>
    ///         <b> SELF-COMPOSITION </b>
    ///     </para>
    ///     <para>
    ///         A <see cref="CompositionContainer" /> does not add itself as an export by default but can be added as an export like any other object (the <see cref="CompositionContainer" /> has also multiple <see cref="ExportAttribute" />s applied).
    ///         A <see cref="CompositionContainer" /> which has an export of another <see cref="CompositionContainer" /> does only export that other <see cref="CompositionContainer" /> instance the same way as it does any other object or type but does not also export the exports of that other <see cref="CompositionContainer" /> (no flattening of the export hierarchy, this can be done using parent/child containers).
    ///     </para>
    ///     <para>
    ///         <b> PARENT/CHILD CONTAINERS </b>
    ///     </para>
    ///     <para>
    ///         When constructing a <see cref="CompositionContainer" />, a parent container can be specified (see <see cref="CompositionContainer(CompositionContainer)" />).
    ///         The constructed <see cref="CompositionContainer" /> then inherits all exports of its parent container and also reflects the changes in composition of its parent container.
    ///         It behaves as if the exports of the parent container are exports of the child container.
    ///     </para>
    ///     <para>
    ///         A parent conatiner does not get the exports of the child containers and is also otherwise not affected in any way by child containers.
    ///         A container can be parent of multiple child containers.
    ///     </para>
    ///     <para>
    ///         The only way to detach from a parent container is through <see cref="Dispose" /> but no other parent container can be attached afterwards.
    ///     </para>
    ///     <para>
    ///         Instead of creating a new child contianer and pass the parrent container, a child container can be created with <see cref="CreateChildContainer" /> (which does basically the same).
    ///     </para>
    ///     <para>
    ///         <b> MULTIPLE COMPOSITION CONTAINERS </b>
    ///     </para>
    ///     <para>
    ///         Multiple composition containers can coexist independently side-by-side and do not affect each other at all.
    ///         They can also share the same exports and <see cref="CompositionCatalog" />s.
    ///     </para>
    ///     <para> SIMPLE USAGE / ROOT SINGLETON </para>
    ///     <para>
    ///         A very simple usage of <see cref="CompositionContainer" /> and the principles of Dependency Injection is the creation of one <see cref="CompositionContainer" /> instance and use it where necessary.
    ///     </para>
    ///     <para>
    ///         This can be done using your own mechanism to create, store, and distribute instances of <see cref="CompositionContainer" /> throughout your application or you can use the built-in singleton mechanism.
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
    ///         In the simplest case, depending on the used extension methods, you would not need to deal with <see cref="CompositionContainer" /> AT ALL.
    ///     </para>
    ///     <para> ADVANCED USAGE / BOOTSTRAPPER </para>
    ///     <para>
    ///         In more advanced scenarios, where the application or game requires bootstrapping of its various components and subsystems, a <see cref="CompositionContainer" /> is one of the core components for bootstrappers.
    ///         See the various bootstrapper implementations for more information about bootstrappers and their use of <see cref="CompositionContainer" />.
    ///     </para>
    ///     <para>
    ///         <b> EXCEPTIONS &amp; UNDEFINED STATE </b>
    ///     </para>
    ///     <para>
    ///         If a <see cref="CompositionException" /> is thrown during a composition, the state of the <see cref="CompositionContainer" /> and all its compositions are undefined and might remain unusable.
    ///         Therefore, a <see cref="CompositionException" /> should always be treated as a fatal error which prevents the program from continueing normally.
    ///     </para>
    ///     <para>
    ///         <b> PERFORMANCE </b>
    ///     </para>
    ///     <para>
    ///         Be aware that doing composition are usually very costly operations and should be avoided in performance or time critical paths.
    ///         Therefore, composition is usually done during startup or when the application composition changes (e.g. enabling/disabling mods or plugins) to bring everything in place and resolve all dependencies.
    ///         The performance impact for composition operations depend on various factors, including the used <see cref="CompositionCatalog" />s, and cannot be generally stated beyond &quot;its costly&quot;.
    ///     </para>
    ///     <para>
    ///         <b> THREAD-SAFETY </b>
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
    [Export(typeof(IDependencyResolver))]
    [Export(typeof(IServiceProvider))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class CompositionContainer : LogSource, IDependencyResolver, IServiceProvider, IDisposable, ISynchronizable
    {
        #region Constants

        internal static readonly StringComparerEx NameComparer = StringComparerEx.Ordinal;

        #endregion




        #region Static Constructor/Destructor

        static CompositionContainer ()
        {
            CompositionContainer.GlobalSyncRoot = new object();
            CompositionContainer.ResolveImports_PropertyCache = new Dictionary<Type, ResolveImports_PropertyInfo[]>();
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

        private static Dictionary<Type, ResolveImports_PropertyInfo[]> ResolveImports_PropertyCache { get; }

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
        ///         <see cref="CreateSingletonWithEverything" /> does the same as <see cref="CreateSingleton" /> except that, if a singleton instance is created, it also adds an <see cref="AppDomainCatalog" /> to immediately get all types in the current application domain as exports (excluding framework-provided types).
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

            if (!CompositionContainer.GetExportsOfTypeInternal(type, includeWithoutAttribute, true, exports))
            {
                exports.Clear();
                return exports;
            }

            List<Type> inheritedTypes = type.GetInheritance(false);
            foreach (Type inheritedType in inheritedTypes)
            {
                if (!CompositionContainer.GetExportsOfTypeInternal(inheritedType, includeWithoutAttribute, false, exports))
                {
                    exports.Clear();
                    return exports;
                }
            }

            Type[] interfaceTypes = type.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (!CompositionContainer.GetExportsOfTypeInternal(interfaceType, includeWithoutAttribute, false, exports))
                {
                    exports.Clear();
                    return exports;
                }
            }

            exports.RemoveWhere(x => x == null);

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
        ///     Validates whether a factory can be used for creating exports.
        /// </summary>
        /// <param name="factory"> The factory to validate. </param>
        /// <returns>
        ///     true if the factory can be used by a <see cref="CompositionContainer" /> to create exports, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         A factory can be used for creating exports if xxx.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> is null. </exception>
        public static bool ValidateExportFactory (Delegate factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return (factory.Method.ReturnType.IsClass || factory.Method.ReturnType.IsInterface) && (factory.Method.ReturnType != typeof(void)) && (!factory.Method.GetParameters().Any(x => !CompositionContainer.ValidateImportType(x.ParameterType)));
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
        /// <exception cref="ArgumentNullException"> <paramref name="instance" /> is null. </exception>
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
                container.AddInstance(container, typeof(CompositionContainer));
                container.AddInstance(container, typeof(IDependencyResolver));
                container.AddInstance(container, typeof(IServiceProvider));
                if (addAppDomainCatalog)
                {
                    container.AddCatalog(new AppDomainCatalog(true, true, true));
                }

                ServiceLocator.BindToDependencyResolver(container);
            }

            return container;
        }


        private static Type GetEnumerableType (Type type)
        {
            Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
            return (genericType == typeof(IEnumerable<>)) ? typeArgument : null;
        }

        private static bool GetExportsOfTypeInternal (Type type, bool includeWithoutAttribute, bool isSelf, HashSet<string> exports)
        {
            object[] exportAttributes = type.GetCustomAttributes(typeof(ExportAttribute), false);
            object[] noExportAttributes = type.GetCustomAttributes(typeof(NoExportAttribute), false);

            if (noExportAttributes.Length > 0)
            {
                return !isSelf;
            }
            else if (exportAttributes.Length > 0)
            {
                foreach (ExportAttribute attribute in exportAttributes)
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

            return true;
        }

        private static Type GetLazyLoadFuncType (Type type)
        {
            Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
            int typeArgumentCount = type.IsGenericType ? type.GetGenericArguments().Length : 0;
            return ((genericType == typeof(Func<>)) && (typeArgumentCount == 1)) ? typeArgument : null;
        }

        private static Type GetLazyLoadObjectType (Type type)
        {
            Type genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            Type typeArgument = type.IsGenericType ? type.GetGenericArguments()[0] : null;
            return (genericType == typeof(Lazy<>)) ? typeArgument : null;
        }

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

        private static ResolveImports_PropertyInfo[] ResolveImports_GetProperties (Type type)
        {
            lock (CompositionContainer.GlobalSyncRoot)
            {
                if (!CompositionContainer.ResolveImports_PropertyCache.ContainsKey(type))
                {
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    ResolveImports_PropertyInfo[] infos = new ResolveImports_PropertyInfo[properties.Length];
                    for (int i1 = 0; i1 < properties.Length; i1++)
                    {
                        PropertyInfo property = properties[i1];
                        ResolveImports_PropertyInfo info = new ResolveImports_PropertyInfo();
                        info.ImportType = property.PropertyType;
                        info.Property = property;
                        info.GetMethod = property.GetGetMethod(true);
                        info.SetMethod = property.GetSetMethod(true);
                        info.ImportAttributes.AddRange(property.GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>());
                        info.CanRecompose = info.ImportAttributes.Any(x => x.Recomposable);
                        info.ImportName = info.ImportAttributes.FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
                        infos[i1] = info;
                    }

                    CompositionContainer.ResolveImports_PropertyCache.Add(type, infos);
                }

                return CompositionContainer.ResolveImports_PropertyCache[type];
            }
        }

        private static bool ValidateImportType (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsClass || type.IsInterface;
        }

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CompositionContainer" />.
        /// </summary>
        public CompositionContainer ()
        {
            this.SyncRoot = new object();

            this.IsClearing = false;

            this.CatalogRecomposeRequestHandler = this.HandleCatalogRecomposeRequest;
            this.ParentContainerCompositionChangedHandler = this.HandleParentContainerCompositionChanged;

            this.AutoDispose = true;

            this.ParentContainer = null;

            this.Instances = new List<CompositionCatalogItem>();
            this.Types = new List<CompositionCatalogItem>();
            this.Factories = new List<CompositionCatalogItem>();
            this.Catalogs = new List<CompositionCatalog>();
            this.Creators = new List<CompositionCreator>();
            this.Composition = new Dictionary<string, CompositionItem>(CompositionContainer.NameComparer);
            this.LazyInvokers = new Dictionary<string, Dictionary<Type, LazyInvoker>>(CompositionContainer.NameComparer);
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

        private List<CompositionCreator> Creators { get; }

        private List<CompositionCatalogItem> Factories { get; }

        private List<CompositionCatalogItem> Instances { get; }

        private bool IsClearing { get; set; }

        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private Dictionary<string, Dictionary<Type, LazyInvoker>> LazyInvokers { get; }

        private EventHandler ParentContainerCompositionChangedHandler { get; }

        private List<CompositionCatalogItem> Types { get; }

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
        ///     Manual export: Adds a factory and exports it under the specified types default name for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportType"> The type under whose default name the factory is exported. </param>
        /// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
        /// <remarks>
        ///     <para>
        ///         If the specified factory is already exported under the specified name, the composition remains unchanged.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="factory" /> is not of a type which can be used for composition. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void AddFactory (Delegate factory, Type exportType, bool privateExport)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.AddFactory(factory, CompositionContainer.GetNameOfType(exportType), privateExport);
        }

        /// <summary>
        ///     Manual export: Adds a factory and exports it under the specified name for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportName"> The name under which the factory is exported. </param>
        /// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
        /// <remarks>
        ///     <para>
        ///         If the specified factory is already exported under the specified name, the composition remains unchanged.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportName" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="factory" /> is not of a type which can be used for composition. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void AddFactory (Delegate factory, string exportName, bool privateExport)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!CompositionContainer.ValidateExportFactory(factory))
            {
                throw new InvalidTypeArgumentException(nameof(factory));
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
                this.AddFactoryInternal(factory, exportName, privateExport);
                this.UpdateComposition(true);
            }

            this.RaiseCompositionChanged();
        }

        /// <summary>
        ///     Manual export: Adds a factory and exports it under the specified types default name for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportType"> The type under whose default name the factory is exported. </param>
        /// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
        /// <remarks>
        ///     <para>
        ///         If the specified factory is already exported under the specified name, the composition remains unchanged.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="factory" /> is not of a type which can be used for composition. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void AddFactory (Func<object> factory, Type exportType, bool privateExport) => this.AddFactory((Delegate)factory, exportType, privateExport);

        /// <summary>
        ///     Manual export: Adds a factory and exports it under the specified name for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportName"> The name under which the factory is exported. </param>
        /// <param name="privateExport"> Specifies whether the export is private (true) or shared (false). </param>
        /// <remarks>
        ///     <para>
        ///         If the specified factory is already exported under the specified name, the composition remains unchanged.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportName" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="factory" /> is not of a type which can be used for composition. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void AddFactory (Func<object> factory, string exportName, bool privateExport) => this.AddFactory((Delegate)factory, exportName, privateExport);

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
        public void AddInstance (object instance, Type exportType)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.AddInstance(instance, CompositionContainer.GetNameOfType(exportType));
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
        public void AddInstance (object instance, string exportName)
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
        public void AddType (Type type, Type exportType, bool privateExport)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.AddType(type, CompositionContainer.GetNameOfType(exportType), privateExport);
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
        public void AddType (Type type, string exportName, bool privateExport)
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
        ///         All catalogs, creators, objects, types, and factories are removed.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void Clear ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsClearing)
                {
                    return;
                }

                try
                {
                    this.IsClearing = true;

                    this.Log(LogLevel.Debug, "Clearing container.");

                    this.ClearInternal();
                    this.UpdateComposition(true);
                }
                finally
                {
                    this.IsClearing = false;
                }
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
                    else if (item.Factory != null)
                    {
                        this.AddFactoryInternal(item.Factory, item.Name, item.PrivateExport);
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
                    else if (item.Factory != null)
                    {
                        this.RemoveFactoryInternal(item.Factory, item.Name);
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
        ///     Creates a text describing the current composition.
        /// </summary>
        /// <param name="writer"> The text writer to write the description to. </param>
        public void GetCurrentCompositionLog (TextWriter writer)
        {
            Dictionary<string, List<CompositionCatalogItem>> compositionSnapshot = this.GetCompositionSnapshot();

            List<string> names = compositionSnapshot.Keys.ToList();
            names.Sort(StringComparerEx.InvariantCultureIgnoreCase);

            for (int i1 = 0; i1 < names.Count; i1++)
            {
                bool last = i1 >= (names.Count - 1);
                string name = names[i1];
                List<CompositionCatalogItem> catalogItems = compositionSnapshot[name];

                List<string> lines = new List<string>();
                lines.Add(name);
                foreach (CompositionCatalogItem catalogItem in catalogItems)
                {
                    if (catalogItem.Value != null)
                    {
                        lines.Add(string.Format(CultureInfo.InvariantCulture, "  Kind=Instance, Private=0, Value= {0} ({1})", catalogItem.Value.GetType().Name, catalogItem.Value.GetType().AssemblyQualifiedName));
                    }
                    else if (catalogItem.Type != null)
                    {
                        lines.Add(string.Format(CultureInfo.InvariantCulture, "  Kind=Type,     Private={0}, Value= {1} ({2})", catalogItem.PrivateExport ? "1" : "0", catalogItem.Type.Name, catalogItem.Type.AssemblyQualifiedName));
                    }
                    else if (catalogItem.Factory != null)
                    {
                        lines.Add(string.Format(CultureInfo.InvariantCulture, "  Kind=Factory,  Private={0}, Value= {1}", catalogItem.PrivateExport ? "1" : "0", catalogItem.Factory.GetFullName()));
                    }
                }

                int separatorLength = lines.MaxLength();
                string separator = new string('-', separatorLength);

                writer.WriteLine(separator);

                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }

                if (last)
                {
                    writer.WriteLine(separator);
                }
            }
        }

        /// <summary>
        ///     Creates a text describing the current composition.
        /// </summary>
        /// <returns>
        ///     The text describing the current composition.
        /// </returns>
        public string GetCurrentCompositionLog ()
        {
            using (StringWriter sw = new StringWriter())
            {
                this.GetCurrentCompositionLog(sw);
                sw.Flush();
                return sw.ToString().Trim();
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
        public T GetExport <T> ()
            where T : class
        {
            lock (this.SyncRoot)
            {
                return (T)this.GetImportValueFromNameOrType(null, typeof(T), out _);
            }
        }

        /// <summary>
        ///     Manual import: Gets the first resolved value for the specified types default name.
        /// </summary>
        /// <param name="exportType"> The type whose default name is resolved. </param>
        /// <returns>
        ///     The first resolved value which is exported under the specified types default name, null if no such value could be resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="exportType" /> is null. </exception>
        /// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
        public object GetExport (Type exportType)
        {
            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            lock (this.SyncRoot)
            {
                return this.GetImportValueFromNameOrType(null, exportType, out _);
            }
        }

        /// <summary>
        ///     Manual import: Gets the first resolved value for the specified name.
        /// </summary>
        /// <param name="exportName"> The name which is resolved. </param>
        /// <returns>
        ///     The first resolved value which is exported under the specified name, null if no such value could be resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="exportName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
        public object GetExport (string exportName)
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
                return this.GetImportValueFromNameOrType(exportName, typeof(object), out _);
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
            lock (this.SyncRoot)
            {
                if (RuntimeEnvironment.IsUnityRuntime())
                {
                    return ((Import)this.GetImportValueFromNameOrType(CompositionContainer.GetNameOfType(typeof(T)), typeof(Import), out _)).ToList<T>();
                }
                else
                {
                    return ((IEnumerable<T>)this.GetImportValueFromNameOrType(null, typeof(IEnumerable<T>), out _)).ToList();
                }
            }
        }

        /// <summary>
        ///     Manual import: Gets all resolved values for the specified types default name.
        /// </summary>
        /// <param name="exportType"> The type whose default name is resolved. </param>
        /// <returns>
        ///     The list containing the resolved values.
        ///     The list is empty if no values could be resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="exportType" /> is null. </exception>
        /// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
        public List<object> GetExports (Type exportType)
        {
            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            lock (this.SyncRoot)
            {
                if (RuntimeEnvironment.IsUnityRuntime())
                {
                    return ((Import)this.GetImportValueFromNameOrType(CompositionContainer.GetNameOfType(exportType), typeof(Import), out _)).ToList<object>();
                }
                else
                {
                    return ((IEnumerable)this.GetImportValueFromNameOrType(null, typeof(IEnumerable<>).MakeGenericType(exportType), out _)).ToList();
                }
            }
        }

        /// <summary>
        ///     Manual import: Gets all resolved values for the specified name.
        /// </summary>
        /// <param name="exportName"> The name which is resolved. </param>
        /// <returns>
        ///     The list containing the resolved values.
        ///     The list is empty if no values could be resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="exportName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The resolving failed although matching exports were found. </exception>
        public List<object> GetExports (string exportName)
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
                if (RuntimeEnvironment.IsUnityRuntime())
                {
                    return ((Import)this.GetImportValueFromNameOrType(exportName, typeof(Import), out _)).ToList<object>();
                }
                else
                {
                    return ((IEnumerable<object>)this.GetImportValueFromNameOrType(exportName, typeof(IEnumerable<object>), out _)).ToList();
                }
            }
        }

        /// <summary>
        ///     Creates a text describing the current composition and writes it to the log.
        /// </summary>
        public void LogCurrentComposition (LogLevel severity)
        {
            string currentCompositionLog = this.GetCurrentCompositionLog();
            this.Log(severity, "Current composition:{0}{1}", Environment.NewLine, currentCompositionLog);
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
                for (int i1 = 0; i1 < instances.Count; i1++)
                {
                    object instance = instances[i1];
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
        ///     Manual export: Removes a factory exported under the specified types default name so that it is no longer used for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportType"> The type under whose default name the factory is exported. </param>
        /// <remarks>
        ///     <para>
        ///         Only the export matching the specified factory is removed.
        ///         If the same factory is also exported under other types or names, those exports remain intact.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void RemoveFactory (Delegate factory, Type exportType)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.RemoveFactory(factory, CompositionContainer.GetNameOfType(exportType));
        }

        /// <summary>
        ///     Manual export: Removes a factory exported under the specified name so that it is no longer used for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportName"> The name under which the factory is exported. </param>
        /// <remarks>
        ///     <para>
        ///         Only the export matching the specified factory and name is removed.
        ///         If the same factory is also exported under other types or names, those exports remain intact.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void RemoveFactory (Delegate factory, string exportName)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
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
                this.RemoveFactoryInternal(factory, exportName);
                this.UpdateComposition(true);
            }

            this.RaiseCompositionChanged();
        }

        /// <summary>
        ///     Manual export: Removes a factory exported under the specified types default name so that it is no longer used for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportType"> The type under whose default name the factory is exported. </param>
        /// <remarks>
        ///     <para>
        ///         Only the export matching the specified factory is removed.
        ///         If the same factory is also exported under other types or names, those exports remain intact.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void RemoveFactory (Func<CompositionContainer, object> factory, Type exportType) => this.RemoveFactory((Delegate)factory, exportType);

        /// <summary>
        ///     Manual export: Removes a factory exported under the specified name so that it is no longer used for composition.
        /// </summary>
        /// <param name="factory"> The factory which creates the exported instance. </param>
        /// <param name="exportName"> The name under which the factory is exported. </param>
        /// <remarks>
        ///     <para>
        ///         Only the export matching the specified factory and name is removed.
        ///         If the same factory is also exported under other types or names, those exports remain intact.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="factory" /> or <paramref name="exportName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void RemoveFactory (Func<CompositionContainer, object> factory, string exportName) => this.RemoveFactory((Delegate)factory, exportName);

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
        public void RemoveInstance (object instance, Type exportType)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.RemoveInstance(instance, CompositionContainer.GetNameOfType(exportType));
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
        public void RemoveInstance (object instance, string exportName)
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
        ///         Only the export matching the specified type is removed.
        ///         If the same type is also exported under other types or names, those exports remain intact.
        ///     </para>
        ///     <para>
        ///         This triggers an internal recomposition using <see cref="CompositionFlags" />.<see cref="CompositionFlags.Normal" />.
        ///         See <see cref="Recompose(CompositionFlags)" /> for details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="exportType" /> is null. </exception>
        /// <exception cref="CompositionException"> The internal recomposition failed. </exception>
        public void RemoveType (Type type, Type exportType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            this.RemoveType(type, CompositionContainer.GetNameOfType(exportType));
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
        public void RemoveType (Type type, string exportName)
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
                ResolveImports_PropertyInfo[] properties = CompositionContainer.ResolveImports_GetProperties(type);
                for (int i1 = 0; i1 < properties.Length; i1++)
                {
                    ResolveImports_PropertyInfo property = properties[i1];
                    List<ImportAttribute> attributes = property.ImportAttributes;

                    bool canCompose = attributes.Count > 0;
                    if (!canCompose)
                    {
                        continue;
                    }

                    bool canRecompose = property.CanRecompose;
                    object oldValue = property.GetMethod?.Invoke(obj, null);

                    if (isConstructing || (updateMissing && (oldValue == null)) || (updateRecomposable && canRecompose) || (updateComposed && (oldValue != null)))
                    {
                        string importName = property.ImportName;
                        Type importType = property.ImportType;

                        ImportKind importKind;
                        object newValue = this.GetImportValueFromNameOrType(importName, importType, out importKind);

                        bool updateValue = false;
                        if (importKind == ImportKind.Special)
                        {
                            Import oldImport = oldValue as Import;
                            Import newImport = newValue as Import;
                            List<object> oldValues = oldImport?.GetInstancesSnapshot() ?? new List<object>();
                            List<object> newValues = newImport?.GetInstancesSnapshot() ?? new List<object>();
                            updateValue = !CollectionComparer<object>.ReferenceEquality.Equals(oldValues, newValues);
                        }
                        else if (importKind == ImportKind.Enumerable)
                        {
                            List<object> oldValues = (oldValue as IEnumerable)?.ToList() ?? new List<object>();
                            List<object> newValues = (newValue as IEnumerable)?.ToList() ?? new List<object>();
                            updateValue = !CollectionComparer<object>.ReferenceEquality.Equals(oldValues, newValues);
                        }
                        else if ((importKind == ImportKind.LazyFunc) || (importKind == ImportKind.LazyObject))
                        {
                            updateValue = oldValue == null;
                        }
                        else if (importKind == ImportKind.Single)
                        {
                            updateValue = !object.ReferenceEquals(oldValue, newValue);
                        }

                        if (updateValue)
                        {
                            MethodInfo setMethod = property.SetMethod;
                            if (setMethod == null)
                            {
                                throw new CompositionException("Cannot set value for property because set accessor is missing/unavailable: " + type.FullName + "." + property.Property.Name);
                            }
                            else
                            {
                                this.Log(LogLevel.Debug, "Updating import ({0}): {1} <- {2}", composition, type.FullName + "." + property.Property.Name, newValue == null ? "[null]" : newValue.GetType().FullName);
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

        private void AddFactoryInternal (Delegate factory, string name, bool privateExport)
        {
            if (this.Factories.Any(x => (x.Factory == factory) && CompositionContainer.NameComparer.Equals(x.Name, name)))
            {
                return;
            }

            this.Factories.Add(new CompositionCatalogItem(name, factory, privateExport));
        }

        private void AddInstanceInternal (object instance, string name)
        {
            if (this.Instances.Any(x => object.ReferenceEquals(x.Value, instance) && CompositionContainer.NameComparer.Equals(x.Name, name)))
            {
                return;
            }

            this.Instances.Add(new CompositionCatalogItem(name, instance));
        }

        private void AddTypeInternal (Type type, string name, bool privateExport)
        {
            if (this.Types.Any(x => (x.Type == type) && CompositionContainer.NameComparer.Equals(x.Name, name)))
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
            this.Factories.Clear();
            this.Creators.Clear();
        }

        private object CreateArray (Type type, List<object> content)
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

        private Dictionary<string, List<CompositionCatalogItem>> GetCompositionSnapshot ()
        {
            lock (this.SyncRoot)
            {
                Dictionary<string, List<CompositionCatalogItem>> snapshot = new Dictionary<string, List<CompositionCatalogItem>>(CompositionContainer.NameComparer);
                foreach (KeyValuePair<string, CompositionItem> composition in this.Composition)
                {
                    string name = composition.Value.Name;

                    List<CompositionCatalogItem> items = new List<CompositionCatalogItem>();
                    snapshot.Add(name, items);

                    foreach (CompositionInstanceItem instance in composition.Value.Instances)
                    {
                        items.Add(new CompositionCatalogItem(name, instance.Instance));
                    }

                    foreach (CompositionTypeItem type in composition.Value.Types)
                    {
                        if (type.ClosedInstance != null)
                        {
                            items.Add(new CompositionCatalogItem(name, type.ClosedInstance));
                        }
                        else
                        {
                            items.Add(new CompositionCatalogItem(name, type.Type, type.PrivateExport));
                        }

                        foreach (KeyValuePair<string, object> openInstance in type.OpenInstances)
                        {
                            items.Add(new CompositionCatalogItem(name, openInstance.Value));
                        }
                    }

                    foreach (CompositionFactoryItem factory in composition.Value.Factories)
                    {
                        if (factory.Instance != null)
                        {
                            items.Add(new CompositionCatalogItem(name, factory.Instance));
                        }
                        else
                        {
                            items.Add(new CompositionCatalogItem(name, factory.Factory, factory.PrivateExport));
                        }
                    }
                }

                return snapshot;
            }
        }

        private List<object> GetExistingInstancesInternal (bool includeParentInstances)
        {
            List<object> instances = new List<object>(this.Composition.Count * 10);

            if (includeParentInstances && (this.ParentContainer != null))
            {
                lock (this.ParentContainer.SyncRoot)
                {
                    instances.AddRange(this.ParentContainer.GetExistingInstancesInternal(true));
                }
            }

            foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
            {
                for (int i1 = 0; i1 < compositionItem.Value.Instances.Count; i1++)
                {
                    CompositionInstanceItem instance = compositionItem.Value.Instances[i1];
                    if (instance.Instance != null)
                    {
                        instances.Add(instance.Instance);
                    }
                }

                for (int i1 = 0; i1 < compositionItem.Value.Types.Count; i1++)
                {
                    CompositionTypeItem type = compositionItem.Value.Types[i1];
                    if (type.ClosedInstance != null)
                    {
                        instances.Add(type.ClosedInstance);
                    }

                    instances.AddRange(type.OpenInstances.Values);
                }

                for (int i1 = 0; i1 < compositionItem.Value.Factories.Count; i1++)
                {
                    CompositionFactoryItem factory = compositionItem.Value.Factories[i1];
                    if (factory.Instance != null)
                    {
                        instances.Add(factory.Instance);
                    }
                }
            }

            return instances;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private T GetExportForLazyInvoker <T> (string exportName)
            where T : class
        {
            lock (this.SyncRoot)
            {
                return (T)this.GetImportValueFromNameOrType(exportName, typeof(T), out _);
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private List<T> GetExportsForLazyInvoker <T> (string exportName)
            where T : class
        {
            lock (this.SyncRoot)
            {
                return new List<T>((IEnumerable<T>)this.GetImportValueFromNameOrType(exportName, typeof(IEnumerable<T>), out _));
            }
        }

        private Type GetImportTypeFromType (Type type, out ImportKind kind)
        {
            if (type == typeof(Import))
            {
                kind = ImportKind.Special;
                return typeof(object);
            }

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

            kind = ImportKind.Single;
            return type;
        }

        private object GetImportValueFromNameOrType (string name, Type type, out ImportKind kind)
        {
            if (type == null)
            {
                throw new CompositionException("No import type specified.");
            }

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
                importValues = this.GetOrCreateInstancesInternal(importName, importType, this);
            }

            if (kind == ImportKind.Special)
            {
                return new Import(importValues?.Count == 0 ? null : importValues?.ToArray());
            }

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

            if (kind == ImportKind.Single)
            {
                return importValues?.Count > 0 ? importValues[0] : null;
            }

            return null;
        }

        private List<object> GetOrCreateInstancesInternal (string nameHint, Type typeHint, CompositionContainer resolveScope)
        {
            string name = nameHint ?? CompositionContainer.GetNameOfType(typeHint);

            HashSet<object> instances = new HashSet<object>();
            HashSet<CompositionTypeItem> types = new HashSet<CompositionTypeItem>();
            HashSet<CompositionFactoryItem> factories = new HashSet<CompositionFactoryItem>();

            if (this.ParentContainer != null)
            {
                lock (this.ParentContainer.SyncRoot)
                {
                    List<object> parentInstances = this.ParentContainer.GetOrCreateInstancesInternal(nameHint, typeHint, resolveScope);
                    instances.AddRange(parentInstances);
                }
            }

            if (this.Composition.ContainsKey(name))
            {
                CompositionItem item = this.Composition[name];

                for (int i1 = 0; i1 < item.Instances.Count; i1++)
                {
                    CompositionInstanceItem instanceItem = item.Instances[i1];
                    if (instanceItem.Instance != null)
                    {
                        instances.Add(instanceItem.Instance);
                    }
                }

                for (int i1 = 0; i1 < item.Types.Count; i1++)
                {
                    CompositionTypeItem typeItem = item.Types[i1];
                    if (typeItem.Type.IsGenericTypeDefinition)
                    {
                        if (typeHint?.IsGenericTypeDefinition ?? false)
                        {
                            instances.AddRange(typeItem.OpenInstances.Values);
                        }
                        else
                        {
                            string openName = typeHint?.FullName ?? string.Empty;
                            if (typeItem.OpenInstances.ContainsKey(openName))
                            {
                                instances.Add(typeItem.OpenInstances[openName]);
                            }
                            else
                            {
                                types.Add(typeItem);
                            }
                        }
                    }
                    else
                    {
                        if (typeItem.ClosedInstance != null)
                        {
                            instances.Add(typeItem.ClosedInstance);
                        }
                        else
                        {
                            types.Add(typeItem);
                        }
                    }
                }

                for (int i1 = 0; i1 < item.Factories.Count; i1++)
                {
                    CompositionFactoryItem factoryItem = item.Factories[i1];
                    if (factoryItem.Instance != null)
                    {
                        instances.Add(factoryItem.Instance);
                    }
                    else
                    {
                        factories.Add(factoryItem);
                    }
                }
            }

            Type genericType = typeHint == null ? null : ((typeHint.IsGenericType && (!typeHint.IsGenericTypeDefinition)) ? typeHint.GetGenericTypeDefinition() : null);
            if (genericType != null)
            {
                string genericName = CompositionContainer.GetNameOfType(genericType);
                if (this.Composition.ContainsKey(genericName))
                {
                    CompositionItem item = this.Composition[genericName];

                    for (int i1 = 0; i1 < item.Types.Count; i1++)
                    {
                        CompositionTypeItem typeItem = item.Types[i1];
                        if (typeItem.Type.IsGenericTypeDefinition)
                        {
                            string openName = typeHint.FullName ?? string.Empty;
                            if (typeItem.OpenInstances.ContainsKey(openName))
                            {
                                instances.Add(typeItem.OpenInstances[openName]);
                            }
                            else
                            {
                                types.Add(typeItem);
                            }
                        }
                        else
                        {
                            if (typeItem.ClosedInstance != null)
                            {
                                instances.Add(typeItem.ClosedInstance);
                            }
                            else
                            {
                                types.Add(typeItem);
                            }
                        }
                    }
                }
            }

            List<object> newInstances = new List<object>(types.Count + factories.Count);

            foreach (CompositionTypeItem typeItem in types)
            {
                Type typeToCreate = typeItem.Type;
                bool isOpenType = typeToCreate.IsGenericTypeDefinition;
                if (isOpenType)
                {
                    Type[] typeArguments = typeHint.GetGenericArguments();
                    typeToCreate = typeToCreate.MakeGenericType(typeArguments);
                }

                string openName = isOpenType ? typeToCreate.FullName : null;
                if ((openName != null) && (typeItem.OpenInstances.ContainsKey(openName)))
                {
                    instances.Add(typeItem.OpenInstances[openName]);
                    continue;
                }

                bool supportedByCreators = this.Creators.Any(x => x.CanCreateInstance(this, name, typeToCreate));

                object newInstance = null;
                {
                    Type returnType = typeHint ?? typeof(object);

                    MethodInfo[] allMethods = typeToCreate.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                    List<MethodInfo> methods = allMethods.Where(x => (x.GetCustomAttributes(typeof(ExportCreatorAttribute), true).Length > 0) && (x.ReturnType != typeof(void)) && (returnType.IsAssignableFrom(x.ReturnType)));

                    if (methods.Count > 1)
                    {
                        throw new CompositionException("Too many export creators defined for type: " + typeToCreate.FullName);
                    }

                    if (methods.Count > 0)
                    {
                        MethodInfo method = methods[0];
                        ParameterInfo[] methodParameters = method.GetParameters();
                        object[] parameters = new object[methodParameters.Length];

                        for (int i1 = 0; i1 < methodParameters.Length; i1++)
                        {
                            string importName = methodParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
                            Type importType = methodParameters[i1].ParameterType;

                            if (importType == typeof(string))
                            {
                                parameters[i1] = name;
                            }
                            else if (importType == typeof(Type))
                            {
                                parameters[i1] = typeToCreate;
                            }
                            else
                            {
                                parameters[i1] = resolveScope.GetImportValueFromNameOrType(importName, importType, out _);
                            }
                        }

                        newInstance = method.Invoke(null, parameters);
                    }
                }

                if (newInstance == null)
                {
                    ConstructorInfo[] allConstructors = typeToCreate.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    List<ConstructorInfo> declaredConstructors = allConstructors.Where(x => x.GetCustomAttributes(typeof(ExportConstructorAttribute), false).Length > 0);

                    if (declaredConstructors.Count > 1)
                    {
                        throw new CompositionException("Too many export constructors defined for type: " + typeToCreate.FullName);
                    }

                    List<ConstructorInfo> greedyConstructors = supportedByCreators ? new List<ConstructorInfo>() : new List<ConstructorInfo>(allConstructors);
                    greedyConstructors.Sort((x, y) => x.GetParameters().Length.CompareTo(y.GetParameters().Length));
                    greedyConstructors.Reverse();

                    List<ConstructorInfo> constructorCandidates = new List<ConstructorInfo>();
                    constructorCandidates.AddRange(greedyConstructors);

                    List<ConstructorInfo> fullyResolvable = new List<ConstructorInfo>();
                    List<ConstructorInfo> partiallyResolvable = new List<ConstructorInfo>();
                    foreach (ConstructorInfo constructor in constructorCandidates)
                    {
                        bool? full = null;
                        ParameterInfo[] constructorParameters = constructor.GetParameters();
                        if (constructorParameters.Length == 0)
                        {
                            full = true;
                        }
                        else
                        {
                            for (int i1 = 0; i1 < constructorParameters.Length; i1++)
                            {
                                string importName = constructorParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
                                Type importType = constructorParameters[i1].ParameterType;

                                if (!CompositionContainer.ValidateImportType(importType))
                                {
                                    full = null;
                                    break;
                                }

                                full = resolveScope.GetImportValueFromNameOrType(importName, importType, out _) != null;
                            }
                        }

                        if (full.HasValue)
                        {
                            if (full.Value)
                            {
                                fullyResolvable.Add(constructor);
                            }
                            else
                            {
                                partiallyResolvable.Add(constructor);
                            }
                        }
                    }

                    List<ConstructorInfo> constructors = new List<ConstructorInfo>();
                    constructors.AddRange(declaredConstructors);
                    constructors.AddRange(fullyResolvable);
                    constructors.AddRange(partiallyResolvable);

                    if (constructors.Count > 0)
                    {
                        ConstructorInfo constructor = constructors[0];
                        ParameterInfo[] constructorParameters = constructor.GetParameters();
                        object[] parameters = new object[constructorParameters.Length];

                        for (int i1 = 0; i1 < constructorParameters.Length; i1++)
                        {
                            string importName = constructorParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
                            Type importType = constructorParameters[i1].ParameterType;

                            if (importType == typeof(string))
                            {
                                parameters[i1] = name;
                            }
                            else if (importType == typeof(Type))
                            {
                                parameters[i1] = typeToCreate;
                            }
                            else
                            {
                                parameters[i1] = resolveScope.GetImportValueFromNameOrType(importName, importType, out _);
                            }
                        }

                        newInstance = constructor.Invoke(parameters);
                    }
                }

                if ((newInstance == null) && supportedByCreators)
                {
                    foreach (CompositionCreator creator in this.Creators)
                    {
                        if (creator.CanCreateInstance(this, name, typeToCreate))
                        {
                            newInstance = creator.CreateInstance(this, name, typeToCreate);
                            if (newInstance != null)
                            {
                                break;
                            }
                        }
                    }
                }

                if (newInstance == null)
                {
                    this.Log(LogLevel.Debug, "The type is not supported by export creators, export constructors, or composition creators: {0}", typeItem.Type.FullName);
                }

                if (newInstance != null)
                {
                    newInstances.Add(newInstance);

                    if (isOpenType)
                    {
                        if (!typeItem.PrivateExport)
                        {
                            typeItem.OpenInstances.Add(openName, newInstance);
                        }

                        foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
                        {
                            foreach (CompositionTypeItem other in compositionItem.Value.Types)
                            {
                                if ((other.Type == typeItem.Type) && (!other.OpenInstances.ContainsKey(openName)) && (!other.PrivateExport))
                                {
                                    other.OpenInstances.Add(openName, newInstance);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!typeItem.PrivateExport)
                        {
                            typeItem.ClosedInstance = newInstance;
                        }

                        foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
                        {
                            foreach (CompositionTypeItem other in compositionItem.Value.Types)
                            {
                                if ((other.Type == typeItem.Type) && (other.ClosedInstance == null) && (!other.PrivateExport))
                                {
                                    other.ClosedInstance = newInstance;
                                }
                            }
                        }
                    }
                }
            }

            foreach (CompositionFactoryItem factoryItem in factories)
            {
                Type typeToCreate = typeHint ?? typeof(object);

                object newInstance = null;

                if (typeToCreate.IsAssignableFrom(factoryItem.Factory.Method.ReturnType) || (factoryItem.Factory.Method.ReturnType == typeof(object)))
                {
                    ParameterInfo[] factoryParameters = factoryItem.Factory.Method.GetParameters();
                    object[] parameters = new object[factoryParameters.Length];

                    for (int i1 = 0; i1 < factoryParameters.Length; i1++)
                    {
                        string importName = factoryParameters[i1].GetCustomAttributes(typeof(ImportAttribute), false).OfType<ImportAttribute>().FirstOrDefault(null, x => !x.Name.IsNullOrEmptyOrWhitespace())?.Name;
                        Type importType = factoryParameters[i1].ParameterType;

                        if (importType == typeof(string))
                        {
                            parameters[i1] = name;
                        }
                        else if (importType == typeof(Type))
                        {
                            parameters[i1] = typeToCreate;
                        }
                        else
                        {
                            parameters[i1] = resolveScope.GetImportValueFromNameOrType(importName, importType, out _);
                        }
                    }

                    newInstance = factoryItem.Factory.DynamicInvoke(parameters);
                }

                if (newInstance == null)
                {
                    this.Log(LogLevel.Debug, "The factory is not a supported delegate type: {0}", factoryItem.Factory.GetFullName());
                }

                if (newInstance != null)
                {
                    newInstances.Add(newInstance);

                    if (!factoryItem.PrivateExport)
                    {
                        factoryItem.Instance = newInstance;
                    }

                    foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
                    {
                        foreach (CompositionFactoryItem other in compositionItem.Value.Factories)
                        {
                            if ((other.Factory == factoryItem.Factory) && (other.Instance == null) && (!other.PrivateExport))
                            {
                                other.Instance = newInstance;
                            }
                        }
                    }
                }
            }

            for (int i1 = 0; i1 < newInstances.Count; i1++)
            {
                object newInstance = newInstances[i1];
                resolveScope.ResolveImports(newInstance, CompositionFlags.Constructing);
            }

            for (int i1 = 0; i1 < newInstances.Count; i1++)
            {
                object newInstance = newInstances[i1];

                HashSet<string> names = this.GetNamesForInstance(newInstance);
                names.Add(name);

                this.Log(LogLevel.Debug, "Type added to container: {0} / {1}", names.Join(", "), newInstance.GetType().AssemblyQualifiedName);

                IExporting exportingInstance = newInstance as IExporting;
                if (exportingInstance != null)
                {
                    names.ForEach(x => exportingInstance.AddedToContainer(x, this));
                }
            }

            instances.AddRange(newInstances);

            return new List<object>(instances);
        }

        private HashSet<string> GetNamesForInstance (object instance)
        {
            HashSet<string> names = new HashSet<string>(CompositionContainer.NameComparer);

            foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
            {
                string name = compositionItem.Key;
                CompositionItem item = compositionItem.Value;

                if (item.Factories.Any(x => object.ReferenceEquals(x.Instance, instance)) || item.Types.Any(x => object.ReferenceEquals(x.ClosedInstance, instance) || x.OpenInstances.Any(y => object.ReferenceEquals(y.Value, instance))) || item.Instances.Any(x => object.ReferenceEquals(x.Instance, instance)))
                {
                    names.Add(name);
                }
            }

            return names;
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

            this.Catalogs.RemoveAll(catalog);
        }

        private void RemoveCreatorInternal (CompositionCreator creator)
        {
            if (!this.Creators.Contains(creator))
            {
                return;
            }

            this.Creators.RemoveAll(creator);
        }

        private void RemoveFactoryInternal (Delegate factory, string name)
        {
            this.Factories.RemoveAll(x => (x.Factory == factory) && CompositionContainer.NameComparer.Equals(x.Name, name));
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
            items.AddRange(this.Factories);

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
                        CompositionInstanceItem instanceItem = compositionItem.Instances.FirstOrDefault(x => object.ReferenceEquals(x.Instance, item.Value));
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
                        CompositionTypeItem typeItem = compositionItem.Types.FirstOrDefault(x => (x.Type == item.Type));
                        if (typeItem == null)
                        {
                            typeItem = new CompositionTypeItem(item.Type, item.PrivateExport);
                            compositionItem.Types.Add(typeItem);
                        }

                        typeItem.Checked = true;
                    }
                }
                else if (item.Factory != null)
                {
                    if (CompositionContainer.ValidateExportFactory(item.Factory))
                    {
                        CompositionFactoryItem factoryItem = compositionItem.Factories.FirstOrDefault(x => (x.Factory == item.Factory));
                        if (factoryItem == null)
                        {
                            factoryItem = new CompositionFactoryItem(item.Factory, item.PrivateExport);
                            compositionItem.Factories.Add(factoryItem);
                        }

                        factoryItem.Checked = true;
                    }
                }
            }

            foreach (KeyValuePair<string, CompositionItem> compositionItem in this.Composition)
            {
                compositionItem.Value.Instances.RemoveWhere(x => !x.Checked).ForEach(x =>
                {
                    this.Log(LogLevel.Debug, "Instance removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.AssemblyQualifiedName ?? "[null]");
                    (x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
                    if (this.AutoDispose && (!object.ReferenceEquals(x?.Instance, this)))
                    {
                        (x?.Instance as IDisposable)?.Dispose();
                    }
                });
                compositionItem.Value.Types.RemoveWhere(x => !x.Checked).ForEach(x =>
                {
                    this.Log(LogLevel.Debug, "Type removed from container (closed): {0} / {1}", compositionItem.Key, x?.ClosedInstance?.GetType()?.AssemblyQualifiedName ?? "[null]");
                    (x?.ClosedInstance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
                    if (this.AutoDispose && (!object.ReferenceEquals(x?.ClosedInstance, this)))
                    {
                        (x?.ClosedInstance as IDisposable)?.Dispose();
                    }

                    foreach (KeyValuePair<string, object> openInstance in x?.OpenInstances ?? new Dictionary<string, object>())
                    {
                        this.Log(LogLevel.Debug, "Type removed from container (open): {0} / {1}", compositionItem.Key, openInstance.Value?.GetType()?.AssemblyQualifiedName ?? "[null]");
                        (openInstance.Value as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
                        if (this.AutoDispose && (!object.ReferenceEquals(openInstance.Value, this)))
                        {
                            (openInstance.Value as IDisposable)?.Dispose();
                        }
                    }
                });
                compositionItem.Value.Factories.RemoveWhere(x => !x.Checked).ForEach(x =>
                {
                    this.Log(LogLevel.Debug, "Factory removed from container: {0} / {1}", compositionItem.Key, x?.Instance?.GetType()?.AssemblyQualifiedName ?? "[null]");
                    (x?.Instance as IExporting)?.RemovedFromContainer(compositionItem.Key, this);
                    if (this.AutoDispose && (!object.ReferenceEquals(x?.Instance, this)))
                    {
                        (x?.Instance as IDisposable)?.Dispose();
                    }
                });
                compositionItem.Value.ResetChecked();
            }

            this.Composition.RemoveWhere(x => (x.Value.Instances.Count == 0) && (x.Value.Types.Count == 0) && (x.Value.Factories.Count == 0)).ForEach(x => this.Log(LogLevel.Debug, "Removing export: {0}", x.Key));

            foreach (KeyValuePair<object, HashSet<string>> newInstance in newInstances)
            {
                foreach (string name in newInstance.Value)
                {
                    this.Log(LogLevel.Debug, "Instance added to container: {0} / {1}", name, newInstance.Key.GetType().AssemblyQualifiedName);

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




        #region Interface: IDependencyResolver

        /// <inheritdoc />
        object IDependencyResolver.GetInstance (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return this.GetExport(type);
        }

        /// <inheritdoc />
        object IDependencyResolver.GetInstance (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            return this.GetExport(name);
        }

        /// <inheritdoc />
        T IDependencyResolver.GetInstance <T> ()
        {
            return this.GetExport<T>();
        }

        /// <inheritdoc />
        List<object> IDependencyResolver.GetInstances (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return this.GetExports(type);
        }

        /// <inheritdoc />
        List<object> IDependencyResolver.GetInstances (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            return this.GetExports(name);
        }

        /// <inheritdoc />
        List<T> IDependencyResolver.GetInstances <T> ()
        {
            return this.GetExports<T>();
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        public void Dispose ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsClearing)
                {
                    return;
                }

                try
                {
                    this.IsClearing = true;

                    this.Log(LogLevel.Debug, "Disposing container.");

                    if (this.ParentContainer != null)
                    {
                        this.ParentContainer.CompositionChanged -= this.ParentContainerCompositionChangedHandler;
                        this.ParentContainer = null;
                    }

                    this.LazyInvokers.Clear();

                    this.ClearInternal();
                    this.UpdateComposition(true);
                }
                finally
                {
                    this.IsClearing = false;
                }
            }

            this.RaiseCompositionChanged();
        }

        #endregion




        #region Interface: IServiceProvider

        /// <inheritdoc />
        object IServiceProvider.GetService (Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if ((!serviceType.IsClass) && (!serviceType.IsInterface))
            {
                throw new InvalidTypeArgumentException(nameof(serviceType));
            }

            return this.GetExport(serviceType);
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion




        #region Type: CompositionFactoryItem

        private sealed class CompositionFactoryItem
        {
            #region Instance Constructor/Destructor

            public CompositionFactoryItem (Delegate factory, bool privateExport)
            {
                this.Factory = factory;
                this.PrivateExport = privateExport;

                this.Checked = false;
                this.Instance = null;
            }

            #endregion




            #region Instance Properties/Indexer

            public bool Checked { get; set; }

            public Delegate Factory { get; }

            public object Instance { get; set; }

            public bool PrivateExport { get; }

            #endregion
        }

        #endregion




        #region Type: CompositionInstanceItem

        private sealed class CompositionInstanceItem
        {
            #region Instance Constructor/Destructor

            public CompositionInstanceItem (object instance)
            {
                this.Instance = instance;

                this.Checked = false;
            }

            #endregion




            #region Instance Properties/Indexer

            public bool Checked { get; set; }

            public object Instance { get; }

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
                this.Factories = new List<CompositionFactoryItem>();
            }

            #endregion




            #region Instance Properties/Indexer

            public List<CompositionFactoryItem> Factories { get; }

            public List<CompositionInstanceItem> Instances { get; }

            public string Name { get; }

            public List<CompositionTypeItem> Types { get; }

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

                foreach (CompositionFactoryItem factoryItem in this.Factories)
                {
                    factoryItem.Checked = false;
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
                this.ClosedInstance = null;
                this.OpenInstances = new Dictionary<string, object>(StringComparerEx.Ordinal);
            }

            #endregion




            #region Instance Properties/Indexer

            public bool Checked { get; set; }

            public object ClosedInstance { get; set; }

            public Dictionary<string, object> OpenInstances { get; }

            public bool PrivateExport { get; }

            public Type Type { get; }

            #endregion
        }

        #endregion




        #region Type: ImportKind

        private enum ImportKind
        {
            Special,
            Enumerable,
            LazyFunc,
            LazyObject,
            Single,
        }

        #endregion




        #region Type: LazyInvoker

        private sealed class LazyInvoker
        {
            #region Instance Constructor/Destructor

            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            public LazyInvoker (CompositionContainer container, string name, Type type)
            {
                this.Container = container;
                this.Name = name;
                this.Type = type;
                this.Resolver = null;

                if (RuntimeEnvironment.IsUnityRuntime())
                {
                    throw new PlatformNotSupportedException("Lazy imports not supported in Unity.");
                }

                Type enumerableType = CompositionContainer.GetEnumerableType(type);
                string resolveName = enumerableType == null ? nameof(CompositionContainer.GetExportForLazyInvoker) : nameof(CompositionContainer.GetExportsForLazyInvoker);
                MethodInfo genericMethod = container.GetType().GetMethod(resolveName, BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo resolveMethod = genericMethod.MakeGenericMethod(type);

                MethodCallExpression resolveCall = Expression.Call(Expression.Constant(this.Container), resolveMethod, Expression.Constant(this.Name));
                this.Resolver = Expression.Lambda(resolveCall).Compile();
            }

            #endregion




            #region Instance Properties/Indexer

            public Delegate Resolver { get; }

            private CompositionContainer Container { get; }

            private string Name { get; }

            private Type Type { get; }

            #endregion
        }

        #endregion




        #region Type: ResolveImports_PropertyInfo

        private sealed class ResolveImports_PropertyInfo
        {
            #region Instance Properties/Indexer

            public bool CanRecompose { get; set; }

            public MethodInfo GetMethod { get; set; }

            public List<ImportAttribute> ImportAttributes { get; } = new List<ImportAttribute>();

            public string ImportName { get; set; }

            public Type ImportType { get; set; }

            public PropertyInfo Property { get; set; }

            public MethodInfo SetMethod { get; set; }

            #endregion
        }

        #endregion
    }
}
