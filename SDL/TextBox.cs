using System;
using System.Linq;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;

namespace SDL
{
    public class TextBox : IDisposable
    {
        public bool Disposed { get; private set; } = false;
        public event EventHandler Disposing;

        public event EventHandler Updated;
        public event EventHandler TextChanged, GotFocus, LostFocus;

        public delegate void KeyPressHandler(TextBox sender, KeyboardEvent e);
        public event KeyPressHandler OnKeyPress;

        private Point _Cursor;
        public Point Cursor
        {
            get => GetSafeCursor(_Cursor, Lines);
            set
            {
                _Cursor = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

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

        public string Text
        {
            get => string.Join("\n", Lines);
            set => Lines = value.Split('\n').ToList();
        }

        private List<string> _Lines = new List<string>();
        public List<string> Lines
        {
            get => _Lines;
            set
            {
                _Lines = value;
                TextChanged?.Invoke(this, new EventArgs());
                Updated?.Invoke(this, new EventArgs());
            }
        }

        public bool Multiline { get; set; } = false;

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
                    EnableEvents();
                    GotFocus?.Invoke(this, new EventArgs());
                }
                else {
                    DisableEvents();
                    LostFocus?.Invoke(this, new EventArgs());
                }
                _Focused = value;
                Updated?.Invoke(this, new EventArgs());
            }
        }

        private void EnableEvents()
        {
            Event.OnTextInput += TextInput;
            Event.OnKeyDown += KeyDown;
        }

        private void DisableEvents()
        {
            Event.OnTextInput -= TextInput;
            Event.OnKeyDown -= KeyDown;
        }

        public void TextInput(TextInputEvent e)
        {
            if (Text.Length >= MaxLength) return;

            var cursor = Cursor;
            var line = Lines[cursor.Y];
            _Lines[cursor.Y] = line.Substring(0, cursor.X)
                + e.Text + line.Substring(cursor.X);

            _Cursor = new Point(cursor.X + e.Text.Length, cursor.Y);

            TextChanged?.Invoke(this, new EventArgs());
            Updated?.Invoke(this, new EventArgs());
        }

        private void HandleBackspace()
        {
            var cursor = Cursor;
            if (cursor.X == 0 && cursor.Y == 0) return;

            if (cursor.X == 0) {
                var x = Lines[cursor.Y - 1].Length;
                _Lines[cursor.Y - 1] += Lines[cursor.Y];
                _Lines.RemoveAt(cursor.Y);
                _Cursor = new Point(x, cursor.Y - 1);
            }
            else {
                _Lines[cursor.Y] = Lines[cursor.Y].Substring(0, cursor.X - 1)
                    + Lines[cursor.Y].Substring(cursor.X);
                _Cursor = new Point(cursor.X - 1, cursor.Y);
            }

            TextChanged?.Invoke(this, new EventArgs());
            Updated?.Invoke(this, new EventArgs());
        }

        private void HandleReturn()
        {
            if (!Multiline || Text.Length >= MaxLength) return;

            var cursor = Cursor;
            var line = Lines[cursor.Y].Substring(cursor.X);
            _Lines[cursor.Y] = Lines[cursor.Y].Substring(0, cursor.X);
            _Lines.Insert(cursor.Y + 1, line);
            _Cursor = new Point(0, cursor.Y + 1);

            TextChanged?.Invoke(this, new EventArgs());
            Updated?.Invoke(this, new EventArgs());
        }

        public void MoveCursorUp() =>
            Cursor = Cursor.Y == 0
            ? new Point(0, 0)
            : new Point(Cursor.X, Cursor.Y - 1);

        public void MoveCursorDown() =>
            Cursor = Cursor.Y == Lines.Count - 1
            ? new Point(Lines[Cursor.Y].Length, Cursor.Y)
            : new Point(Cursor.X, Cursor.Y + 1);

        public void MoveCursorLeft() =>
            Cursor = Cursor.X == 0 && Cursor.Y > 0
                ? new Point(Lines[Cursor.Y - 1].Length, Cursor.Y - 1)
                : new Point(Cursor.X - 1, Cursor.Y);

        public void MoveCursorRight() =>
            Cursor = Cursor.X < Lines[Cursor.Y].Length
                ? new Point(Cursor.X + 1, Cursor.Y)
                : Cursor.Y < Lines.Count - 1
                ? new Point(0, Cursor.Y + 1)
                : new Point(Lines[Cursor.Y].Length, Cursor.Y);

        public void KeyDown(KeyboardEvent e)
        {
            if (e.KeyCode == KeyCode.Backspace) HandleBackspace();
            else if (e.KeyCode == KeyCode.Return) HandleReturn();
            else if (e.KeyCode == KeyCode.Up) MoveCursorUp();
            else if (e.KeyCode == KeyCode.Down) MoveCursorDown();
            else if (e.KeyCode == KeyCode.Left) MoveCursorLeft();
            else if (e.KeyCode == KeyCode.Right) MoveCursorRight();
            OnKeyPress?.Invoke(this, e);
        }

        // Return a safe, within-range value
        private static Point GetSafeCursor(Point cursor, List<string> lines)
        {
            if (lines.Count == 0) return new Point(0, 0);
            var y = Math.Max(0, Math.Min(cursor.Y, lines.Count - 1));
            var x = Math.Max(0, Math.Min(cursor.X, lines[y].Length));
            return new Point(x, y);
        }

        private string ToPasswordChar(string s) =>
            new String(PasswordChar, s.Length);

        public Texture CreateTexture(Renderer renderer)
        {
            var lines = UsePasswordChar
                ? Lines.Select(ToPasswordChar).ToArray()
                : Lines.ToArray();

            var cursorY = Cursor.Y * Font.LineSkip;
            var cursorX = Font.GetSize(lines[Cursor.Y].Substring(
                0, Math.Min(Cursor.X, lines[Cursor.Y].Length))).Width;

            using (var surface = Font.CreateSurface(lines, ForeColor))
                using (var fore = new Texture(renderer, surface))
                {
                    var size = fore.Size;
                    var rectangle = new Rectangle(
                        0, 0,
                        Math.Min(Size.Width, size.Width),
                        Math.Min(Size.Height, size.Height));

                    var back = new Texture(
                        renderer, PixelFormat.ARGB8888, TextureAccess.Target,
                        Size.Width, Size.Height);

                    renderer.WithRenderTarget(back, () => {
                        renderer.RenderClear(BackColor);
                        renderer.RenderCopy(fore, rectangle, rectangle);

                        var oldColor = renderer.Color;
                        renderer.Color = ForeColor;

                        if (Focused) renderer.RenderDrawLine(
                            cursorX, cursorY, cursorX, cursorY + Font.LineSkip);

                        if (BorderColor != Color.Empty)
                        {
                            renderer.Color = BorderColor;
                            renderer.RenderDrawRect(
                                new Rectangle(0, 0, Size.Width, Size.Height));
                        }

                        renderer.Color = oldColor;
                    });

                    return back;
                };
        }

        public TextBox(Font font, Size size)
        {
            Font = font;
            Size = size;
            Lines.Add(string.Empty);
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

            _Focused = false;
            DisableEvents();

            Disposed = true;
        }
    }
}
