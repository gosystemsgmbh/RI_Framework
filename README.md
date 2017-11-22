# Decoupling & Utilities Framework

A framework for the .NET Framework, Mono, and Unity.

## Releases

*no release yet*

## Overview

The Decoupling & Utilities Framework is a collection of various functions for the .NET Framework, Mono, and Unity.

It implements all kind of bigger functional blocks (the "decoupling" part), asorted small helpers (the "utilities" part), and extensions for other libraries/frameworks.

All those functions, regardless whether big or small, are (mostly) independent of each other and can be used individually.

## Compatibility

At this moment, the following runtimes and platforms are supported:

| Runtime        | Platform                                       | Version | Remarks                                                                 |
| -------------- | ---------------------------------------------- | ------- | ----------------------------------------------------------------------- |
| .NET Framework | Windows                                        | 4.6.1   |                                                                         |
| Mono           | Linux                                          | 5.4.1.6 |                                                                         |
| Unity          | Windows, MacOS, iOS,<br>(Linux)\*, (Android)\* | 5.4.1f1 | * = not tested, not supported                                           |

.NET Core and the .NET Standard are currently **not** supported.
Support for those is on the roadmap as a long-term goal (through .NET Standard).

## Contents

N = .NET Framework / M = Mono / U = Unity / W = .NET Framework, Windows only / L = Mono, Linux only

### Decoupling

| Functionality         | Platform          | Namespace                              | Remarks                                                                          |
| --------------------- | ----------------- | -------------------------------------- | -------------------------------------------------------------------------------- |
| Message bus           | N, M              | RI.Framework.Bus.*                     | Simple address and/or type based asynchronous message bus                        |
| Composition container | N, M, U           | RI.Framework.Composition.*             | Dependency Injection, Inversion-of-Control (IoC)                                 |
| Database manager      | N, M              | RI.Framework.Data.Database.*           | Low-level database: script management, schema versioning, cleanup, backup        |
| Repository            | N, M              | RI.Framework.Data.Repository.*         | High-level database: generic repository, entities, views, filters, validation    |
| Model-View-ViewModel  | W                 | RI.Framework.Mvvm.*                    | MVVM basics for WPF                                                              |
| State machines        | N, M, U           | RI.Framework.StateMachines.*           | Hierarchical asynchronous state machines with signals, transients, and updates   |
| Thread dispatcher     | N, M              | RI.Framework.Threading.Dispatcher.*    | Platform-independent dispatcher with priorities, timers, and task scheduler      |
| Bootstrapper          | N, M, U           | RI.Framework.Services.*                | Application and service bootstrapper                                             |
| Backup                | N, M              | RI.Framework.Services.Backup.*         | Application data backup                                                          |
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

| Library / Framework   | Platform          | Namespace                              | Version  |
| --------------------- | ----------------- | -------------------------------------- | -------- |
| Entity Framework 6.x  | N, M              | RI.Framework.Data.EF.*                 | 6.1.3    |
| SQLite                | N, M              | RI.Framework.Data.SQLite.*             | 1.0.106  |
| SQL Server            | N, M              | RI.Framework.Data.SqlServer.*          |          |
| Nancy Framework       | N, M              | RI.Framework.Web.Nancy.*               | 1.4.4    |
| DotNetZip             | N, M              |                                        | 1.10.1   |
| EPPlus                | N, M              |                                        | 4.1.1    |
| Extended WPF Toolkit  | W                 |                                        | 3.2.0    |
| Fluent Ribbon         | W                 |                                        | 5.0.2.46 |
| Mono.Posix            | L                 |                                        | 4.5.0    |
| Newtonsoft JSON       | N, M              |                                        | 10.0.3   |

## Target audience

Due to the utilities part of the framework, which implements general purpose functions not tied to a particular application type or structure, the target audience is simply everyone who uses the .NET Framework, Mono, or Unity.

The decoupling part on the other hand has a more narrower target audience as it focuses on implementation of mostly infrastructure functions common to desktop applications, server software, and games.

## Origin & Motivation & Reinventing the wheel

The framework was originally a collection of functions which were used in multiple private projects, simply as a container for code reuse.

As it grew over time, accumulating more and more functionality, the framework was made into its own standalone project and eventually polished and open-sourced.

The motivation for open-sourcing the framework is simply that others might benefit from it and to potentially improve it through exposure, feedback, and contribution.

The motivation for writing a particular function and put it into the framework was/is one or more of the following:

 * It did not exist
 * It did exist, but the implementation was following too different requirements
 * It did exist, but the implementation was unsatisfying (style, abstractions, customizability, quality, license, etc.)
 * Productivity concerns ("Learning/customizing takes at least as long as writing it myself")
 * Naivety, not-invented-here syndrom, overconfidence ("I can do this better")
 * Educational purposes ("I need to experience the problems of a message bus implementation myself")
 * Fun (Ever tried to write a composition container yourself? What an adventure!)

## Documentation

See [Documentation](DOCUMENTATION.md) for development and usage documentation.

## License

The Decoupling & Utilities Framework uses its own license, the Roten Informatik Framework License, which is mostly based on the Apache 2.0 license.
See [License](LICENSE.md) for more details.

## Contribution

If you wish to contribute to the project, see [Contribution](CONTRIBUTION.md) for more details.
