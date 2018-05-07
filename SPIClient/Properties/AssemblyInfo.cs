﻿using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SPIClient")]
[assembly: AssemblyDescription("Client for Assembly Payments Instore Simple Payments Integration (SPI) API")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Assembly Payments")]
[assembly: AssemblyProduct("Instore")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3BB60A49-EBA4-4099-8756-95A654A9329E")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.1.5.*")]
[assembly: AssemblyInformationalVersion("2.1.4")]


// Let log4net know that it can look for configuration in the default application config file
[assembly: log4net.Config.Repository("SPIClient")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "SPIClient.dll.config", Watch = true)]