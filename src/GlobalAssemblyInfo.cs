using System.Reflection;

[assembly: AssemblyProduct("Hot Spot")]
[assembly: AssemblyCompany("Hot Spot Contributors")]
[assembly: AssemblyCopyright("Copyright © 2015-2016")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
