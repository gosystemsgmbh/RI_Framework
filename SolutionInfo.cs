using System;
using System.Reflection;




[assembly: AssemblyProduct("Decoupling & Utilities Framework")]
[assembly: AssemblyCompany("Roten Informatik")]
[assembly: AssemblyCopyright("Copyright (c) 2011-2019 Roten Informatik")]
[assembly: AssemblyTrademark("Licensed under the Apache License 2.0")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyFileVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
