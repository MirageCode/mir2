using System;
using System.Collections.Generic;
using SDL;
using Client.MirControls;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.MirGraphics
{
    static class SDLManager
    {
        public static Window Window;
        public static Renderer Renderer;

        public static void Create()
        {
            SDLContext.Init(SubSystem.Video);
            SDLContext.InitTTF();

            Window = new Window("Mir", Settings.ScreenWidth, Settings.ScreenHeight, 0);
            Renderer = Window.CreateRenderer(-1, 0);
        }

        public static void Destroy()
        {
            Clean();

            Renderer.Dispose();
            Window.Dispose();

            SDLContext.QuitTTF();
            SDLContext.Quit();
        }

        public static float Opacity = 1.0F;
        // TODO: Blending
        public static bool Blending
        {
            get => false;
        }

        public static List<MImage> TextureList = new List<MImage>();
        public static List<MirControl> ControlList = new List<MirControl>();
        public static List<Texture> Lights = new List<Texture>();

        public static Texture RadarTexture;
        public static Texture PoisonDotBackground;

        public static bool GrayScale = false;

        public static Point[] LightSizes =
        {
            new Point(125,95),
            new Point(205,156),
            new Point(285,217),
            new Point(365,277),
            new Point(445,338),
            new Point(525,399),
            new Point(605,460),
            new Point(685,521),
            new Point(765,581),
            new Point(845,642),
            new Point(925,703)
        };

        // TODO: CreateLights

        private static void LoadTextures()
        {
            if (RadarTexture == null || RadarTexture.Disposed)
            {
                RadarTexture = CreateTexture(2, 2);
                Clear(RadarTexture, Color.White);
            }
            if (PoisonDotBackground == null || PoisonDotBackground.Disposed)
            {
                PoisonDotBackground = CreateTexture(5, 5);
                Clear(PoisonDotBackground, Color.White);
            }
        }

        public static void Clean()
        {
            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                MImage m = TextureList[i];

                if (m == null)
                {
                    TextureList.RemoveAt(i);
                    continue;
                }

                if (CMain.Time <= m.CleanTime) continue;


                TextureList.RemoveAt(i);
                if (m.Image != null && !m.Image.Disposed)
                    m.Image.Dispose();
            }

            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                MirControl c = ControlList[i];

                if (c == null)
                {
                    ControlList.RemoveAt(i);
                    continue;
                }

                if (CMain.Time <= c.CleanTime) continue;

                c.DisposeTexture();
            }
        }

        private static void CleanUp()
        {
            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                MImage m = TextureList[i];

                if (m == null) continue;

                if (m.Image != null && !m.Image.Disposed)
                    m.Image.Dispose();
            }
            TextureList.Clear();


            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                MirControl c = ControlList[i];

                if (c == null) continue;

                c.DisposeTexture();
            }
            ControlList.Clear();
        }

        public static Texture CreateTexture(
            int width, int height,
            TextureAccess access = TextureAccess.Target) =>
            new Texture(Renderer, PixelFormat.ARGB8888, access, width, height);

        public static Texture CreateTexture(byte[] data, int width, int height)
        => Texture.FromRaw(Renderer, data, width, height);

        public static Texture CreateTexture(string s, Font font, Color color, uint width)
        {
            using (var surface = font.CreateSurface(s, color, width))
                return new Texture(Renderer, surface);
        }

        public static void Clear(Texture texture)
        {
            var oldTexture = Renderer.RenderTarget;
            Renderer.RenderTarget = texture;
            Renderer.RenderClear();
            Renderer.RenderTarget = oldTexture;
        }

        public static void Clear(Texture texture, Color color)
        {
            var oldTexture = Renderer.RenderTarget;
            Renderer.RenderTarget = texture;
            Renderer.RenderClear(color);
            Renderer.RenderTarget = oldTexture;
        }

        public static void DrawToTexture(
            Texture texture, Color background, Action action)
        {
            var oldTexture = Renderer.RenderTarget;
            Renderer.RenderTarget = texture;
            Renderer.RenderClear(background);
            action();
            Renderer.RenderTarget = oldTexture;
        }

        public static void Draw2D(
            Texture texture, Point point, float opacity)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, Rectangle.Empty, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Point point, Color color, float opacity)
        {
            var oldColor = texture.Color;
            var oldOpacity = texture.Opacity;
            texture.Color = color;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, Rectangle.Empty, new Rectangle(point, texture.Size));

            texture.Color = oldColor;
            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Rectangle section, Point point, float opacity)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, section, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Rectangle section, Point point, Color color,
            float opacity)
        {
            var oldColor = texture.Color;
            var oldOpacity = texture.Opacity;
            texture.Color = color;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, section, new Rectangle(point, texture.Size));

            texture.Color = oldColor;
            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Point point)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = Opacity;

            Renderer.RenderCopy(
                texture, Rectangle.Empty, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Point point, Color color)
        {
            var oldColor = texture.Color;
            var oldOpacity = texture.Opacity;
            texture.Color = color;
            texture.Opacity = Opacity;

            Renderer.RenderCopy(
                texture, Rectangle.Empty, new Rectangle(point, texture.Size));

            texture.Color = oldColor;
            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Rectangle section, Point point)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = Opacity;

            Renderer.RenderCopy(
                texture, section, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Rectangle section, Point point, Color color)
        {
            var oldColor = texture.Color;
            var oldOpacity = texture.Opacity;
            texture.Color = color;
            texture.Opacity = Opacity;

            Renderer.RenderCopy(
                texture, section, new Rectangle(point, texture.Size));

            texture.Color = oldColor;
            texture.Opacity = oldOpacity;
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            var oldColor = Renderer.Color;
            Renderer.Color = color;

            Renderer.RenderDrawRect(rectangle);

            Renderer.Color = oldColor;
        }

        public static void DrawText(
            string s, Font font, Rectangle dst, Color color)
        {
            using (var surface = font.CreateSurface(s, color, (uint) dst.Width))
                using (var texture = new Texture(Renderer, surface))
                    Draw2D(texture, new Point(dst.X, dst.Y));
        }
    }
}
