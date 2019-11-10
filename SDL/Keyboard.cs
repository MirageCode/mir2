using System;
using System.Runtime.InteropServices;

namespace SDL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Keysym
    {
        public Scancode scancode;
        public Keycode sym;
        public Keymod mod; /* UInt16 */
        public UInt32 unicode; /* Deprecated */
    }

}
