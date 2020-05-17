using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDL
{
    [Flags]
    public enum WindowFlags : uint
    {
        None = 0,
        Fullscreen = 0x00000001,
        OpenGL = 0x00000002,
        Shown = 0x00000004,
        Hidden = 0x00000008,
        Borderless = 0x00000010,
        Resizable = 0x00000020,
        Minimized = 0x00000040,
        Maximized = 0x00000080,
        InputGrabbed = 0x00000100,
        InputFocus = 0x00000200,
        MouseFocus = 0x00000400,
        FullscreenDesktop = (Fullscreen | 0x00001000),
        Foreign = 0x00000800,
        AllowHighDPI = 0x00002000, /* Only available in 2.0.1 */
        MouseCapture = 0x00004000, /* Only available in 2.0.4 */
        AlwaysOnTop = 0x00008000, /* Only available in 2.0.5 */
        SkipTaskbar = 0x00010000, /* Only available in 2.0.5 */
        Utility = 0x00020000, /* Only available in 2.0.5 */
        Tooltip = 0x00040000, /* Only available in 2.0.5 */
        PopupMenu = 0x00080000, /* Only available in 2.0.5 */
        Vulkan = 0x10000000, /* Only available in 2.0.6 */
    }

    public class Window : Resource<SDLException>
    {
        private const int WindowPositionUndefined = 0x1FFF0000;
        private const int WindowPositionCentered = 0x2FFF0000;

        public Window(string title, int x, int u, int w, int h,
                      WindowFlags flags = WindowFlags.None)
        : base(SDL_CreateWindow(Util.FromString(title), x, u, w, h, flags)) {}

        public Window(string title, int w, int h,
                      WindowFlags flags = WindowFlags.None)
        : base(SDL_CreateWindow(
                   Util.FromString(title), WindowPositionCentered,
                   WindowPositionCentered, w, h, flags)) {}

        protected override void Free(IntPtr handle) =>
            SDL_DestroyWindow(handle);

        public Renderer CreateRenderer(
            int index = -1, RendererFlags flags = RendererFlags.None) =>
            new Renderer(this, index, flags);

        public Renderer Renderer
        {
            get => new Renderer(SDL_GetRenderer(handle), false);
        }

        public bool Fullscreen
        {
            get => SDL_GetWindowFlags(handle).HasFlag(WindowFlags.Fullscreen);
            set => EnsureSafe(SDL_SetWindowFullscreen(
                handle, value ? WindowFlags.Fullscreen : WindowFlags.None));
        }

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow(
            byte[] title, int x, int y, int w, int h, WindowFlags flags);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyWindow(IntPtr window);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern WindowFlags SDL_GetWindowFlags(IntPtr window);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetRenderer(IntPtr window);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowFullscreen(
            IntPtr window, WindowFlags flags);
    }
}
