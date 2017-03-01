# Framework

## TODO

- [ ] Features
  - [ ] Implement things from Wishlist
- [ ] Documentation
  - [ ] Complete list of examples
  - [ ] Type and member descriptions
  - [ ] Check all namespace descriptions (in final documentation)
  - [ ] Conceptional documentation
  - [ ] Cleanup artifacts
- [ ] Cleanup
  - [ ] Check whether nested types really need to be nested
  - [ ] Exception throwing
    - [ ] Always use parameter names for argument exceptions
    - [ ] Always provide message or at least parameter name (for argument exceptions only)
  - [ ] Moving all randomizer stuff into Utilities.Randomizing
  - [ ] Check all usage of StringComparison and StringComparer
  - [ ] Check usage of ServiceLocator
  - [ ] Make all enums serializable
  - [ ] Make all exceptions serializable
  - [ ] Check usage of IDisposable
  - [ ] Do we log correctly?
    - [ ] ISupportsLogging
    - [ ] Usage of LogLocator, including used methods
    - [ ] Source must be this.GetType()Name
    - [ ] ToString() overload and use for multi-instance objects
    - [ ] Log often and everywhere weher applicable
    - [ ] Private or protected log method
    - [ ] Check fixed log levels
    - [ ] Check correct null handling
  - [ ] Do all events use proper event handler and event args?
  - [ ] TODOs
- [ ] Testing
  - [ ] Remove test projects from ReSharper exclusion
  - [ ] Complete tests
  - [ ] Run tests in .NET
  - [ ] Run tests in Unity
- [ ] Release
  - [ ] Promotional material
  - [ ] Web page
  - [ ] Unity Asset Store

## Future

- [ ] Migrate Common to Shared
  - [ ] Migrate Common to Shared
  - [ ] Add reference to RI.Framework.Shared to all projects
- [ ] Moving to Shared
  - [ ] Resource Service
  - [ ] Setting Service
