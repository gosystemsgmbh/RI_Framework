# Utility Framework

## First release

- [ ] Features
  - [X] File and directory path
  - [X] INI handling
  - [ ] Use FilePath and DirectoryPath
  - [ ] Additional dispatcher Features
  - [ ] Analyze and implement magic thread
  - [X] LogWriter for .NET
  - [ ] Fatal LogLevel
- [ ] Documentation
  - [ ] Examples
  - [ ] Type and member descriptions
  - [ ] Check all namespace descriptions
  - [ ] Conceptional documentation
- [ ] Cleanup
  - [X] Rename RI.Tools.Doc2Pdf to RI.Tools.Framework.Doc2Pdf
  - [X] Rename RI.Tools.VersionUpdater to RI.Tools.Framework.VersionUpdater
  - [ ] Exception throwing (messages, parameter names)
  - [X] Remove SuppressMessage attributes
  - [X] ReSharper configuration, cleanup, and review
  - [ ] Cleanup artifacts
  - [X] Split WPF stuff into separate assembly
  - [ ] WinForms similar to WPF
  - [ ] Locking for all LogWriters
  - [ ] Moving all randomizer stuff into Utilities.Randomizing
  - [ ] Check all usage of StringComparison and StringComparer
  - [ ] Use nameof() instead of GetType().Name for logging where possible (sealed classes)
  - [ ] Check usage of LogLocator (including used methods) and ServiceLocator
  - [ ] Overhaul SQLite module
  - [ ] Move IEnumerableExtensions to Linq subfolder
  - [ ] Create constants for literals
  - [ ] Make all enums serializable
  - [ ] Make all exceptions serializable
  - [ ] Check usage of IDisposable
  - [ ] Do we log correctly?
	- [ ] Source must be this.GetType()Name
	- [ ] ToString() overload and use for multi-instance objects
	- [ ] Log often and everywhere weher applicable
	- [ ] Private or protected log method
	- [ ] Check fixed log levels
	- [ ] Check correct null handling
  - [ ] Do all events use proper event handler and event args?
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

## Second release

- [ ] Features
  - [ ] RandomExtensions
  - [ ] RandomStream
  - [ ] UncloseableStream
  - [ ] StreamExtensions
  - [ ] I/O
  - [ ] Math
- [ ] Documentation
  - [ ] .NET documentation
- [ ] Cleanup
  - [ ] Move settings base to Common, add settings types for Unity
  - [ ] TODOs
- [ ] Testing
  - [ ] Complete tests
  - [ ] Run tests in .NET
  - [ ] Run tests in Unity
- [ ] Release
  - [ ] Web page
  - [ ] Unity Asset Store

## Later

- [ ] TODOs

## Candidates for moving to Common
- [ ] Resource Service
- [ ] Setting Service
