using System;
using System.Collections.Generic;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;

using RI.Framework.Composition;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Nancy bootstrapper which uses a <see cref="CompositionContainer" /> and a separate <see cref="CompositionContainer"/> for each request.
	/// </summary>
	public class CompositionContainerNancyBootstrapperWithRequestContainer : NancyBootstrapperWithRequestContainerBase<CompositionContainer>, ILogSource
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerNancyBootstrapperWithRequestContainer" />.
		/// </summary>
		/// <param name="compositionContainer"> The composition container to use as the root application container. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="compositionContainer" /> is null. </exception>
		public CompositionContainerNancyBootstrapperWithRequestContainer(CompositionContainer compositionContainer)
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
			CompositionBatch batch = new CompositionBatch();
			foreach (Type requestStartupType in requestStartupTypes)
			{
				batch.AddExport(requestStartupType, typeof(IRequestStartup));
			}
			container.Compose(batch);

			return container.GetExports<IRequestStartup>();
		}

		/// <inheritdoc />
		protected override void RegisterBootstrapperTypes (CompositionContainer applicationContainer)
		{
			CompositionBatch batch = new CompositionBatch();
			batch.AddExport(this, typeof(INancyBootstrapper));
			batch.AddExport(this, typeof(INancyModuleCatalog));
			batch.AddExport(this, this.GetType());
			applicationContainer.Compose(batch);
		}

		/// <inheritdoc />
		protected override void RegisterCollectionTypes (CompositionContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
		{
			CompositionBatch batch = new CompositionBatch();
			foreach (CollectionTypeRegistration collectionTypeRegistration in collectionTypeRegistrationsn)
			{
				foreach (Type typeRegistration in collectionTypeRegistration.ImplementationTypes)
				{
					switch (collectionTypeRegistration.Lifetime)
					{
						case Lifetime.Singleton:
							batch.AddExport(typeRegistration, collectionTypeRegistration.RegistrationType, false);
							break;
						case Lifetime.Transient:
							batch.AddExport(typeRegistration, collectionTypeRegistration.RegistrationType, true);
							break;
						case Lifetime.PerRequest: throw new InvalidOperationException("Type registration on a per-request basis are not supported.");
						default: throw new InvalidOperationException("Unknown type registration lifetime.");
					}
				}
			}
			container.Compose(batch);
		}

		/// <inheritdoc />
		protected override void RegisterInstances (CompositionContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
		{
			CompositionBatch batch = new CompositionBatch();
			foreach (InstanceRegistration instanceRegistration in instanceRegistrations)
			{
				batch.AddExport(instanceRegistration.Implementation, instanceRegistration.RegistrationType);
			}
			container.Compose(batch);
		}

		/// <inheritdoc />
		protected override void RegisterRequestContainerModules (CompositionContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
		{
		}

		/// <inheritdoc />
		protected override void RegisterTypes (CompositionContainer container, IEnumerable<TypeRegistration> typeRegistrations)
		{
			CompositionBatch batch = new CompositionBatch();
			foreach (TypeRegistration typeRegistration in typeRegistrations)
			{
				switch (typeRegistration.Lifetime)
				{
					case Lifetime.Singleton:
						batch.AddExport(typeRegistration.ImplementationType, typeRegistration.RegistrationType, false);
						break;
					case Lifetime.Transient:
						batch.AddExport(typeRegistration.ImplementationType, typeRegistration.RegistrationType, true);
						break;
					case Lifetime.PerRequest: throw new InvalidOperationException("Type registration on a per-request basis are not supported.");
					default: throw new InvalidOperationException("Unknown type registration lifetime.");
				}
			}
			container.Compose(batch);
		}

		#endregion




		#region Interface: ILogSource

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;


		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		#endregion
	}
}
