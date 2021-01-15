# Contribution

Please read the [overview](README.md) to get started and to get additional information.

## General

At the moment, [issues](https://github.com/RotenInformatik/RI_Framework/issues) are the preferred way for contributions.

You are of course welcome to submit [pull requests](https://github.com/RotenInformatik/RI_Framework/pulls) from your own fork as well, but due to scarce resources (that is: time), processing pull requests can take a relatively long time.

## Issues

An issue must follow the following rules, otherwise it will be closed immediately without further consideration:

 * Everything has to be in English language.
 * Issues which represent bugs have a higher chance of being accepted if they contain a description about how to reproduce them.

## Pull requests

A pull request must follow the following rules, otherwise it will be closed immediately without further consideration:

 * Everything has to be in English language.
 * The existing code style must be followed.
 * A unit test must be written (if not yet existing) or modified (if existing) for each created or modified type and/or member.
 * The following is only accepted if there is a corresponding open issue:
   * Structural changes (deleting, creating, renaming, moving, copying of files or directories).
   * Namespace changes.
   * Dependency changes (removing, adding, changing of assembly references or NuGet packages).
   * Change of the name of a namespace, public type, or public member.
 * The following must always have proper XML comments (also check the generated SHFB documentation for completeness):
   * Namespaces
   * Public types
   * Public members
