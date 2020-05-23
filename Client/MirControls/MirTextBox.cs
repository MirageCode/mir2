using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.MirGraphics;
using TextBox = SDL.TextBox;
using Font = SDL.Font;
using KeyboardEvent = SDL.KeyboardEvent;
using KeyCode = SDL.KeyCode;

namespace Client.MirControls
{
    public sealed class MirTextBox : MirControl
    {
        public static MirTextBox ActiveTextBox { get; set; }

        public static List<MirTextBox> TextBoxList = new List<MirTextBox>();

        #region Back Color

        protected override void OnBackColourChanged()
        {
            base.OnBackColourChanged();
            if (TextBox != null && !TextBox.Disposed)
                TextBox.BackColor = BackColour;
        }

        #endregion

        #region Enabled

        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();
            // TODO: TextBox Enabled
            // if (TextBox != null && !TextBox.Disposed)
            //     TextBox.Enabled = Enabled;
        }

        #endregion

        #region Fore Color

        protected override void OnForeColourChanged()
        {
            base.OnForeColourChanged();
            if (TextBox != null && !TextBox.Disposed)
                TextBox.ForeColor = ForeColour;
        }

        #endregion

        #region Location

        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();
            TextBox.Location = DisplayLocation;
        }

        #endregion

        #region Max Length

        public int MaxLength
        {
            get
            {
                if (TextBox != null && !TextBox.Disposed)
                    return TextBox.MaxLength;
                return -1;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.MaxLength = value;
            }
        }

        #endregion

        #region Parent

        protected override void OnParentChanged()
        {
            base.OnParentChanged();
            if (TextBox != null && !TextBox.Disposed)
                OnVisibleChanged();
        }

        #endregion

        #region Password

        public bool Password
        {
            get
            {
                if (TextBox != null && !TextBox.Disposed)
                    return TextBox.UsePasswordChar;
                return false;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.UsePasswordChar = value;
            }
        }

        #endregion

        #region Focused

        public override bool Focused
        {
            get => TextBox != null && !TextBox.Disposed && TextBox.Focused;
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.Focused = value;
            }
        }

        #endregion

        #region Font

        public Font Font
        {
            get
            {
                if (TextBox != null && !TextBox.Disposed)
                    return TextBox.Font;
                return null;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.Font = value;
            }
        }

        #endregion

        #region Size

        protected override void OnSizeChanged()
        {
            TextBox.Size = Size;
            _size = TextBox.Size;

            if (TextBox != null && !TextBox.Disposed)
                base.OnSizeChanged();
        }

        #endregion

        #region TextBox

        public bool CanFocus = true;
        public bool CanLoseFocus;
        public readonly TextBox TextBox;

        #endregion

        #region Label

        public string Text
        {
            get
            {
                if (TextBox != null && !TextBox.Disposed)
                    return TextBox.Text;
                return null;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.Text = value;
            }
        }
        public string[] MultiText
        {
            get
            {
                if (TextBox != null && !TextBox.Disposed)
                    return TextBox.Lines.ToArray();
                return null;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.Lines = value.ToList();
            }
        }

        #endregion

        #region Visible

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
                OnVisibleChanged();
            }
        }

        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();
        }
        private void TextBox_VisibleChanged(object sender, EventArgs e)
        {
            DialogChanged();

            if (Visible && CanFocus && ActiveControl == null) {
                ActiveControl = this;
                Focused = true;
            }
        }
        private void SetFocus(object sender, EventArgs e)
        {
            if (Visible) {
                ActiveControl = this;
                Focused = true;
            }
        }

        #endregion

        #region MultiLine

        public override void MultiLine()
        {
            TextBox.Multiline = true;
        }

        #endregion

        protected override void CreateTexture()
        {
            if (ControlTexture != null && !ControlTexture.Disposed)
                ControlTexture.Dispose();

            SDLManager.ControlList.Add(this);
            ControlTexture = SDLManager.CreateTexture(TextBox);
            ControlTexture.Disposing += ControlTexture_Disposing;
            TextureSize = ControlTexture.Size;
            TextureValid = true;
        }

        public MirTextBox()
        {
            BackColour = Color.Black;

            DrawControlTexture = true;

            TextBox = new TextBox(new Font(Settings.FontName, 10), Size)
            {
                BackColor = BackColour,
                ForeColor = ForeColour,
                Location = DisplayLocation,
            };

            TextBox.Updated += (o, e) => {
                TextureValid = false;
                Redraw();
            };
            TextBox.GotFocus += (o, e) => ActiveTextBox = this;
            TextBox.LostFocus += (o, e) => {
                if (ActiveTextBox != this) return;
                ActiveTextBox = null;
            };

            // TODO: Events

            // TextBox.VisibleChanged += TextBox_VisibleChanged;
            // TextBox.ParentChanged += TextBox_VisibleChanged;
            TextBox.OnKeyPress += TextBox_KeyPress;

            // Shown += MirTextBox_Shown;

            TextBoxList.Add(this);
        }

        protected override void Activate()
        {
            if (ActiveControl == this) return;
            base.Activate();

            if (ActiveTextBox != null && ActiveTextBox != this)
                ActiveTextBox.Focused = false;

            Focused = true;
        }

        void TextBox_KeyPress(TextBox sender, KeyboardEvent e)
        {
            base.OnKeyPress(e);

            if (e.KeyCode == KeyCode.Escape)
            {
                Focused = false;
                ActiveControl = null;
                e.Handled = true;
            }
        }


        void MirTextBox_Shown(object sender, EventArgs e)
        {
            CMain.Ctrl = false;
            CMain.Shift = false;
            CMain.Alt = false;
            CMain.Tilde = false;
        }

        public void SetFocus() => Focused = true;

        public void DialogChanged()
        {
            MirMessageBox box1 = null;
            MirInputBox box2 = null;
            MirAmountBox box3 = null;

            if (MirScene.ActiveScene != null && MirScene.ActiveScene.Controls.Count > 0)
            {
                box1 = (MirMessageBox) MirScene.ActiveScene.Controls.FirstOrDefault(ob => ob is MirMessageBox);
                box2 = (MirInputBox) MirScene.ActiveScene.Controls.FirstOrDefault(O => O is MirInputBox);
                box3 = (MirAmountBox) MirScene.ActiveScene.Controls.FirstOrDefault(ob => ob is MirAmountBox);
            }


            if ((box1 != null && box1 != Parent) || (box2 != null && box2 != Parent)  || (box3 != null && box3 != Parent))
                Visible = false;
            else
                Visible = Parent != null;
        }


        #region Disposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Focused = false;
            TextBoxList.Remove(this);

            if (!disposing) return;
        }


        #endregion
    }
}
