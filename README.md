# Decoupling & Utilities Framework

## 1.1.0.0

- [ ] Packages
- [ ] Cleanup
  - [ ] Warnings
  - [ ] TODOs
  - [ ] NuGet updates
  - [ ] Code cleanup
  - [ ] Check usage of IModuleService and IDispatcherService and ILogService in documentation
- [ ] Reduce dependencies (Locators, DispatcherService, IThreadDispatcher, Logging) and check threading
  - [ ] CompositionContainer
    - [ ] Make bootstrapper container-agnostic
    - [ ] ResolveImports event for InstanceLocator
    - [ ] ResolveExport event for InstanceLocator
    - [ ] CompositionContainer property for InstanceLocator
    - [ ] Locking
  - [ ] IRegionService
    - [ ] Global property in RegionBinder
  - [ ] Logging
  - [ ] Modularization
- [ ] Documentation
  - [ ] Namespace descriptions
  - [ ] NetFx: Simple documentation
  - [ ] Unity: Uncomments after implementation
  - [ ] Unity: AOT/IL2CPP
  - [ ] Unity: Performance
  - [ ] Unity: Thread Safety
- [ ] Testing
  - [ ] ThreadDispatcher with priority queue
  - [ ] ThreadDispatcherTimer
  - [ ] ThreadDispatcher async/await
  - [ ] StateMachine.TransitionAborted
  - [ ] StateMachine Updates
  - [ ] RuntimeEnvironment
  - [ ] Singleton (generic and non-generic)
  - [ ] AlphanumComparer
  - [ ] TemporaryFile
  - [ ] CultureInfoExtensions
  - [ ] CompositionContainer with parent container
  - [ ] Filtered log service
  - [ ] Filtered log writer
  - [ ] CSV
  - [ ] AggregateCatalog
  - [ ] FileCatalog
- [ ] Advertisement


## Backlog

- [ ] Wishlist
- [ ] Provide all possible overloads for timeout and cancellation token

## Proposals

- [ ] Unify cross-platform model and code
- [ ] AOT checking utility
- [ ] Migrate Common to Shared (own DLL using .NET Standard?)
