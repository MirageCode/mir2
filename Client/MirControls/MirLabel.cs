using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Client.MirGraphics;
using SDL;
using Font = SDL.Font;

using TextFormatFlags = System.Windows.Forms.TextFormatFlags;

namespace Client.MirControls
{
    public class MirLabel : MirControl
    {
        #region Auto Size
        private bool _autoSize;
        public bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                if (_autoSize == value)
                    return;
                _autoSize = value;
                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
        public event EventHandler AutoSizeChanged;
        private void OnAutoSizeChanged(EventArgs e)
        {
            TextureValid = false;
            GetSize();
            if (AutoSizeChanged != null)
                AutoSizeChanged.Invoke(this, e);
        }
        #endregion

        #region DrawFormat
        private TextFormatFlags _drawFormat;
        public TextFormatFlags DrawFormat
        {
            get { return _drawFormat; }
            set
            {
                _drawFormat = value;
                OnDrawFormatChanged(EventArgs.Empty);
            }
        }
        public event EventHandler DrawFormatChanged;
        private void OnDrawFormatChanged(EventArgs e)
        {
            TextureValid = false;

            if (DrawFormatChanged != null)
                DrawFormatChanged.Invoke(this, e);
        }
        #endregion

        #region Font
        private Font _font;
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                OnFontChanged(EventArgs.Empty);
            }
        }
        public event EventHandler FontChanged;
        private void OnFontChanged(EventArgs e)
        {
            TextureValid = false;

            GetSize();

            if (FontChanged != null)
                FontChanged.Invoke(this, e);
        }
        #endregion

        #region Out Line
        private bool _outLine;
        public bool OutLine
        {
            get { return _outLine; }
            set
            {
                if (_outLine == value)
                    return;
                _outLine = value;
                OnOutLineChanged(EventArgs.Empty);
            }
        }
        public event EventHandler OutLineChanged;
        private void OnOutLineChanged(EventArgs e)
        {
            TextureValid = false;
            GetSize();
            
            if (OutLineChanged != null)
                OutLineChanged.Invoke(this, e);
        }
        #endregion

        #region Out Line Colour
        private Color _outLineColour;
        public Color OutLineColour
        {
            get { return _outLineColour; }
            set
            {
                if (_outLineColour == value)
                    return;
                _outLineColour = value;
                OnOutLineColourChanged();
            }
        }
        public event EventHandler OutLineColourChanged;
        private void OnOutLineColourChanged()
        {
            TextureValid = false;

            if (OutLineColourChanged != null)
                OutLineColourChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Size

        private void GetSize()
        {
            if (!AutoSize)
                return;

            if (string.IsNullOrEmpty(_text))
                Size = Size.Empty;
            else
            {
                Size = Font.GetSize(Text);
                //Size = new Size(Size.Width, Size.Height + 5);

                if (OutLine && Size != Size.Empty)
                    Size = new Size(Size.Width + 2, Size.Height + 2);
            }
        }
        #endregion

        #region Label
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;

                _text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }
        public event EventHandler TextChanged;
        private void OnTextChanged(EventArgs e)
        {
            DrawControlTexture = !string.IsNullOrEmpty(Text);
            TextureValid = false;
            Redraw();

            GetSize();

            if (TextChanged != null)
                TextChanged.Invoke(this, e);
        }
        #endregion

        public MirLabel()
        {
            DrawControlTexture = true;
            _drawFormat = TextFormatFlags.WordBreak;

            _font = new Font(Settings.FontName, 8);
            _outLine = true;
            _outLineColour = Color.Black; 
            _text = string.Empty;

        }

        protected override unsafe void CreateTexture()
        {
            if (string.IsNullOrEmpty(Text))
                return;

            if (Size.Width == 0 || Size.Height == 0)
                return;

            if (ControlTexture != null && !ControlTexture.Disposed && TextureSize != Size)
                ControlTexture.Dispose();

            if (ControlTexture == null || ControlTexture.Disposed)
            {
                SDLManager.ControlList.Add(this);

                ControlTexture = SDLManager.CreateTexture(Size.Width, Size.Height);
                ControlTexture.Disposing += ControlTexture_Disposing;
                TextureSize = Size;
            }

            // TODO: DrawFormat
            SDLManager.DrawToTexture(ControlTexture, BackColour, () => {
                if (OutLine)
                {
                    SDLManager.DrawText(Text, Font, new Rectangle(1, 0, Size.Width, Size.Height), OutLineColour);
                    SDLManager.DrawText(Text, Font, new Rectangle(0, 1, Size.Width, Size.Height), OutLineColour);
                    SDLManager.DrawText(Text, Font, new Rectangle(2, 1, Size.Width, Size.Height), OutLineColour);
                    SDLManager.DrawText(Text, Font, new Rectangle(1, 2, Size.Width, Size.Height), OutLineColour);
                    SDLManager.DrawText(Text, Font, new Rectangle(1, 1, Size.Width, Size.Height), ForeColour);
                }
                else
                    SDLManager.DrawText(Text, Font, new Rectangle(1, 0, Size.Width, Size.Height), ForeColour);
            });

            TextureValid = true;
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            
            AutoSizeChanged = null;
            _autoSize = false;

            DrawFormatChanged = null;
            _drawFormat = 0;

            FontChanged = null;
            _font = null;

            OutLineChanged = null;
            _outLine = false;

            OutLineColourChanged = null;
            _outLineColour = Color.Empty;

            TextChanged = null;
            _text = null;
        }
        #endregion

    }
}
