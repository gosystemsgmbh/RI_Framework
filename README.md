# Decoupling & Utilities Framework

A framework for .NET, Mono, and Unity.

## Releases

TODO

## Contents

### Decoupling

*Bigger functional blocks*

| Functionality         | Platform          | Namespace                           | Remarks                                                                        |
| --------------------- | ----------------- | ----------------------------------- | ------------------------------------------------------------------------------ |
| Message bus           | .NET, Mono        | RI.Framework.Bus.*                  | Simple address and/or type based asynchronous message bus                      |
| Composition container | .NET, Mono, Unity | RI.Framework.Composition.*          | Dependency Injection, Inversion-of-Control (IoC)                               |
| Database manager      | .NET, Mono        | RI.Framework.Data.Database.*        | Low-level database: schema versioning, script management, backup, cleanup      |
| Repository            | .NET, Mono        | RI.Framework.Data.Repository.*      | High-level database: generic repository, entities, filters, validation         |
| Model-View-ViewModel  | .NET              | RI.Framework.Mvvm.*                 | MVVM basics for WPF                                                            |
| State machines        | .NET, Mono, Unity | RI.Framework.StateMachines.*        | Hierarchical asynchronous state machines with signals, transients, and updates |
| Thread dispatcher     | .NET, Mono        | RI.Framework.Threading.Dispatcher.* | Platform-independent dispatcher with priorities, timers, and task scheduler    |

### Utilities

*Asorted small helpers*

| Functionality    | Platform          | Namespace                             | Remarks                                                                          |
| ---------------- | ----------------- | ------------------------------------- | -------------------------------------------------------------------------------- |
| Collections      | .NET, Mono, Unity | RI.Framework.Collections.*            |                                                                                  |
| Direct LINQ      | .NET, Mono, Unity | RI.Framework.Collections.DirectLinq   | AOT-compatible LINQ replacement with immediate execution                         |
| I/O              | .NET, Mono, Unity | RI.Framework.IO.*                     |                                                                                  |
| CSV              | .NET, Mono, Unity | RI.Framework.IO.CSV.*                 | Files and data with comma separated values                                       |
| INI              | .NET, Mono, Unity | RI.Framework.IO.INI.*                 | Files and data with key-value-pairs in the traditional INI style                 |
| Filesystem paths | .NET, Mono, Unity | RI.Framework.IO.Paths.*               | Platform-independent file and directory path handling                            |
| Mathematics      | .NET, Mono, Unity | RI.Framework.Mathematic.*             |                                                                                  |
| PID controller   | .NET, Mono, Unity | RI.Framework.Mathematic.Controllers.* |                                                                                  |
| Threading        | .NET, Mono, Unity | RI.Framework.Threading.*              |                                                                                  |
| Basic utilities  | .NET, Mono, Unity | RI.Framework.Utilities.*              | Utilities and extensions for basic CLR and BCL types and functionality           |
| Windows platform | .NET, Windows     | RI.Framework.Windows.*                | Windows platform-specific functionality                                          |
| Linux platform   | Mono, Linux       | RI.Framework.Linux.*                  | Linux platform-specific functionality                                            |
| Cross-platform   | .NET, Mono        | RI.Framework.CrossPlatform.*          | Platform-independent wrapper for common functions of Windows and Linux functions |

### Extensions

*Extensions for other libraries/frameworks*

| Library / Framework   | Platform          | Namespace                           | Remarks                                                  |
| --------------------- | ----------------- | ----------------------------------- | -------------------------------------------------------- |
| Entity Framework 6.x  | .NET, Mono        | RI.Framework.Data.EF.*              |                                                          |
| SQLite                | .NET, Mono        | RI.Framework.Data.SQLite.*          |                                                          |
| SQL Server            | .NET, Mono        | RI.Framework.Data.SqlServer.*       |                                                          |
| Nancy Framework       | .NET, Mono        | RI.Framework.Web.Nancy.*            |                                                          |

## Overview

The Decoupling & Utilities Framework is a collection of various functions for .NET, Mono, and Unity.

It implements all kinds of bigger functional blocks (the "decoupling" part, e.g. dependency injection, state machines, message bus, etc.), asorted small helpers (the "utilities" part, e.g. string extensions, roman number conversion, math functions, etc.), and extensions for other libraries/frameworks.

All those functions, regardless whether big or small, are (mostly) independent of each other and can be used individually, as needed.

## Origin

The framework was originally a collection of functions which were used in multiple private projects, simply as a container for code reuse.

As it grew over time, accumulating more and more functionality which were and are commonly used in mainly desktop applications, server software, and games, the framework was made into its own project and eventually polished and open-sourced.

The history of the project is still somewhat visible in the structure, for example a very Visual Studio centric organization and workflow or that the separation into assemblies is based on dependencies and not functionality.

## Target audience

Due to the utilities part of the framework, which implements general purpose functionality not tied to a particular application type or structure, the target audience is simply everyone who uses the .NET Framework, Mono, or Unity.

The decoupling part on the other hand has a more narrower target audience as it focuses on implementation of infrastructure functionality common to "real world" desktop applications, server software, and games.

## "Real world" & reinventing the wheel

TODO

## Documentation

TODO

## License

The Decoupling & Utilities Framework uses its own license, the Roten Informatik Framework License, which is mostly based on the Apache 2.0 license.

See [License](LICENSE.md) for more details.

## Contribution

If you wish to contribute to the project, see [Contribution](CONTRIBUTION.md) for more details.
