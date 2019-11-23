using System;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace SDL
{
    public class Font : Resource<TTFException>
    {
        public Font(String file, int ptsize)
        : base(TTF_OpenFont(Util.FromString(file), ptsize)) {}

        protected override void Free(IntPtr handle) => TTF_CloseFont(handle);

        public Size GetSize(string s)
        {
            int w, h;
            EnsureSafe(TTF_SizeUTF8(handle, Util.FromString(s), out w, out h));
            return new Size(w, h);
        }

        public int Height { get => TTF_FontHeight(handle); }
        public int Ascent { get => TTF_FontAscent(handle); }
        public int Descent { get => TTF_FontDescent(handle); }
        public int LineSkip { get => TTF_FontLineSkip(handle); }

        public Surface CreateSurface(string s, Color color, uint width) =>
            new Surface(TTF_RenderUTF8_Blended_Wrapped(
                handle, Util.FromString(s.Length == 0 ? " " : s),
                new InternalColor(color), width));

        public Surface CreateSurface(string s, Color color) =>
            new Surface(TTF_RenderUTF8_Solid(
                handle, Util.FromString(s.Length == 0 ? " " : s),
                new InternalColor(color)));

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr TTF_OpenFont(byte[] file, int ptsize);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void TTF_CloseFont(IntPtr font);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SizeUTF8(
			IntPtr font, byte[] text, out int w, out int h);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr TTF_RenderUTF8_Blended_Wrapped(
			IntPtr font, byte[] text, InternalColor fg, uint wrapped);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr TTF_RenderUTF8_Solid(
			IntPtr font, byte[] text, InternalColor fg);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TTF_FontHeight(IntPtr font);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TTF_FontAscent(IntPtr font);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TTF_FontDescent(IntPtr font);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TTF_FontLineSkip(IntPtr font);
    }
}
