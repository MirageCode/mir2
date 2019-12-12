using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirScenes;
using Client.MirSounds;
using Font = System.Drawing.Font;
using SDL;

namespace Client
{
    public partial class CMain : IDisposable
    {
        public static MirControl DebugBaseLabel, HintBaseLabel;
        public static MirLabel DebugTextLabel, HintTextLabel, ScreenshotTextLabel;
        public static Point MPoint { get => SDLContext.MousePoint; }

        public readonly static Stopwatch Timer = Stopwatch.StartNew();
        public readonly static DateTime StartTime = DateTime.Now;
        public static long Time, OldTime;
        public static DateTime Now { get { return StartTime.AddMilliseconds(Time); } }
        public static readonly Random Random = new Random();

        public static bool DebugOverride;

        private static long _fpsTime;
        private static int _fps;
        public static int FPS;

        public static bool Shift, Alt, Ctrl, Tilde;
        public static KeyBindSettings InputKeys = new KeyBindSettings();

        public bool Closing = false;

        public bool Disposed { get; private set; }
        public event EventHandler Disposing;

        public CMain()
        {
            SDLManager.Create();
            SoundManager.Create();

            LoginScene LoginScene = new LoginScene();
            MirScene.ActiveScene = LoginScene;

            Event.OnQuit += e => MirScene.ActiveScene.Close();

            Event.OnMouseButtonUp += CMain_MouseClick;
            Event.OnMouseButtonDown += CMain_MouseDown;
            Event.OnMouseButtonUp += CMain_MouseUp;
            Event.OnMouseMotion += CMain_MouseMove;
            Event.OnKeyDown += CMain_KeyPress;
            Event.OnKeyDown += CMain_KeyDown;
            Event.OnKeyUp += CMain_KeyUp;
            Event.OnMouseWheel += CMain_MouseWheel;
        }

        ~CMain() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing) {
                EventHandler handler = Disposing;
                if (handler != null) handler(this, new EventArgs());
            }

            SDLManager.Destroy();

