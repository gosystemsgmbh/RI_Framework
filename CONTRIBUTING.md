# Contribution

## General

At the moment, [issues](https://github.com/RotenInformatik/RI_Framework/issues) are the preferred way for contributions.

You are of course welcome to submit pull request from your own fork as well, but due to scarce resources (that is: time), processing pull requests can take a relatively long time.

## Issues

An issue must follow the following rules, otherwise it will be closed immediately without further consideration.

 * Everything has to be in proper English language.
 * Issues which represent bugs have a higher chance of being accepted if they contain a description about how to reproduce them.

## Pull requests

A pull request must follow the following rules, otherwise it will be closed immediately without further consideration.

 * Everything has to be in proper English language.
 * The existing code style must be followed.
 * Structural changes (deleting, creating, renaming, moving, copying of files or directories) are only accepted if there is a corresponding open issue.
 * Namespace changes are only accepted if there is a corresponding open issue.
 * Dependency changes (removing, adding, changing of assembly references or NuGet packages) are only accepted if there is a corresponding open issue.
 * A unit test must be written (if not yet existing) or modified (if existing) for each created or modified type and/or member.
 * The following must always have proper XML comments (also check the generated SHFB documentation for completeness):
   * Namespaces
   * Public types
   * Public members

## Required tools

The following tools are required in order to successfully build the project:

 * [Visual Studio](https://www.visualstudio.com/) 2017 15.5.2 or newer
 * [Unity](https://unity3d.com/) 5.4.1f1 or newer
 * [Visual Studio Tools for Unity](https://docs.microsoft.com/en-us/visualstudio/cross-platform/visual-studio-tools-for-unity) 3.5.0.2 or newer
 * [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB) 2017.5.15.0 or newer

## Recommended tools

The following tools are recommended but not necessary to build the project:

 * [ReSharper](https://www.jetbrains.com/resharper/)
