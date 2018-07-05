using System.Runtime.InteropServices;

﻿namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("585C0CFB-0124-47EE-B566-A9DF211BFA32")]
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