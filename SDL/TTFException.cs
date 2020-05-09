using System;
using System.Runtime.InteropServices;

namespace SDL
{
    public class TTFException : Exception
    {
        public const string TTFLib = Resource<TTFException>.TTFLib;

        public TTFException () : base(Util.ToString(TTF_GetError())) {}

        [DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_GetError();
    }
}
