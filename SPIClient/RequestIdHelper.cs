using System.Runtime.InteropServices;

﻿namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("42D2C9B0-0E8F-4CF5-AD0D-1FADCF0AB127")]
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