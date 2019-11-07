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
        // FIXME: Don't do this here
        public static Window Window = new Window("Test", 512, 512, 0);
        public static Renderer Renderer = Window.CreateRenderer(-1, 0);

        public static List<MImage> TextureList = new List<MImage>();
        public static List<MirControl> ControlList = new List<MirControl>();

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

        public static Texture CreateTexture(byte[] data, int width, int height)
        => Texture.FromRaw(Renderer, data, width, height);

        public static Texture CreateTexture(string s, Font font, Color color, uint width)
        {
            using (var surface = font.CreateSurface(s, color, width))
                return new Texture(Renderer, surface);
        }

        public static void Draw2D(
            Texture texture, Point point, float opacity = 1.0F)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, Rectangle.Empty, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Point point, Color color, float opacity = 1.0F)
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
            Texture texture, Rectangle section, Point point,
            float opacity = 1.0F)
        {
            var oldOpacity = texture.Opacity;
            texture.Opacity = opacity;

            Renderer.RenderCopy(
                texture, section, new Rectangle(point, texture.Size));

            texture.Opacity = oldOpacity;
        }

        public static void Draw2D(
            Texture texture, Rectangle section, Point point,
            Color color, float opacity = 1.0F)
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
    }
}
