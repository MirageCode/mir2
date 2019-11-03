using System;
using System.Runtime.InteropServices;

namespace SDL
{
    public class RWops : Resource<SDLException>
    {
        public RWops(byte[] mem, int size)
        : base(SDL_RWFromMem(ref mem, size)) {}

        protected override void Free(IntPtr handle) => SDL_RWclose(handle);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_RWFromMem(ref byte[] mem, int size);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern long SDL_RWclose(IntPtr context);
    }
}
