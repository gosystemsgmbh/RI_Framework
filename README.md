# Decoupling & Utilities Framework

A framework for .NET, Mono, and Unity.

## Releases

*no release yet*

## Overview

The Decoupling & Utilities Framework is a collection of various functions for different .NET flavours.

![Overview](README-overview.png)

TODO
It contains...
 * ...functional blocks for application infrastructure and services common to most applications (e.g. bootstrapper, logging, settings, resources, ...)
 * ...types which help to create decoupled and maintainable code (e.g. dependency injection, database manager, data repository, state machines, message bus, ...)
 * ...asorted small helpers and utilities (e.g. collections, threading, MVVM, math, I/O, ...)
 * ...extensions for other libraries/frameworks to use them with the Decoupling & Utilities Framework (e.g. SQLite, Entity Framework, ...)

 TODO

All those functions are mostly independent of each other and can be used individually.
So you can simply ignore functions you do not want to use.

The "utilities" part implements general-purpose functionality not tied to a particular application type.
Therefore, the target audience of this framework can be everyone who develops for one of the .NET flavours.

The "decoupling" part on the other hand has a more narrower target audience as it focuses on functionality common to the following application types:

 * Desktop applications
 * Desktop games
 * Server applications

## Compatibility

Not all functions are available for all runtimes/targets or on all platforms. See documentation for details.

| Runtime / Target                                 | Version  | Platform                                           | Remarks                                                                       |
| ------------------------------------------------ | -------- | -------------------------------------------------- | ----------------------------------------------------------------------------- |
| [.NET Framework](https://www.microsoft.com/net/) | 4.6.1    | Windows                                            |                                                                               |
| [Mono](http://www.mono-project.com/)             | 5.4.1.6  | Linux                                              |                                                                               |
| [Unity](https://unity3d.com/)                    | 2018.1.1 | Windows, MacOS<br>iOS\*\*, Linux\*\*, Android\*\*  | including AOT/IL2CPP<br>\*\* = not actively tested or supported at the moment |

## Dependencies

The dependencies only apply if the corresponding extension is used. Otherwise there are no dependencies besides the used runtime/target and platform.

| Library / Framework                                                 | Version   | Runtime / Target     |
| ------------------------------------------------------------------- | --------- | -------------------- |
| [Entity Framework 6.x](https://github.com/aspnet/EntityFramework6)  | 6.2.0     | .NET Framework, Mono |
| [SQLite](https://system.data.sqlite.org/)                           | 1.0.108   | .NET Framework, Mono |
| [SQL Server](https://www.microsoft.com/en-us/sql-server)            | 2008+     | .NET Framework, Mono |
| [Nancy Framework](https://github.com/NancyFx/Nancy)                 | 1.4.4     | .NET Framework, Mono |
| [DotNetZip](https://dotnetzip.codeplex.com/)                        | 1.11.1    | .NET Framework, Mono |
| [EPPlus](https://github.com/JanKallman/EPPlus)                      | 4.1.1     | .NET Framework, Mono |
| [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json)       | 10.0.3    | .NET Framework, Mono |
| [Bouncy Castle](https://github.com/onovotny/bc-csharp)              | 1.8.1.3   | .NET Framework, Mono |
| [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit) | 3.2.0     | .NET Framework       |
| [Fluent Ribbon](https://github.com/fluentribbon/Fluent.Ribbon)      | 6.0.0.208 | .NET Framework       |
| [Mono.Posix](https://github.com/mono/mono)                          | 4.5.0     | Mono                 |

## Roadmap

The short-term and medium-term roadmap includes:

 * More unit tests
 * Bugfixes
 * Improvement of existing functions
 * Small and medium enhancements
 * More documentation, examples, and tutorials

The long-term roadmap includes several large enhancements:

 * Functionality for additional application types:
   * Mobile applications
   * Mobile games
   * Web applications
 * Support for additional runtimes/targets:
   * .NET Standard
   * .NET Core (through .NET Standard)
   * Xamarin
 * Extensions for additional libraries/frameworks:
   * ASP.NET Core
   * Entity Framework Core

## Documentation

Each release contains a description about its assemblies, namespaces, types, and members.

The project itself and its structure/organization is described in the [project documentation](DOCUMENTATION.md).

## Contribution & Issues

See [contribution documentation](CONTRIBUTING.md) if you wish to contribute to the project or report [issues](https://github.com/RotenInformatik/RI_Framework/issues).

## License

The Decoupling & Utilities Framework uses its own license, the [Roten Informatik Framework License 1.0](LICENSE.txt), which is mostly based on the [Apache 2.0 License](https://choosealicense.com/licenses/apache-2.0/).
