﻿using System;
using System.Reflection;




[assembly: AssemblyProduct("Decoupling & Utilities Framework")]
[assembly: AssemblyCompany("Roten Informatik")]
[assembly: AssemblyCopyright("Copyright (c) 2011-2018 Roten Informatik")]
[assembly: AssemblyTrademark("Licensed under the Roten Informatik Framework License 1.0")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
