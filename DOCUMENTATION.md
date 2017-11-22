# Documentation

:warning: Work in progress! :warning: Not released yet! :warning: Subject to changes! :warning:

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

TBD

<!---
The history of the framework is still somewhat visible in its structure, for example a very Visual Studio centric organization, that the separation into assemblies is based on dependencies and not functionality, or the obvious fact that it is one big project containing a lot of actually unrelated functions (instead of several smaller, independent projects).
-->
