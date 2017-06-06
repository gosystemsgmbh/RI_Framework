using System;
using System.Collections.Generic;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;

using RI.Framework.Composition;




namespace RI.Framework
{
	/// <summary>
	///     Nancy bootstrapper which uses a composition container.
	/// </summary>
	public class CompositionContainerNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<CompositionContainer>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerNancyBootstrapper" />.
		/// </summary>
		/// <param name="compositionContainer"> The composition container to use as the root application container. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="compositionContainer" /> is null. </exception>
		public CompositionContainerNancyBootstrapper (CompositionContainer compositionContainer)
		{
			if (compositionContainer == null)
			{
				throw new ArgumentNullException(nameof(compositionContainer));
			}

			this.CompositionContainer = compositionContainer;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		protected CompositionContainer CompositionContainer { get; private set; }

		#endregion




		#region Virtuals

		/// <summary>
		///     Used to override Nancy-internal configuration.
		/// </summary>
		/// <param name="nancyInternalConfiguration"> The Nancy-internal configuration to override. </param>
		protected virtual void OnConfigurationBuilder (NancyInternalConfiguration nancyInternalConfiguration)
		{
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override NancyInternalConfiguration InternalConfiguration => NancyInternalConfiguration.WithOverrides(this.OnConfigurationBuilder);

		/// <inheritdoc />
		protected override CompositionContainer CreateRequestContainer (NancyContext context)
		{
			return new CompositionContainer(this.CompositionContainer);
		}

		/// <inheritdoc />
		protected override IEnumerable<INancyModule> GetAllModules (CompositionContainer container)
		{
			return container.GetExports<INancyModule>();
		}

		/// <inheritdoc />
		protected override CompositionContainer GetApplicationContainer ()
		{
			return this.CompositionContainer;
		}

		/// <inheritdoc />
		protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks ()
		{
			return this.ApplicationContainer.GetExports<IApplicationStartup>();
		}

		/// <inheritdoc />
		protected override IDiagnostics GetDiagnostics ()
		{
			return this.ApplicationContainer.GetExport<IDiagnostics>();
		}

		/// <inheritdoc />
		protected override INancyEngine GetEngineInternal ()
		{
			return this.ApplicationContainer.GetExport<INancyEngine>();
		}

		/// <inheritdoc />
		protected override INancyModule GetModule (CompositionContainer container, Type moduleType)
		{
			return container.GetExport<INancyModule>(moduleType);
		}

		/// <inheritdoc />
		protected override IEnumerable<IRegistrations> GetRegistrationTasks ()
		{
			return this.ApplicationContainer.GetExports<IRegistrations>();
		}

		/// <inheritdoc />
		protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks (CompositionContainer container, Type[] requestStartupTypes)
		{
			foreach (Type requestStartupType in requestStartupTypes)
			{
				container.AddExport(requestStartupType, typeof(IRequestStartup));
			}

			return container.GetExports<IRequestStartup>();
		}

		/// <inheritdoc />
		protected override void RegisterBootstrapperTypes (CompositionContainer applicationContainer)
		{
			applicationContainer.AddExport(this, typeof(INancyModuleCatalog));
		}

		/// <inheritdoc />
		protected override void RegisterCollectionTypes (CompositionContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
		{
			foreach (CollectionTypeRegistration collectionTypeRegistration in collectionTypeRegistrationsn)
			{
				foreach (Type typeRegistration in collectionTypeRegistration.ImplementationTypes)
				{
					switch (collectionTypeRegistration.Lifetime)
					{
						case Lifetime.Singleton:
							container.AddExport(typeRegistration, collectionTypeRegistration.RegistrationType, false);
							break;
						case Lifetime.Transient:
							container.AddExport(typeRegistration, collectionTypeRegistration.RegistrationType, true);
							break;
						case Lifetime.PerRequest: throw new InvalidOperationException("Type registration on a per-request basis are not supported.");
						default: throw new InvalidOperationException("Unknown type registration lifetime.");
					}
				}
			}
		}

		/// <inheritdoc />
		protected override void RegisterInstances (CompositionContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
		{
			foreach (InstanceRegistration instanceRegistration in instanceRegistrations)
			{
				container.AddExport(instanceRegistration.Implementation, instanceRegistration.RegistrationType);
			}
		}

		/// <inheritdoc />
		protected override void RegisterRequestContainerModules (CompositionContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
		{
		}

		/// <inheritdoc />
		protected override void RegisterTypes (CompositionContainer container, IEnumerable<TypeRegistration> typeRegistrations)
		{
			foreach (TypeRegistration typeRegistration in typeRegistrations)
			{
				switch (typeRegistration.Lifetime)
				{
					case Lifetime.Singleton:
						container.AddExport(typeRegistration.ImplementationType, typeRegistration.RegistrationType, false);
						break;
					case Lifetime.Transient:
						container.AddExport(typeRegistration.ImplementationType, typeRegistration.RegistrationType, true);
						break;
					case Lifetime.PerRequest: throw new InvalidOperationException("Type registration on a per-request basis are not supported.");
					default: throw new InvalidOperationException("Unknown type registration lifetime.");
				}
			}
		}

		#endregion
	}
}
