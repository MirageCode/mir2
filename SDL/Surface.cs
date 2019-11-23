using System;
using System.Runtime.InteropServices;
using Rectangle = System.Drawing.Rectangle;

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

        public Rectangle Clip
        {
            get
            {
                Rect rect;
                SDL_GetClipRect(handle, out rect);
                return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            }

            set
            {
                if (value == Rectangle.Empty)
                {
                    SDL_SetClipRect(handle, IntPtr.Zero);
                }
                else
                {
                    Rect rect = new Rect(value);
                    SDL_SetClipRect(handle, ref rect);
                }
            }
        }

        public void BlitFrom(
            Surface src, Rectangle srcRectangle, Rectangle dstRectangle)
        {
            if (srcRectangle == Rectangle.Empty && dstRectangle == Rectangle.Empty)
            {
                EnsureSafe(SDL_BlitSurface(
                    src.handle, IntPtr.Zero, handle, IntPtr.Zero));
            }
            else if (srcRectangle == Rectangle.Empty)
            {
                Rect dstrect = new Rect(dstRectangle);
                EnsureSafe(SDL_BlitSurface(
                    src.handle, IntPtr.Zero, handle, ref dstrect));
            }
            else if (dstRectangle == Rectangle.Empty)
            {
                Rect srcrect = new Rect(srcRectangle);
                EnsureSafe(SDL_BlitSurface(
                    src.handle, ref srcrect, handle, IntPtr.Zero));
            }
            else
            {
                Rect srcrect = new Rect(srcRectangle);
                Rect dstrect = new Rect(dstRectangle);
                EnsureSafe(SDL_BlitSurface(
                    src.handle, ref srcrect, handle, ref dstrect));
            }
        }

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

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GetClipRect(
			IntPtr surface, out Rect rect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern Bool SDL_SetClipRect(
			IntPtr surface, ref Rect rect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern Bool SDL_SetClipRect(
			IntPtr surface, IntPtr rect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "SDL_UpperBlit")]
		private static extern int SDL_BlitSurface(
			IntPtr src, ref Rect srcrect, IntPtr dst, ref Rect dstrect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "SDL_UpperBlit")]
		private static extern int SDL_BlitSurface(
			IntPtr src, IntPtr srcrect, IntPtr dst, ref Rect dstrect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "SDL_UpperBlit")]
		private static extern int SDL_BlitSurface(
			IntPtr src, ref Rect srcrect, IntPtr dst, IntPtr dstrect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "SDL_UpperBlit")]
		private static extern int SDL_BlitSurface(
			IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect);
    }
}
