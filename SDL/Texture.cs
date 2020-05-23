using System;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace SDL
{
    public enum TextureAccess
    {
        Static,
        Streaming,
        Target,
    }

    [Flags]
    public enum TextureModulate
    {
        None = 0x00000000,
        Horizontal = 0x00000001,
        Vertical = 0x00000002,
    }

    public class Texture : Resource<SDLException>
    {
        public Texture(Renderer renderer, PixelFormat format,
                       TextureAccess access, int w, int h)
        : base(SDL_CreateTexture(renderer.handle, format, access, w, h)) {}

        public Texture(Renderer renderer, Surface surface)
        : base(SDL_CreateTextureFromSurface(renderer.handle, surface.handle)) {}

        public Texture (IntPtr texture, bool managed = true, bool safe = true)
        : base(texture, managed, safe) {}

        public static Texture FromRaw(
            Renderer renderer, byte[] data, int width, int height)
        {
            using (var surface = new Surface(data, width, height))
                return new Texture(renderer, surface);
        }

        public static Texture StreamFromRaw(
            Renderer renderer, byte[] data, int width, int height)
        {
            var texture = new Texture(
                renderer, PixelFormat.ARGB8888,
                TextureAccess.Streaming, width, height);

            texture.WithLock((IntPtr pixels, int pitch) => {
                    Marshal.Copy(data, 0, pixels, data.Length);
                });

            return texture;
        }

        protected override void Free(IntPtr handle) =>
            SDL_DestroyTexture(handle);

        public byte Alpha
        {
            get {
                byte a;
                EnsureSafe(SDL_GetTextureAlphaMod(handle, out a));
                return a;
            }

            set {
                EnsureSafe(SDL_SetTextureAlphaMod(handle, value));
            }
        }

        public float Opacity
        {
            get {
                float alpha = Alpha;
                return Math.Max(Math.Min(alpha / 256, 1.0F), 0F);
            }

            set {
                byte alpha = (byte) Math.Floor(value * 255);
                Alpha = Math.Max(Math.Min(alpha, (byte) 255), (byte) 0);
            }
        }

        // TODO: These values are actually BGRA. Figure out if it's
        // dependent on LittleEndian.

        public Color Color
        {
            get {
                byte[] argb = new byte[4];

                EnsureSafe(SDL_GetTextureAlphaMod(handle, out argb[3]));
                EnsureSafe(SDL_GetTextureColorMod(
                               handle, out argb[2], out argb[1], out argb[0]));

                return Color.FromArgb(BitConverter.ToInt32(argb, 0));
            }

            set {
                byte[] argb = BitConverter.GetBytes(value.ToArgb());

                EnsureSafe(SDL_SetTextureAlphaMod(handle, argb[3]));
                EnsureSafe(SDL_SetTextureColorMod(
                               handle, argb[2], argb[1], argb[0]));
            }
        }

        public BlendMode BlendMode
        {
            get {
                BlendMode blendMode;
                EnsureSafe(SDL_GetTextureBlendMode(handle, out blendMode));
                return blendMode;
            }

            set {
                EnsureSafe(SDL_SetTextureBlendMode(handle, value));
            }
        }

        public Size Size
        {
            get {
                uint format;
                int access, w, h;
                EnsureSafe(SDL_QueryTexture(
                               handle, out format, out access, out w, out h));
                return new Size(w, h);
            }
        }

        private void Lock(out IntPtr pixels, out int pitch) => EnsureSafe(
            SDL_LockTexture(handle, IntPtr.Zero, out pixels, out pitch));

        private void Unlock() => SDL_UnlockTexture(handle);

        public delegate void LockCallback(IntPtr pixels, int pitch);

        public void WithLock(LockCallback callback)
        {
            IntPtr pixels;
            int pitch;
            Lock(out pixels, out pitch);
            callback(pixels, pitch);
            Unlock();
        }

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTexture(
            IntPtr renderer, PixelFormat format, TextureAccess access,
            int w, int h);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyTexture(IntPtr texture);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(
            IntPtr renderer, IntPtr surface);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetTextureAlphaMod(
            IntPtr texture, out byte alpha);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetTextureAlphaMod(
            IntPtr texture, byte alpha);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetTextureColorMod(
            IntPtr texture, out byte r, out byte g, out byte b);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetTextureColorMod(
            IntPtr texture, byte r, byte g, byte b);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetTextureBlendMode(
            IntPtr texture, out BlendMode blendMode);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetTextureBlendMode(
            IntPtr texture, BlendMode blendMode);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_QueryTexture(
            IntPtr texture, out uint format, out int access,
            out int w, out int h);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_LockTexture(
            IntPtr texture, IntPtr rect, out IntPtr pixels, out int pitch);

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_UnlockTexture(IntPtr texture);
    }
}
