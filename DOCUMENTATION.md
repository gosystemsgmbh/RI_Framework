# Documentation

Please read the [overview](README.md) to get started and to get additional information.

## Using the framework

Each release contains a description about its assemblies, namespaces, types, and members.

## History, Motivation & Reinventing the wheel

The Decoupling & Utilities Framework was originally a collection of functions which were used in multiple private projects, simply as a container for code reuse.

As it grew over time, accumulating more and more functionality, the framework was made into its own standalone project and eventually polished and open-sourced.

The motivation to write a particular function and put it into the framework was (and still is) one or more of the following:

 * It did not exist.
 * It did exist, but the implementation was following too different requirements or guidelines.
 * It did exist, but the implementation was unsatisfying (style, abstractions, customizability, quality, license, etc.).
 * Productivity concerns ("Learning/customizing takes at least as long as writing it myself").
 * Naivety, [not-invented-here](https://en.wikipedia.org/wiki/Not_invented_here) syndrom, overconfidence ("I can do this better").
 * Educational purposes ("I need to experience the problems of implementing this myself").
 * Fun (ever tried to write a composition container yourself? What an adventure!).

Therefore, keep in mind that this framework is not a silver bullet and you should take the time to check whether it (or parts of it...) is really what you need.

## Code organization

### Visual Studio

The history of the framework is still somewhat visible in its structure, mostly in a very Visual Studio centric organization and workflow.

There are no "loose files" as every file is a part of the Visual Studio solution or one of its C# projects.

A few C# projects are also "misused" for file grouping/organization without actually producing an usable assembly.

### Build procedure

The build procedure relies entirely on Visual Studio as custom steps are implemented in batch scripts which are tied to Visual Studio project prebuild and postbuild events.

The build output is generated in the *_Output* folder in the directory of the Visual Studio solution.
The redistributable package is generated in the *_Packages* folder in the directory of the Visual Studio solution.

Note that to save time while doing "normal" work on the code, not all Visual Studio projects are built by default when the soultion is (re)built.

### Assemblies vs. Namespaces

The separation of code into assemblies is based on dependencies and not functionality.

This means that everything which has the same dependencies is put into the same assembly, while everything which belongs to the same functionality is put into the same namespace.
If a certain functionality is not used, simply do not use the corresponding namespace.

## Contents

The following abbrevations are used to indicate the supported runtimes/targets and/or platform:<br>
N = .NET Framework <br> M = Mono <br> U = Unity <br> W = Windows (.NET Framework) <br> L = Linux (Mono)

### Base assemblies

| Assembly                              | Runtime / Target / Platform | Remarks                                                                                    |
| ------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------------ |
| RI.Framework.NetFx.dll                | N, M                        | Main assembly for the .NET Framework and Mono                                              |
| RI.Framework.Unity.dll                | U                           | Main assembly for Unity                                                                    |

### Platform-dependent assemblies

| Assembly                              | Runtime / Target / Platform | Remarks                                                                                    |
| ------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------------ |
| RI.Framework.Windows.Common.dll       | W                           | Additional Windows functions                                                               |
| RI.Framework.Windows.Wpf.dll          | W                           | Additional Windows Presentation Foundation functions                                       |
| RI.Framework.Windows.Forms.dll        | W                           | Additional Windows Forms functions                                                         |
| RI.Framework.Windows.Service.dll      | W                           | Additional Winows Service functions                                                        |
| RI.Framework.Linux.Common.dll         | L                           | Additional Linux functions                                                                 |
| RI.Framework.CrossPlatform.Common.dll | N, M                        | Platform-independent wrapper for RI.Framework.Windows.\*.dll and RI.Framework.Linux.\*.dll |

### Extension assemblies

| Library / Framework                                                 | Version   | Assembly                                                                                                             | Runtime / Target / Platform | Remarks |
| ------------------------------------------------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------- | --------------------------- | ------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6)  | 6.2.0     | RI.Framework.Extensions.EF6.dll<br>RI.Framework.Extensions.SQLiteEF6.dll<br>RI.Framework.Extensions.SqlServerEF6.dll | N, M                        |         |
| [SQLite](https://system.data.sqlite.org/)                           | 1.0.108   | RI.Framework.Extensions.SQLite.dll<br>RI.Framework.Extensions.SQLiteEF6.dll                                          | N, M                        |         |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)            | 2008+     | RI.Framework.Extensions.SqlServer.dll<br>RI.Framework.Extensions.SqlServerEF6.dll                                    | N, M                        |         |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                 | 1.4.4     | RI.Framework.Extensions.Nancy.dll<br>RI.Framework.Extensions.NancyJson.dll                                           | N, M                        |         |
| [DotNetZip](https://dotnetzip.codeplex.com/)                        | 1.10.1    | RI.Framework.Extensions.DotNetZip.dll                                                                                | N, M                        |         |
| [EPPlus](https://github.com/JanKallman/EPPlus)                      | 4.1.1     | RI.Framework.Extensions.EPPlus.dll                                                                                   | N, M                        |         |
| [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json)       | 10.0.3    | RI.Framework.Extensions.Json.dll<br>RI.Framework.Extensions.NancyJson.dll                                            | N, M                        |         |
| [Bouncy Castle](https://github.com/onovotny/bc-csharp)              | 1.8.1.3   | RI.Framework.Extensions.BouncyCastle.dll                                                                             | N, M                        |         |
| [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit) | 3.2.0     | RI.Framework.Extensions.WpfToolkit.dll                                                                               | W                           |         |
| [Fluent Ribbon](https://github.com/fluentribbon/Fluent.Ribbon)      | 6.0.0.208 | RI.Framework.Extensions.FluentRibbon.dll                                                                             | W                           |         |
| [Mono.Posix](https://github.com/mono/mono)                          | 4.5.0     | RI.Framework.Linux.Shared.dll                                                                                        | L                           |         |

### Decoupling namespaces

| Functionality         | Namespace                               | Runtime / Target / Platform | Remarks                                                                                          |
| --------------------- | --------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------------------ |
| Bootstrapper          | RI.Framework.Bootstrapping.\*           | N, M, U                     | Application and service bootstrapper                                                             |
| Composition container | RI.Framework.Composition.\*             | N, M, U                     | Dependency Injection (DI), Inversion-of-Control (IoC)                                            |
| State machines        | RI.Framework.StateMachines.\*           | N, M, U                     | Hierarchical, asynchronous state machines with signals, transients, and updates                  |
| Message bus           | RI.Framework.Bus.\*                     | N, M                        | Simple address and/or type based, local and/or distributed, asynchronous message bus             |
| Database manager      | RI.Framework.Data.Database.\*           | N, M                        | Low-level database: script management, connection management, schema versioning, cleanup, backup |
| Repository            | RI.Framework.Data.Repository.\*         | N, M                        | High-level database: generic repository, entities, views, filters, validation                    |
| Model-View-ViewModel  | RI.Framework.Mvvm.\*                    | W                           | Model-View-ViewModel (MVVM) base infrastructure for Windows Presentation Foundation              |
| Logging               | RI.Framework.Services.Logging.\*        | N, M, U                     | Logging service                                                                                  |
| Modularization        | RI.Framework.Services.Modularization.\* | N, M, U                     | Service to modularize applications into functional blocks                                        |
| Regions               | RI.Framework.Services.Regions.\*        | N, M                        | Service to modularize GUIs into separate regions                                                 |
| Settings              | RI.Framework.Services.Settings.\*       | N, M                        | Application settings and configuration management service                                        |
| Resources             | RI.Framework.Services.Resources.\*      | N, M                        | Application resource management and loading service                                              |
| Backup                | RI.Framework.Services.Backup.\*         | N, M                        | Application data backup service                                                                  |
| Dispatcher            | RI.Framework.Services.Dispatcher.\*     | U                           | Task dispatcher service for Unity                                                                |

### Utilities namespaces

| Functionality         | Namespace                               | Runtime / Target / Platform | Remarks                                                                                          |
| --------------------- | --------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------------------ |
| Basic utilities       | RI.Framework.Utilities.\*               | N, M, U                     | Utilities and extensions for basic CLR and BCL types and functions                               |
| Components            | RI.Framework.ComponentModel             | N, M, U                     | Types and utilities to componentize an application and to handle its dependencies                |
| Collections           | RI.Framework.Collections.\*             | N, M, U                     | Collection types and utilities                                                                   |
| Direct LINQ           | RI.Framework.Collections.DirectLinq     | N, M, U                     | AOT-compatible LINQ subset/replacement with immediate execution                                  |
| I/O                   | RI.Framework.IO.\*                      | N, M, U                     | Binary, text, stream types and utilities                                                         |
| CSV                   | RI.Framework.IO.CSV                     | N, M, U                     | Files and data with comma separated values                                                       |
| INI                   | RI.Framework.IO.INI                     | N, M, U                     | Files and data with key-value-pairs                                                              |
| Paths                 | RI.Framework.IO.Paths                   | N, M, U                     | Platform-independent file and directory path handling                                            |
| Keyboard              | RI.Framework.IO.Keyboard                | W                           | Low-level keyboard access                                                                        |
| Printer               | RI.Framework.IO.Printer                 | W                           | Low-level printer access (e.g. for POS printers)                                                 |
| Serial                | RI.Framework.IO.Serial                  | W                           | Low-level serial port access                                                                     |
| Mathematics           | RI.Framework.Mathematic.\*              | N, M, U                     | Math types and utilities                                                                         |
| Controllers           | RI.Framework.Mathematic.Controllers     | N, M, U                     | PID controller                                                                                   |
| Threading             | RI.Framework.Threading.\*               | N, M, U                     | Threading types and utilities                                                                    |
| Thread dispatcher     | RI.Framework.Threading.Dispatcher       | N, M                        | Platform-independent dispatcher with priorities, timers, task scheduler, synchronization context |
| Windows               | RI.Framework.Windows.\*                 | W                           | Windows platform-specific functions                                                              |
| Linux                 | RI.Framework.Linux.\*                   | L                           | Linux platform-specific functions                                                                |
| Cross-platform        | RI.Framework.CrossPlatform.\*           | N, M                        | Platform-independent wrapper for RI.Framework.Windows.\* and RI.Framework.Linux.\*               |

### Extension namespaces

| Library / Framework                                                | Namespace                      | Runtime / Target / Platform | Remarks |
| ------------------------------------------------------------------ | ------------------------------ | --------------------------- | ------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6) | RI.Framework.Data.EF.\*        | N, M                        |         |
| [SQLite](https://system.data.sqlite.org/)                          | RI.Framework.Data.SQLite.\*    | N, M                        |         |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)           | RI.Framework.Data.SqlServer.\* | N, M                        |         |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                | RI.Framework.Web.Nancy.\*      | N, M                        |         |
