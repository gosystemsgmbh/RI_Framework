# Documentation

## Using the framework

Each release contains an API documentation which describes the assemblies, namespaces, types, and members of the various functions.

## History, Motivation & Reinventing the wheel

The Decoupling & Utilities Framework was originally a collection of functions which were used in multiple private projects, simply as a container for code reuse.

As it grew over time, accumulating more and more functionality, the framework was made into its own standalone project and eventually polished and open-sourced.

The motivation to write a particular function and put it into the framework was (and still is) one or more of the following:

 * It did not exist
 * It did exist, but the implementation was following too different requirements or guidelines
 * It did exist, but the implementation was unsatisfying (style, abstractions, customizability, quality, license, etc.)
 * Productivity concerns ("Learning/customizing takes at least as long as writing it myself")
 * Naivety, [not-invented-here](https://en.wikipedia.org/wiki/Not_invented_here) syndrom, overconfidence ("I can do this better")
 * Educational purposes ("I need to experience the problems of implementing this myself")
 * Fun (Ever tried to write a composition container yourself? What an adventure!)

Therefore, keep in mind that this framework is not a silver bullet and you should take the time to check whether it will really help you.

## Code organization

### Visual Studio

The history of the framework is still somewhat visible in its structure, mostly in a very Visual Studio centric organization and workflow.

There are no "loose files" as every file is a part of the projects Visual Studio solution or one of its C# projects.
A few C# projects are also "misused" for grouping special kind of files or to define a separate compilation unit.

### Build procedure

The build procedure relies entirely on Visual Studio as custom steps are implemented in batch scripts which are tied to Visual Studio project prebuild and postbuild events.

The build output is generated in the *_Output* folder in the directory of the Visual Studio solution.

Note that not all Visual Studio projects are built by default to save time while doing "normal" work on the code.
For example, the SHFB projects are not built by default.

### Assemblies vs. Namespaces

The separation of code into assemblies is based on dependencies and not functionality.

This means that everything which has the same dependencies is put into the same assembly, while everything which belongs to the same functionality is put into the same namespace.
