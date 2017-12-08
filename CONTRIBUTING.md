# Contribution

## General

At the moment, [issues](https://github.com/RotenInformatik/RI_Framework/issues) are the preferred way for contributions.

You are of course welcome to submit pull request from your own fork as well, but due to scrace resources (that is: time), processing pull requests can take a relatively long time.

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
 * Dependency changes (removing, adding, changing or references or NuGet packages) are only accepted if there is a corresponding open issue.
 * A unit test must be written (if not yet existing) or modified (if existing) for each created or modified type and/or member.
 * The following must always have proper XML comments (also check the generated SHFB documentation for completeness):
   * Namespaces
   * Public types
   * Public members

## Required tools

In order to successfully build your own fork, you need the following tools (see also [Documentation](DOCUMENTATION.md)):

 * [Visual Studio](https://www.visualstudio.com/) 2015 or newer
 * [Visual Studio Tools for Unity](https://docs.microsoft.com/en-us/visualstudio/cross-platform/visual-studio-tools-for-unity)
 * [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB) 2016.9.17 or newer
