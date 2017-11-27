# Decoupling & Utilities Framework

A framework for the .NET Framework, Mono, and Unity.

## Releases

*no release yet*

## Overview

The Decoupling & Utilities Framework is a collection of various functions for the .NET Framework, Mono, and Unity.

It implements all kind of bigger functional blocks (the "decoupling" part), asorted small helpers (the "utilities" part), and extensions for other libraries/frameworks (both in the "decoupling" and "utilities" part).

All those functions, regardless whether big or small, are (mostly) independent of each other and can be used individually.

Due to the utilities part of the framework, which implements general-purpose functions not tied to a particular application type, the target audience can be everyone who uses .NET.

The decoupling part on the other hand has a more narrower target audience as it focuses on functionality common to the following application types:

 * Desktop applications
 * Desktop games
 * Server software

Therefore, it currently supports the following "fat" runtimes/targets:
 * [.NET Framework](https://www.microsoft.com/net/)
 * [Mono](http://www.mono-project.com/)
 * [Unity](https://unity3d.com/)

## Roadmap

Short-term and medium-term, the intention is to support the Decoupling and Utilities Framework with bugfixes, improvements, and small enhancements.

The long-term goal includes several larger endeavors:

 * Functionality for additional application types:
   * Web applications
   * Mobile games
 * Support for additional runtimes/targets:
   * .NET Standard
   * .NET Core (through .NET Standard)
 * Extensions for additional libraries/frameworks:
   * ASP.NET
   * ASP.NET Core
   * Entity Framework Core

## Compatibility

| Runtime                                          | Platform                                       | Version | Remarks                                                                 |
| ------------------------------------------------ | ---------------------------------------------- | ------- | ----------------------------------------------------------------------- |
| [.NET Framework](https://www.microsoft.com/net/) | Windows                                        | 4.6.1   |                                                                         |
| [Mono](http://www.mono-project.com/)             | Linux                                          | 5.4.1.6 |                                                                         |
| [Unity](https://unity3d.com/)                    | Windows, MacOS, iOS,<br>(Linux)\*, (Android)\* | 5.4.1f1 | * = not actively tested or supported                                    |

## Contents

N = .NET Framework <br> M = Mono <br> U = Unity <br> W = Windows (.NET Framework) <br> L = Linux (Mono)

### Decoupling

| Functionality         | Platform          | Namespace                              | Remarks                                                                          |
| --------------------- | ----------------- | -------------------------------------- | -------------------------------------------------------------------------------- |
| Message bus           | N, M              | RI.Framework.Bus.*                     | Simple address and/or type based asynchronous message bus                        |
| Composition container | N, M, U           | RI.Framework.Composition.*             | Dependency Injection, Inversion-of-Control (IoC)                                 |
| Database manager      | N, M              | RI.Framework.Data.Database.*           | Low-level database: script management, schema versioning, cleanup, backup        |
| Repository            | N, M              | RI.Framework.Data.Repository.*         | High-level database: generic repository, entities, views, filters, validation    |
| Model-View-ViewModel  | N                 | RI.Framework.Mvvm.*                    | MVVM basics for WPF                                                              |
| State machines        | N, M, U           | RI.Framework.StateMachines.*           | Hierarchical asynchronous state machines with signals, transients, and updates   |
| Thread dispatcher     | N, M              | RI.Framework.Threading.Dispatcher.*    | Platform-independent dispatcher with priorities, timers, and task scheduler      |
| Bootstrapper          | N, M, U           | RI.Framework.Services.*                | Application and service bootstrapper                                             |
| Backup                | N, M              | RI.Framework.Services.Backup.*         | Application data backup                                                          |
| Dispatcher            | U                 | RI.Framework.Services.Dispatcher.*     | Task dispatcher for Unity                                                        |
| Logging               | N, M, U           | RI.Framework.Services.Logging.*        | Technical logging                                                                |
| Modularization        | N, M, U           | RI.Framework.Services.Modularization.* | Modularizes application into functional modules                                  |
| Regions               | N, M              | RI.Framework.Services.Regions.*        | Modularizes GUI into separate regions                                            |
| Resources             | N, M              | RI.Framework.Services.Resources.*      | Application resource management and loading                                      |
| Settings              | N, M              | RI.Framework.Services.Settings.*       | Application settings and configuration                                           |

### Utilities

| Functionality         | Platform          | Namespace                              | Remarks                                                                          |
| --------------------- | ----------------- | -------------------------------------- | -------------------------------------------------------------------------------- |
| Collections           | N, M, U           | RI.Framework.Collections.*             | Collection types and utilities                                                   |
| Direct LINQ           | N, M, U           | RI.Framework.Collections.DirectLinq    | AOT-compatible LINQ replacement with immediate execution                         |
| I/O                   | N, M, U           | RI.Framework.IO.*                      | Binary, text, and stream types and utilities                                     |
| CSV                   | N, M, U           | RI.Framework.IO.CSV                    | Files and data with comma separated values                                       |
| INI                   | N, M, U           | RI.Framework.IO.INI                    | Files and data with key-value-pairs                                              |
| Keyboard              | W                 | RI.Framework.IO.Keyboard               | Low-level keyboard access                                                        |
| Printer               | W                 | RI.Framework.IO.Printer                | Low-level printer access                                                         |
| Serial                | W                 | RI.Framework.IO.Serial                 | Low-level serial port access                                                     |
| Paths                 | N, M, U           | RI.Framework.IO.Paths                  | Platform-independent file and directory path handling                            |
| Mathematics           | N, M, U           | RI.Framework.Mathematic.*              | Math types and utilities                                                         |
| Controllers           | N, M, U           | RI.Framework.Mathematic.Controllers    | PID controller                                                                   |
| Threading             | N, M, U           | RI.Framework.Threading.*               | Threading types and utilities                                                    |
| Basic utilities       | N, M, U           | RI.Framework.Utilities.*               | Utilities and extensions for basic CLR and BCL types and functions               |
| Windows               | W                 | RI.Framework.Windows.*                 | Windows platform-specific functions                                              |
| WPF                   | W                 | RI.Framework.Windows.Wpf.*             | Utilities and extensions for WPF                                                 |
| Linux                 | L                 | RI.Framework.Linux.*                   | Linux platform-specific functions                                                |
| Cross-platform        | N, M              | RI.Framework.CrossPlatform.*           | Platform-independent wrapper for RI.Framework.Windows.* and RI.Framework.Linux.* |

### Extensions

| Library / Framework                                                 | Platform          | Namespace                              | Version  |
| ------------------------------------------------------------------- | ----------------- | -------------------------------------- | -------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6)  | N, M              | RI.Framework.Data.EF.*                 | 6.1.3    |
| [SQLite](https://system.data.sqlite.org/)                           | N, M              | RI.Framework.Data.SQLite.*             | 1.0.106  |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)            | N, M              | RI.Framework.Data.SqlServer.*          |          |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                 | N, M              | RI.Framework.Web.Nancy.*               | 1.4.4    |
| [DotNetZip](https://dotnetzip.codeplex.com/)                        | N, M              |                                        | 1.10.1   |
| [EPPlus](https://github.com/JanKallman/EPPlus)                      | N, M              |                                        | 4.1.1    |
| [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit) | W                 |                                        | 3.2.0    |
| [Fluent Ribbon](https://github.com/fluentribbon/Fluent.Ribbon)      | W                 |                                        | 5.0.2.46 |
| [Mono.Posix](https://github.com/mono/mono)                          | L                 |                                        | 4.5.0    |
| [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json)       | N, M              |                                        | 10.0.3   |

## License

The Decoupling & Utilities Framework uses its own license, the Roten Informatik Framework License 1.0, which is mostly based on the [Apache 2.0 license](https://choosealicense.com/licenses/apache-2.0/).

See [License](LICENSE.txt) for more details.

## Documentation

Each release contains an API documentation which describes the assemblies, namespaces, types, and members of the various functions.

See [Documentation](DOCUMENTATION.md) for more details about the project itself.

## Contribution & Issues

See [Contribution](CONTRIBUTION.md) if you wish to contribute to the project or report issues.
