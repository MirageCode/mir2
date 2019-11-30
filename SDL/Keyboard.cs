using System;
using System.Runtime.InteropServices;

namespace SDL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Keysym
    {
        public ScanCode scancode;
        public KeyCode sym;
        public KeyMod mod; /* UInt16 */
        public UInt32 unicode; /* Deprecated */
    }

}
