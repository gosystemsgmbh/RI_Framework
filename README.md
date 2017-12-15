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
   * ASP.NET Core
   * Entity Framework Core

## Compatibility

| Runtime                                          | Version | Platform                                         | Remarks                                                              |
| ------------------------------------------------ | ------- | ------------------------------------------------ | -------------------------------------------------------------------- |
| [.NET Framework](https://www.microsoft.com/net/) | 4.6.1   | Windows                                          |                                                                      |
| [Mono](http://www.mono-project.com/)             | 5.4.1.6 | Linux                                            |                                                                      |
| [Unity](https://unity3d.com/)                    | 5.4.1f1 | Windows, MacOS, iOS\*,<br>Linux\*\*, Android\*\* | \* = including AOT/IL2CPP<br>\*\* = not actively tested or supported |

## Contents

N = .NET Framework <br> M = Mono <br> U = Unity <br> W = Windows (.NET Framework) <br> L = Linux (Mono)

See the [documentation](DOCUMENTATION.md) for more details about how to use the Decoupling and Utilities Framework and its separation into namespaces and assemblies.

### Decoupling namespaces

| Functionality         | Namespace                               | Platform          | Remarks                                                                                          |
| --------------------- | --------------------------------------- | ----------------- | ------------------------------------------------------------------------------------------------ |
| Composition container | RI.Framework.Composition.\*             | N, M, U           | Dependency Injection (DI), Inversion-of-Control (IoC)                                            |
| State machines        | RI.Framework.StateMachines.\*           | N, M, U           | Hierarchical, asynchronous state machines with signals, transients, and updates                  |
| Message bus           | RI.Framework.Bus.\*                     | N, M              | Simple address and/or type based, local and/or distributed, asynchronous message bus             |
| Database manager      | RI.Framework.Data.Database.\*           | N, M              | Low-level database: script management, connection management, schema versioning, cleanup, backup |
| Repository            | RI.Framework.Data.Repository.\*         | N, M              | High-level database: generic repository, entities, views, filters, validation                    |
| Model-View-ViewModel  | RI.Framework.Mvvm.\*                    | W                 | Model-View-ViewModel (MVVM) base infrastructure for Windows Presentation Foundation              |
| Bootstrapper          | RI.Framework.Services                   | N, M, U           | Application and service bootstrapper                                                             |
| Logging               | RI.Framework.Services.Logging.\*        | N, M, U           | Logging service                                                                                  |
| Modularization        | RI.Framework.Services.Modularization.\* | N, M, U           | Service to modularize applications into functional blocks                                        |
| Regions               | RI.Framework.Services.Regions.\*        | N, M              | Service to modularize GUIs into separate regions                                                 |
| Settings              | RI.Framework.Services.Settings.\*       | N, M              | Application settings and configuration management service                                        |
| Resources             | RI.Framework.Services.Resources.\*      | N, M              | Application resource management and loading service                                              |
| Backup                | RI.Framework.Services.Backup.\*         | N, M              | Application data backup service                                                                  |
| Dispatcher            | RI.Framework.Services.Dispatcher.\*     | U                 | Task dispatcher service for Unity                                                                |

### Utilities namespaces

| Functionality         | Namespace                               | Platform          | Remarks                                                                                          |
| --------------------- | --------------------------------------- | ----------------- | ------------------------------------------------------------------------------------------------ |
| Components            | RI.Framework.ComponentModel             | N, M, U           | Types and utilities to componentize an application and to handle its dependencies                |
| Collections           | RI.Framework.Collections.\*             | N, M, U           | Collection types and utilities                                                                   |
| Direct LINQ           | RI.Framework.Collections.DirectLinq     | N, M, U           | AOT-compatible LINQ subset/replacement with immediate execution                                  |
| I/O                   | RI.Framework.IO.\*                      | N, M, U           | Binary, text, stream types and utilities                                                         |
| CSV                   | RI.Framework.IO.CSV                     | N, M, U           | Files and data with comma separated values                                                       |
| INI                   | RI.Framework.IO.INI                     | N, M, U           | Files and data with key-value-pairs                                                              |
| Paths                 | RI.Framework.IO.Paths                   | N, M, U           | Platform-independent file and directory path handling                                            |
| Keyboard              | RI.Framework.IO.Keyboard                | W                 | Low-level keyboard access                                                                        |
| Printer               | RI.Framework.IO.Printer                 | W                 | Low-level printer access (e.g. for POS printers)                                                 |
| Serial                | RI.Framework.IO.Serial                  | W                 | Low-level serial port access                                                                     |
| Mathematics           | RI.Framework.Mathematic.\*              | N, M, U           | Math types and utilities                                                                         |
| Controllers           | RI.Framework.Mathematic.Controllers     | N, M, U           | PID controller                                                                                   |
| Threading             | RI.Framework.Threading.\*               | N, M, U           | Threading types and utilities                                                                    |
| Thread dispatcher     | RI.Framework.Threading.Dispatcher       | N, M              | Platform-independent dispatcher with priorities, timers, task scheduler, synchronization context |
| Basic utilities       | RI.Framework.Utilities.\*               | N, M, U           | Utilities and extensions for basic CLR and BCL types and functions                               |
| Windows               | RI.Framework.Windows.\*                 | W                 | Windows platform-specific functions                                                              |
| Linux                 | RI.Framework.Linux.\*                   | L                 | Linux platform-specific functions                                                                |
| Cross-platform        | RI.Framework.CrossPlatform.\*           | N, M              | Platform-independent wrapper for RI.Framework.Windows.\* and RI.Framework.Linux.\*               |

### Extension namespaces

| Library / Framework                                                | Namespace                      | Platform          |
| ------------------------------------------------------------------ | ------------------------------ | ----------------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6) | RI.Framework.Data.EF.\*        | N, M, U           |
| [SQLite](https://system.data.sqlite.org/)                          | RI.Framework.Data.SQLite.\*    | N, M, U           |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)           | RI.Framework.Data.SqlServer.\* | N, M, U           |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                | RI.Framework.Web.Nancy.\*      | N, M, U           |

### Base assemblies

| Assembly                              | Platform | Remarks                                                                                    |
| ------------------------------------- | -------- | ------------------------------------------------------------------------------------------ |
| RI.Framework.NetFx.dll                | N, M     | Functions for the .NET Framework and Mono without additional dependencies                  |
| RI.Framework.Unity.dll                | U        | Functions for Unity without additional dependencies                                        |
| RI.Framework.Windows.Shared.dll       | W        | Functions for Windows                                                                      |
| RI.Framework.Windows.Wpf.dll          | W        | Functions for Windows Presentation Foundation                                              |
| RI.Framework.Windows.Forms.dll        | W        | Functions for Windows Forms                                                                |
| RI.Framework.Windows.Service.dll      | W        | Functions for Windows Services                                                             |
| RI.Framework.Linux.Shared.dll         | L        | Functions for Linux                                                                        |
| RI.Framework.CrossPlatform.Shared.dll | N, M     | Platform-independent wrapper for RI.Framework.Windows.\*.dll and RI.Framework.Linux.\*.dll |

### Extension assemblies

| Library / Framework                                                 | Version  | Assembly                                                                                                             | Platform          |
| ------------------------------------------------------------------- | -------- | -------------------------------------------------------------------------------------------------------------------- | ----------------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6)  | 6.1.3    | RI.Framework.Extensions.EF6.dll<br>RI.Framework.Extensions.SQLiteEF6.dll<br>RI.Framework.Extensions.SqlServerEF6.dll | N, M              |
| [SQLite](https://system.data.sqlite.org/)                           | 1.0.106  | RI.Framework.Extensions.SQLite.dll<br>RI.Framework.Extensions.SQLiteEF6.dll                                          | N, M              |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)            | 2008+    | RI.Framework.Extensions.SqlServer.dll<br>RI.Framework.Extensions.SqlServerEF6.dll                                    | N, M              |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                 | 1.4.4    | RI.Framework.Extensions.Nancy.dll<br>RI.Framework.Extensions.NancyJson.dll                                           | N, M              |
| [DotNetZip](https://dotnetzip.codeplex.com/)                        | 1.10.1   | RI.Framework.Extensions.DotNetZip.dll                                                                                | N, M              |
| [EPPlus](https://github.com/JanKallman/EPPlus)                      | 4.1.1    | RI.Framework.Extensions.EPPlus.dll                                                                                   | N, M              |
| [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json)       | 10.0.3   | RI.Framework.Extensions.Json.dll<br>RI.Framework.Extensions.NancyJson.dll                                            | N, M              |
| [Bouncy Castle](https://github.com/onovotny/bc-csharp)              | 1.8.1.3  | RI.Framework.Extensions.BouncyCastle.dll                                                                             | N, M              |
| [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit) | 3.2.0    | RI.Framework.Extensions.WpfToolkit.dll                                                                               | W                 |
| [Fluent Ribbon](https://github.com/fluentribbon/Fluent.Ribbon)      | 5.0.2.46 | RI.Framework.Extensions.FluentRibbon.dll                                                                             | W                 |
| [Mono.Posix](https://github.com/mono/mono)                          | 4.5.0    | RI.Framework.Linux.Shared.dll                                                                                        | L                 |

## License

The Decoupling & Utilities Framework uses its own license, the Roten Informatik Framework License 1.0, which is mostly based on the [Apache 2.0 license](https://choosealicense.com/licenses/apache-2.0/).

See [License](LICENSE.txt) for more details and the license itself.

## Documentation

Each [Release](#releases) contains an API documentation which describes the assemblies, namespaces, types, and members of the various functions.

See [Documentation](DOCUMENTATION.md) for more details about the project and code.

## Contribution & Issues

See [Contribution](CONTRIBUTING.md) if you wish to contribute to the project or report [issues](https://github.com/RotenInformatik/RI_Framework/issues).
