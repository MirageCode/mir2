using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TextBox = SDL.TextBox;
using Font = SDL.Font;

namespace Client.MirControls
{
    public sealed class MirTextBox : MirControl
    {
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
            // TODO TextBox Location
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
            get => ActiveControl == this &&
                TextBox != null && !TextBox.Disposed && TextBox.Focused;
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                {
                    TextBox.Focused = value;
                    if (value) {
                        if (ActiveControl != null)
                            ActiveControl.Focused = false;
                        ActiveControl = this;
                    }
                }
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
            // TODO: TextBox Size

            // TextBox.Size = Size;
            // _size = TextBox.Size;

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
                    return TextBox.Lines;
                return null;
            }
            set
            {
                if (TextBox != null && !TextBox.Disposed)
                    TextBox.Lines = value;
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

            if (Visible && CanFocus && ActiveControl == null)
                ActiveControl = this;
        }
        private void SetFocus(object sender, EventArgs e)
        {
            if (Visible) ActiveControl = this;
        }

        #endregion

        #region MultiLine

        public override void MultiLine()
        {
            TextBox.Multiline = true;
        }

        #endregion

        public MirTextBox()
        {
            BackColour = Color.Black;

            TextBox = new TextBox(new Font(Settings.FontName, 10), Size)
            {
                BackColor = BackColour,
                ForeColor = ForeColour,
                // TODO: TextBox Location
                // Location = DisplayLocation,
            };
            // TODO: Events

            // TextBox.VisibleChanged += TextBox_VisibleChanged;
            // TextBox.ParentChanged += TextBox_VisibleChanged;
            // TextBox.KeyUp += TextBoxOnKeyUp;
            // TextBox.KeyPress += TextBox_KeyPress;

            // Shown += MirTextBox_Shown;
            // TextBox.MouseMove += CMain.CMain_MouseMove;
        }

        private void TextBoxOnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PrintScreen:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }

        void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)
            {
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

            if (!disposing) return;
        }


        #endregion
    }
}
