﻿using System;
using System.Drawing;
using Client.MirGraphics;
using SDL;


namespace Client.MirControls
{
    public sealed class MirInputBox : MirImageControl
    {
        public readonly MirLabel CaptionLabel;
        public readonly MirButton OKButton, CancelButton;
        public readonly MirTextBox InputTextBox;


        public MirInputBox(string message)
        {
            Modal = true;
            Movable = false;

            Index = 660;
            Library = Libraries.Prguse;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            CaptionLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.WordBreak,
                Location = new Point(25, 25),
                Size = new Size(235, 40),
                Parent = this,
                Text = message,
            };

            InputTextBox = new MirTextBox
            {
                Parent = this,
                Border = true,
                BorderColour = Color.Lime,
                Location = new Point(23, 86),
                Size = new Size(240, 19),
                MaxLength = 50,
            };
            InputTextBox.SetFocus();
            InputTextBox.TextBox.OnKeyPress += MirInputBox_KeyPress;

            OKButton = new MirButton
            {
                HoverIndex = 201,
                Index = 200,
                Library = Libraries.Title,
                Location = new Point(60, 123),
                Parent = this,
                PressedIndex = 202,
            };

            CancelButton = new MirButton
            {
                HoverIndex = 204,
                Index = 203,
                Library = Libraries.Title,
                Location = new Point(160, 123),
                Parent = this,
                PressedIndex = 205,
            };
            CancelButton.Click += DisposeDialog;
        }

        void MirInputBox_KeyPress(TextBox sender, KeyboardEvent e)
        {
            if (e.KeyCode == KeyCode.Return)
            {
                if (OKButton != null && !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(null);
                e.Handled = true;
            }
            else if (e.KeyCode == KeyCode.Escape)
            {
                if (CancelButton != null && !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(null);
                e.Handled = true;
            }
        }
        void DisposeDialog(object sender, MouseButtonEvent e)
        {
            Dispose();
        }

        public override void OnKeyDown(KeyboardEvent e)
        {
            base.OnKeyDown(e);
            e.Handled = true;
        }
        public override void OnKeyUp(KeyboardEvent e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }
        public override void OnKeyPress(KeyboardEvent e)
        {
            base.OnKeyPress(e);

            if (e.KeyCode == KeyCode.Escape)
            {
                if (CancelButton != null && !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(null);
            }
            else if (e.KeyCode == KeyCode.Return)
            {
                if (OKButton != null && !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(null);

            }
            e.Handled = true;
        }

        public void Show()
        {
            if (Parent != null) return;

            Parent = MirScene.ActiveScene;

            Highlight();

            for (int i = 0; i < Controls.Count; i++)
            {
                MirTextBox T = Controls[i] as MirTextBox;
                if (T != null) T.DialogChanged();
            }
        }


        #region Disposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            for (int i = 0; i < Controls.Count; i++)
            {
                MirTextBox T = Controls[i] as MirTextBox;
                if (T != null) T.DialogChanged();
            }
        }

        #endregion

    }
}
