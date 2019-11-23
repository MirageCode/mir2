using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using Rectangle = System.Drawing.Rectangle;

namespace SDL
{
    public class TextBox : IDisposable
    {
        public bool Disposed { get; private set; } = false;
        public event EventHandler Disposing;

        public event EventHandler Updated;

        private Font _Font;
        public Font Font
        {
            get => _Font;
            set
            {
                _Font = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        private Color _ForeColor = Color.Black;
        public Color ForeColor
        {
            get => _ForeColor;
            set
            {
                _ForeColor = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        private Color _BackColor = Color.White;
        public Color BackColor
        {
            get => _BackColor;
            set
            {
                _BackColor = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        private Color _BorderColor = Color.Empty;
        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                _BorderColor = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        private string _Text = string.Empty;
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        // TODO: Support for multiple lines
        public string[] Lines
        {
            get => new [] { Text };
            set => Text = value.Length > 0 ? value[0] : string.Empty;
        }

        public bool Multiline { get => false; set {} }

        private Size _Size;
        public Size Size
        {
            get => _Size;
            set
            {
                _Size = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        public int MaxLength { get; set; } = 255;

        public char PasswordChar { get; set; } = '*';
        public bool UsePasswordChar { get; set; } = false;

        private bool _Focused = false;
        public bool Focused
        {
            get => _Focused;
            set
            {
                if (_Focused == value) return;
                if (value) {
                    Event.OnTextInput += TextInput;
                }
                else {
                    Event.OnTextInput -= TextInput;
                }
                _Focused = value;
            }
        }

        public void TextInput(TextInputEvent e)
        {
            if (Text.Length < MaxLength) Text += e.Text;
        }

        public Texture CreateTexture(Renderer renderer)
        {
            var text = UsePasswordChar
                ? new String(PasswordChar, Text.Length) : Text;

            using (var surface = Font.CreateSurface(text, ForeColor))
                using (var fore = new Texture(renderer, surface))
                {
                    var size = fore.Size;
                    var rectangle = new Rectangle(
                        0, 0,
                        Size.Width > size.Width ? size.Width : Size.Width,
                        Size.Height > size.Height ? size.Height : Size.Height);

                    var back = new Texture(
                        renderer, PixelFormat.ARGB8888, TextureAccess.Target,
                        Size.Width, Size.Height);

                    renderer.WithRenderTarget(back, () => {
                        renderer.RenderClear(BackColor);
                        renderer.RenderCopy(fore, rectangle, rectangle);

                        if (BorderColor != Color.Empty)
                        {
                            var oldColor = renderer.Color;
                            renderer.Color = BorderColor;
                            renderer.RenderDrawRect(
                                new Rectangle(0, 0, Size.Width, Size.Height));
                            renderer.Color = oldColor;
                        }
                    });

                    return back;
                };
        }

        public TextBox(Font font, Size size)
        {
            Font = font;
            Size = size;
        }

        ~TextBox() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing) Disposing?.Invoke(this, new EventArgs());

            Disposed = true;
        }
    }
}
