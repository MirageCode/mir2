using System;
using System.Runtime.InteropServices;

namespace SDL
{
    [Flags]
    public enum SubSystem : uint
    {
        Timer = 0x00000001,
		Audio = 0x00000010,
		Video = 0x00000020,
		Joystick = 0x00000200,
		Haptic = 0x00001000,
		Gamecontroller = 0x00002000,
		Events = 0x00004000,
		Sensor = 0x00008000,
		Noparachute = 0x00100000,
    }

    public static class SDLContext
    {
		public const string SDLLib = "libSDL2-2.0.so.0";
		public const string TTFLib = "libSDL2_ttf-2.0.so.0";

        public static void Init(SubSystem flags)
        {
            if (SDL_Init(flags) != 0) throw new SDLException();
        }

        public static void InitSubSystem(SubSystem flags)
        {
            if (SDL_InitSubSystem(flags) != 0) throw new SDLException();
        }

        public static void Quit() => SDL_Quit();

        public static void QuitSubSystem(SubSystem flags) =>
            SDL_QuitSubSystem(flags);

        public static void InitTTF()
        {
            if (TTF_Init() != 0) throw new TTFException();
        }

        public static void QuitTTF() => TTF_Quit();

        public static void Delay(UInt32 ms) => SDL_Delay(ms);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_Init(SubSystem flags);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_InitSubSystem(SubSystem flags);

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Quit();

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_QuitSubSystem(SubSystem flags);

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int TTF_Init();

		[DllImport(TTFLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void TTF_Quit();

		[DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_Delay(UInt32 ms);
    }
}
