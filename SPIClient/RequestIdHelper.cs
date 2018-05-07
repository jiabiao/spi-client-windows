using System.Runtime.InteropServices;

﻿namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("E0DB64C4-EF81-4541-BDEC-53754B966FB4")]
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