# Decoupling & Utilities Framework

## 2.0.0.0

- [ ] Cleanup
  - [ ] TODOs
  - [ ] Code cleanup
  - [ ] Packages
- [ ] Testing (common)
  - [ ] RuntimeEnvironment
  - [ ] CultureInfoExtensions
  - [ ] ObjectExtensions
  - [ ] AlphanumComparer
  - [ ] PredicateLogFilter
  - [ ] SeverityLogFilter
  - [ ] Filtered log service
  - [ ] Filtered log writer
  - [ ] FileLogWriter
  - [ ] DirectoryLogWriter (including day separation)
  - [ ] LogFileReader
  - [ ] TemporaryFile
  - [ ] UncloseableStream
  - [ ] SynchronizedStream
  - [ ] CSV
  - [ ] AggregateCatalog
  - [ ] FileCatalog
  - [ ] DirectoryCatalog
  - [ ] ContainerCatalog
  - [ ] AppDomainCatalog
  - [ ] InstanceCatalog
  - [ ] TypeCatalog
  - [ ] Singleton (generic and non-generic)
  - [ ] ServiceLocator
  - [ ] ServiceLocatorStateResolver
  - [ ] SingletonStateResolver
  - [ ] CompositionContainerStateResolver
  - [ ] DependencyResolverStateResolver
  - [ ] DefaultStateResolver
  - [ ] DefaultStateCache
  - [ ] DefaultStateDispatcher
  - [ ] StateMachine (including TransitionAborted and Update)
  - [ ] FilePath (including new ops)
  - [ ] DirectoryPath (including new ops)
  - [ ] CompositionContainer with parent container
  - [ ] CompositionContainer without export attributes
  - [ ] CompositionCreator
  - [ ] CompositionExtensions
  - [ ] HeavyThread
- [ ] Testing (NetFX)
  - [ ] TaskExtensions
  - [ ] ThreadPoolAwaiter
  - [ ] CultureFlower
  - [ ] ThreadDispatcherAwaiter
  - [ ] ThreadDispatcherSynchronizationContext
  - [ ] ThreadDispatcherTaskScheduler
  - [ ] ThreadDispatcherTimer
  - [ ] ThreadDispatcherExecutionContext
  - [ ] IThreadDispatcherExtensions
  - [ ] ThreadDispatcherOperation
  - [ ] ThreadDispatcher
  - [ ] HeavyThreadDispatcher
  - [ ] AsyncStateMachine
  - [ ] ThreadDispatcherStateDispatcher
  - [ ] SynchronizationContextExtensions
  - [ ] SynchronizationContextFlower
  - [ ] SynchronizationContextAwaiter
- [ ] Testing (Unity)
  - [ ] GameObjectCatalog
  - [ ] ScriptingCatalog (including without export attributes)
  - [ ] MonoBehaviourCreator
  - [ ] DispatcherServiceStateDispatcher
  - [ ] Run tests with oldest and newest Unity version (update compatibility page)
- [ ] Documentation
  - [ ] NetFx: Simple documentation
  - [ ] Unity: Update bootstrapper screenshot and options description
  - [ ] Unity: Uncomments after implementation
  - [ ] Unity: Performance
  - [ ] Unity: Thread Safety
- [ ] Advertisement


## Backlog

- [ ] Wishlist
- [ ] Provide all possible overloads for timeout and cancellation token

## Proposals

- [ ] Unify cross-platform model and code
- [ ] AOT checking utility
- [ ] Migrate Common to Shared (own DLL using .NET Standard?)
