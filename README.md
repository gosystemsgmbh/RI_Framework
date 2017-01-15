# Utility Framework

## First release

- [ ] Features
  - [X] File and directory path
  - [X] INI handling
  - [ ] Use FilePath and DirectoryPath everywhere
  - [X] LogWriter for .NET
- [ ] Documentation
  - [ ] Complete list of examples
  - [ ] Type and member descriptions
  - [ ] Check all namespace descriptions
  - [ ] Conceptional documentation
- [ ] Cleanup
  - [ ] All ReSharper configurations of all solutions (shared settings file in own repository?)
  - [X] Rename RI.Tools.Doc2Pdf to RI.Tools.Framework.Doc2Pdf
  - [X] Rename RI.Tools.VersionUpdater to RI.Tools.Framework.VersionUpdater
  - [ ] Check wherther nested types really need to be nested
  - [ ] Exception throwing
    - [ ] Always use parameter names for argument exceptions
    - [ ] Always provide message or at least parameter name (for argument exceptions only)
  - [X] Remove SuppressMessage attributes
  - [X] ReSharper configuration, cleanup, and review
  - [ ] Cleanup artifacts
  - [X] Split WPF stuff into separate assembly
  - [ ] Moving all randomizer stuff into Utilities.Randomizing
  - [ ] Check all usage of StringComparison and StringComparer
  - [ ] Check usage of ServiceLocator
  - [X] Overhaul SQLite module
  - [X] Move IEnumerableExtensions to Linq subfolder
  - [ ] Create constants for literals
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
  - [ ] Rename EF to EF6
  - [ ] Rename SQLiteEF to SQLiteEF6
  - [X] Add NuGet packages to Libraries project
  - [ ] TODOs
- [ ] Testing
  - [X] Split testing into "Unity" and ".NET"
  - [ ] Remove test projects from ReSharper exclusion
  - [ ] Complete tests
  - [ ] Run tests in .NET
  - [ ] Run tests in Unity
- [ ] Release
  - [ ] Promotional material
  - [ ] Web page
  - [ ] Unity Asset Store

## Future

- [ ] Moving to Common/Shared
  - [ ] Resource Service
  - [ ] Setting Service
- [ ] Migrate Common to Shared
  - [ ] Migrate Common to Shared
  - [ ] Add reference to RI.Framework.Shared to all projects