            Disposed = true;
        }

        public void Close() => Closing = true;

        public void Run()
        {
            try
            {
                while (!Closing)
                {
                    UpdateTime();
                    Event.Poll();
                    UpdateEnviroment();
                    RenderEnvironment();
                }

            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }

        public static void CMain_KeyDown(KeyboardEvent e)
        {
            Shift = e.Shift;
            Alt = e.Alt;
            Ctrl = e.Ctrl;

            // TODO: Tilde

            // if (e.KeyCode == Keys.Oem8)
            //     CMain.Tilde = true;

            try
            {
                if (e.Alt && e.KeyCode == KeyCode.Return)
                {
                    SDLManager.ToggleFullScreen();
                    return;
                }

                if (MirScene.ActiveScene != null
                    && MirTextBox.ActiveTextBox == null)
                    MirScene.ActiveScene.OnKeyDown(e);

            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
            if (e.KeyCode == KeyCode.F10)
            {
                e.Handled = true;
                if (GameScene.Scene != null)
                {
                    GameScene.Scene.F10();
                }
            }
        }
        public static void CMain_MouseMove(MouseMotionEvent e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseMove(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_KeyUp(KeyboardEvent e)
        {
            Shift = e.Shift;
            Alt = e.Alt;
            Ctrl = e.Ctrl;

            // TODO: Tilde

            // if (e.KeyCode == Keys.Oem8)
            //     CMain.Tilde = false;

            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.function != KeybindOptions.Screenshot) continue;
                if (KeyCheck.Key != e.KeyCode)
                    continue;
                if ((KeyCheck.RequireAlt != 2) && (KeyCheck.RequireAlt != (Alt ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireShift != 2) && (KeyCheck.RequireShift != (Shift ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireCtrl != 2) && (KeyCheck.RequireCtrl != (Ctrl ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireTilde != 2) && (KeyCheck.RequireTilde != (Tilde ? 1 : 0)))
                    continue;
                Program.Form.CreateScreenShot();
                break;

            }
            try
            {
                if (MirScene.ActiveScene != null
                    && MirTextBox.ActiveTextBox == null)
                    MirScene.ActiveScene.OnKeyUp(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_KeyPress(KeyboardEvent e)
        {
            if (e.Repeat || MirTextBox.ActiveTextBox != null) return;

            try
            {
                if (MirScene.ActiveScene != null
                    && MirTextBox.ActiveTextBox == null)
                    MirScene.ActiveScene.OnKeyPress(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_MouseUp(MouseButtonEvent e)
        {
            MapControl.MapButtons &= ~e.Button;
            if (!MapControl.MapButtons.HasFlag(MouseButton.Right))
                GameScene.CanRun = false;

            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseUp(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_MouseDown(MouseButtonEvent e)
        {
            if (MirControl.ActiveControl is MirTextBox)
            {
                MirTextBox textBox = (MirTextBox) MirControl.ActiveControl;

                if (textBox != null && textBox.CanLoseFocus)
                    textBox.Focused = false;
            }

            if (e.Button == MouseButton.Right && (GameScene.SelectedCell != null || GameScene.PickedUpGold))
            {
                GameScene.SelectedCell = null;
                GameScene.PickedUpGold = false;
                return;
            }

            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_MouseClick(MouseButtonEvent e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseClick(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }
        public static void CMain_MouseWheel(MouseWheelEvent e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseWheel(e);
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
            }
        }

        private static void UpdateTime()
        {
            Time = Timer.ElapsedMilliseconds;
        }
        private static void UpdateEnviroment()
        {  

            if (Time >= _fpsTime)
            {
                _fpsTime = Time + 1000;
                FPS = _fps;
                _fps = 0;
                SDLManager.Clean(); // Clean once a second.
            }
            else
                _fps++;

            Network.Process();

            if (MirScene.ActiveScene != null)
                MirScene.ActiveScene.Process();

            for (int i = 0; i < MirAnimatedControl.Animations.Count; i++)
                MirAnimatedControl.Animations[i].UpdateOffSet();

            for (int i = 0; i < MirAnimatedButton.Animations.Count; i++)
                MirAnimatedButton.Animations[i].UpdateOffSet();

            CreateHintLabel();
            CreateDebugLabel();
 
        }
        private static void RenderEnvironment()
        {
            SDLManager.Renderer.RenderClear(Color.CornflowerBlue);
            if (MirScene.ActiveScene != null)
                MirScene.ActiveScene.Draw();

            SDLManager.Renderer.RenderPresent();
        }

        private static void CreateDebugLabel()
        {
            if (!Settings.DebugMode) return;

            if (DebugBaseLabel == null || DebugBaseLabel.IsDisposed)
            {
                DebugBaseLabel = new MirControl
                    {
                        BackColour = Color.FromArgb(50, 50, 50),
                        Border = true,
                        BorderColour = Color.Black,
                        DrawControlTexture = true,
                        Location = new Point(5, 5),
                        NotControl = true,
                        Opacity = 0.5F
                    };
            }
            
            if (DebugTextLabel == null || DebugTextLabel.IsDisposed)
            {
                DebugTextLabel = new MirLabel
                {
                    AutoSize = true,
                    BackColour = Color.Transparent,
                    ForeColour = Color.White,
                    Parent = DebugBaseLabel,
                };

                DebugTextLabel.SizeChanged += (o, e) => DebugBaseLabel.Size = DebugTextLabel.Size;
            }

            if (DebugOverride) return;
            
            string text;
            if (MirControl.MouseControl != null)
            {
                text = string.Format("FPS: {0}", FPS);

                if (MirControl.MouseControl is MapControl)
                {
                    text += string.Format(", Co Ords: {0}", MapControl.MapLocation);

                    //text += "\r\n";

                    //var cell = GameScene.Scene.MapControl.M2CellInfo[MapControl.MapLocation.X, MapControl.MapLocation.Y];

                    //if (cell != null)
                    //{
                    //    text += string.Format("BackImage : {0}. BackIndex : {1}. MiddleImage : {2}. MiddleIndex {3}. FrontImage : {4}. FrontIndex : {5}", cell.BackImage, cell.BackIndex, cell.MiddleImage, cell.MiddleIndex, cell.FrontImage, cell.FrontIndex);
                    //}
                }

                if (MirScene.ActiveScene is GameScene)
                {
                    //text += "\r\n";
                    text += string.Format(", Objects: {0}", MapControl.Objects.Count);
                }
                if (MirObjects.MapObject.MouseObject != null)
                {
                    text += string.Format(", Target: {0}", MirObjects.MapObject.MouseObject.Name);
                }
                else
                {
                    text += string.Format(", Target: none");
                }
            }
            else
            {
                text = string.Format("FPS: {0}", FPS);
            }
            

            DebugTextLabel.Text = text;
        }

        public static void SendDebugMessage(string text)
        {
            if (!Settings.DebugMode) return;

            if (DebugBaseLabel == null || DebugTextLabel == null)
            {
                CreateDebugLabel();
            }

            DebugOverride = true;

            DebugTextLabel.Text = text;
        }

        private static void CreateHintLabel()
        {
            if (HintBaseLabel == null || HintBaseLabel.IsDisposed)
            {
                HintBaseLabel = new MirControl
                {
                    BackColour = Color.FromArgb(128, 128, 50),
                    Border = true,
                    DrawControlTexture = true,
                    BorderColour = Color.Yellow,
                    ForeColour = Color.Yellow,
                    Parent = MirScene.ActiveScene,
                    NotControl = true,
                    Opacity = 0.5F
                };
            }


            if (HintTextLabel == null || HintTextLabel.IsDisposed)
            {
                HintTextLabel = new MirLabel
                {
                    AutoSize = true,
                    BackColour = Color.Transparent,
                    ForeColour = Color.White,
                    Parent = HintBaseLabel,
                };

                HintTextLabel.SizeChanged += (o, e) => HintBaseLabel.Size = HintTextLabel.Size;
            }

            if (MirControl.MouseControl == null || string.IsNullOrEmpty(MirControl.MouseControl.Hint))
            {
                HintBaseLabel.Visible = false;
                return;
            }

            HintBaseLabel.Visible = true;

            HintTextLabel.Text = MirControl.MouseControl.Hint;

            Point point = MPoint.Add(-HintTextLabel.Size.Width, 20);

            if (point.X + HintBaseLabel.Size.Width >= Settings.ScreenWidth)
                point.X = Settings.ScreenWidth - HintBaseLabel.Size.Width - 1;
            if (point.Y + HintBaseLabel.Size.Height >= Settings.ScreenHeight)
                point.Y = Settings.ScreenHeight - HintBaseLabel.Size.Height - 1;

            if (point.X < 0)
                point.X = 0;
            if (point.Y < 0)
                point.Y = 0;

            HintBaseLabel.Location = point;
        }

        public void CreateScreenShot()
        {
            // Point location = PointToClient(Location);

            // location = new Point(-location.X, -location.Y);

            string text = string.Format("[{0} Server {1}] {2} {3:hh\\:mm\\:ss}", 
                Settings.P_ServerName.Length > 0 ? Settings.P_ServerName : "Crystal", 
                MapControl.User != null ? MapControl.User.Name : "", 
                Now.ToShortDateString(), 
                Now.TimeOfDay);

            // TODO: Screenshot

            // using (Bitmap image = GetImage(Handle, new Rectangle(location, ClientSize)))
            // using (Graphics graphics = Graphics.FromImage(image))
            // {
            //     StringFormat sf = new StringFormat();
            //     sf.LineAlignment = StringAlignment.Center;
            //     sf.Alignment = StringAlignment.Center;

            //     graphics.DrawString(text, new Font(Settings.FontName, 9F), Brushes.Black, new Point((Settings.ScreenWidth / 2) + 3, 10), sf);
            //     graphics.DrawString(text, new Font(Settings.FontName, 9F), Brushes.Black, new Point((Settings.ScreenWidth / 2) + 4, 9), sf);
            //     graphics.DrawString(text, new Font(Settings.FontName, 9F), Brushes.Black, new Point((Settings.ScreenWidth / 2) + 5, 10), sf);
            //     graphics.DrawString(text, new Font(Settings.FontName, 9F), Brushes.Black, new Point((Settings.ScreenWidth / 2) + 4, 11), sf);
            //     graphics.DrawString(text, new Font(Settings.FontName, 9F), Brushes.White, new Point((Settings.ScreenWidth / 2) + 4, 10), sf);//SandyBrown               

            //     string path = Path.Combine(Application.StartupPath, @"Screenshots\");
            //     if (!Directory.Exists(path))
            //         Directory.CreateDirectory(path);

            //     int count = Directory.GetFiles(path, "*.png").Length;

            //     image.Save(Path.Combine(path, string.Format("Image {0}.Png", count)), ImageFormat.Png);
            // }
        }

        public static void SaveError(string ex)
        {
            try
            {
                if (Settings.RemainingErrorLogs-- > 0)
                {
                    File.AppendAllText(Path.Combine(".", "Error.txt"),
                                       string.Format("[{0}] {1}{2}", Now, ex, Environment.NewLine));
                }
            }
            catch
            {
            }
        }

        public static void SetResolution(int width, int height)
        {
            if (Settings.ScreenWidth == width && Settings.ScreenHeight == height) return;

            Settings.ScreenWidth = width;
            Settings.ScreenHeight = height;
            // TODO: Set Window size
            // Program.Form.ClientSize = new Size(width, height);
        }
    }
}
