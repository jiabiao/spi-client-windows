using System.Runtime.InteropServices;

﻿namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("7EF49ABD-3938-418E-ABC9-2FAD7EAD69D9")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public static class RequestIdHelper
    {
        private static int _counter = 1;

        public static string Id(string prefix)
        {
            return prefix + _counter++;
        }

    }
}