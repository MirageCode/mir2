using System;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SDL
{
    [Flags]
    public enum RendererFlags : uint
    {
        None = 0,
        Software = 0x00000001,
        Accelerated = 0x00000002,
        PresentVSync = 0x00000004,
        TargetTexture = 0x00000008,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rect(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }
    }

    public class Renderer : Resource<SDLException>
    {
        public Renderer(Window window, int index, RendererFlags flags)
        : base(SDL_CreateRenderer(window.handle, index, flags)) {}

        protected override void Free(IntPtr handle) =>
            SDL_DestroyRenderer(handle);

        public void RenderPresent()
        {
            SDL_RenderPresent(handle);
        }

        public void RenderClear()
        {
            EnsureSafe(SDL_RenderClear(handle));
        }

        public void RenderClear(Color color)
        {
            var oldColor = Color;
            Color = color;
            RenderClear();
            Color = oldColor;
        }

        public void RenderCopy(Texture texture, Rectangle src, Rectangle dst)
        {
            IntPtr srcrect = IntPtr.Zero;
            IntPtr dstrect = IntPtr.Zero;

            if (src != null && src != Rectangle.Empty)
            {
                srcrect = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rect)));
                Marshal.StructureToPtr(new Rect(src), srcrect, false);
            }

            if (dst != null && dst != Rectangle.Empty)
            {
                dstrect = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rect)));
                Marshal.StructureToPtr(new Rect(dst), dstrect, false);
            }

            try {
                EnsureSafe(SDL_RenderCopy(
                               handle, texture.handle, srcrect, dstrect));
            }
            finally {
                if (srcrect != IntPtr.Zero) Marshal.FreeHGlobal(srcrect);
                if (dstrect != IntPtr.Zero) Marshal.FreeHGlobal(dstrect);
            }
        }

        public void RenderDrawRect(Rectangle rectangle)
        {
            IntPtr rect = IntPtr.Zero;

            if (rectangle != null && rectangle != Rectangle.Empty)
            {
                rect = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rect)));
                Marshal.StructureToPtr(new Rect(rectangle), rect, false);
            }

            try {
                EnsureSafe(SDL_RenderDrawRect(handle, rect));
            }
            finally {
                if (rect != IntPtr.Zero) Marshal.FreeHGlobal(rect);
            }
        }

        public void RenderFillRect(Rectangle rectangle)
        {
            IntPtr rect = IntPtr.Zero;

            if (rectangle != null && rectangle != Rectangle.Empty)
            {
                rect = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rect)));
                Marshal.StructureToPtr(new Rect(rectangle), rect, false);
            }

            try {
                EnsureSafe(SDL_RenderFillRect(handle, rect));
            }
            finally {
                if (rect != IntPtr.Zero) Marshal.FreeHGlobal(rect);
            }
        }

        public void RenderDrawLine(int x1, int y1, int x2, int y2)
        {
            EnsureSafe(SDL_RenderDrawLine(handle, x1, y1, x2, y2));
        }

        public void RenderDrawLine(Point start, Point end)
        {
            RenderDrawLine(start.X, start.Y, end.X, end.Y);
        }

        public Color Color
        {
            get {
                byte r, g, b, a;
                EnsureSafe(SDL_GetRenderDrawColor(
                               handle, out r, out g, out b, out a));
                return Color.FromArgb(a, r, g, b);
            }

            set {
                InternalColor color = new InternalColor(value);
                EnsureSafe(SDL_SetRenderDrawColor(
                               handle, color.R, color.G, color.B, color.A));
            }
        }

        public bool RenderTargetSupported
        {
            get => SDL_RenderTargetSupported(handle) == Bool.True
                ? true : false;
        }

        public Texture RenderTarget
        {
            get => new Texture(SDL_GetRenderTarget(handle), false, false);
            set => EnsureSafe(SDL_SetRenderTarget(handle, value.handle));
        }

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_CreateRenderer(
			IntPtr window, int index, RendererFlags flags);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_DestroyRenderer(IntPtr renderer);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_RenderPresent(IntPtr renderer);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_RenderClear(IntPtr renderer);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_RenderCopy(
			IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_RenderDrawRect(
            IntPtr renderer, IntPtr rect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_RenderFillRect(
			IntPtr renderer, IntPtr rect);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GetRenderDrawColor(
			IntPtr renderer, out byte r, out byte g, out byte b, out byte a);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_SetRenderDrawColor(
			IntPtr renderer, byte r, byte g, byte b, byte a);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern Bool SDL_RenderTargetSupported(IntPtr renderer);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_GetRenderTarget(IntPtr renderer);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_SetRenderTarget(
            IntPtr renderer, IntPtr texture);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_RenderDrawLine(
			IntPtr renderer, int x1, int y1, int x2, int y2);
    }
}
