using System.Reflection;
using System.Runtime.InteropServices;





// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3BB60A49-EBA4-4099-8756-95A654A9329E")]



// Let log4net know that it can look for configuration in the default application config file
[assembly: log4net.Config.Repository("SPIClient")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "SPIClient.dll.config", Watch = true)]