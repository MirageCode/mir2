using System;
using System.Runtime.InteropServices;

namespace SDL
{
    public class Surface : Resource<SDLException>
    {
        private static readonly uint Bmask = Convert.ToUInt32(
            BitConverter.IsLittleEndian ? 0x000000ff : 0xff000000);
        private static readonly uint Gmask = Convert.ToUInt32(
            BitConverter.IsLittleEndian ? 0x0000ff00 : 0x00ff0000);
        private static readonly uint Rmask = Convert.ToUInt32(
            BitConverter.IsLittleEndian ? 0x00ff0000 : 0x0000ff00);
        private static readonly uint Amask = Convert.ToUInt32(
            BitConverter.IsLittleEndian ? 0xff000000 : 0x000000ff);

        private const int Depth = 32;

        public Surface(int width, int height)
        : base(SDL_CreateRGBSurface(
                   0, width, height, Depth, Rmask, Gmask, Bmask, Amask)) {}

        public Surface(byte[] data, int width, int height)
        : base(SDL_CreateRGBSurfaceFrom(
                   data, width, height, Depth, width * 4, Rmask, Gmask, Bmask, Amask)) {}

        public Surface (IntPtr surface) : base(surface) {}

        protected override void Free(IntPtr handle) => SDL_FreeSurface(handle);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_CreateRGBSurface(
			uint flags, int width, int height, int depth,
            uint Rmask, uint Gmask, uint Bmask, uint Amask);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_CreateRGBSurfaceFrom(
            byte[] pixels, int width, int height, int depth, int pitch,
            uint Rmask, uint Gmask, uint Bmask, uint Amask);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_FreeSurface(IntPtr surface);
    }
}
