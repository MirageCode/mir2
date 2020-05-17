using System;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;

namespace SDL
{
    [Flags]
    public enum MouseButton : byte
    {
        None = 0,
        Left = (1 << 0),
        Middle = (1 << 1),
        Right = (1 << 2),
        X1 = (1 << 3),
        X2 = (1 << 4),
    }

    public enum WindowEventType : byte
    {
        None,
        Shown,
        Hidden,
        Exposed,
        Moved,
        Resized,
        SizeChanged,
        Minimized,
        Maximized,
        Restored,
        Enter,
        Leave,
        FocusGained,
        FocusLost,
        Close,
        /* Available in 2.0.5 or higher */
        TakeFocus,
        HitTest,
    }

    public enum DisplayEventType : byte
    {
        None,
        Orientation,
    }

    public enum EventType : uint
    {
        First = 0,

        /* Application events */
        Quit = 0x100,

        /* iOS/Android/WinRT app events */
        AppTerminating,
        AppLowmemory,
        AppWillEnterBackground,
        AppDidEnterBackground,
        AppWillEnterForeground,
        AppDidEnterForeground,

        /* Display events */
        /* Only available in SDL 2.0.9 or higher */
        Display = 0x150,

        /* Window events */
        Window = 0x200,
        SysWM,

        /* Keyboard events */
        KeyDown = 0x300,
        KeyUp,
        TextEditing,
        TextInput,
        KeyMapChanged,

        /* Mouse events */
        MouseMotion = 0x400,
        MouseButtonDown,
        MouseButtonUp,
        MouseWheel,

        /* Joystick events */
        JoyAxisMotion = 0x600,
        JoyBallMotion,
        JoyHatMotion,
        JoyButtonDown,
        JoyButtonUp,
        JoyDeviceAdded,
        JoyDevicereMoved,

        /* Game controller events */
        ControllerAxisMotion = 0x650,
        ControllerButtonDown,
        ControllerButtonUp,
        ControllerDeviceAdded,
        ControllerDeviceRemoved,
        ControllerDevicereMapped,

        /* Touch events */
        FingerDown = 0x700,
        FingerUp,
        FingerMotion,

        /* Gesture events */
        DollarGesture = 0x800,
        DollarRecord,
        MultiGesture,

        /* Clipboard events */
        ClipboardUpdate = 0x900,

        /* Drag and drop events */
        DropFile = 0x1000,
        /* Only available in 2.0.4 or higher */
        DropText,
        DropBegin,
        DropComplete,

        /* Audio hotplug events */
        /* Only available in SDL 2.0.4 or higher */
        AudioDeviceAdded = 0x1100,
        AudioDeviceRemoved,

        /* Sensor events */
        /* Only available in SDL 2.0.9 or higher */
        SensorUpdate = 0x1200,

        /* Render events */
        /* Only available in SDL 2.0.2 or higher */
        RenderTargetsReset = 0x2000,
        /* Only available in SDL 2.0.4 or higher */
        RenderDeviceReset,

        /* Events SDL_USEREVENT through SDL_LASTEVENT are for
         * your use, and should be allocated with
         * SDL_RegisterEvents()
         */
        User = 0x8000,

        /* The last event, used for bouding arrays. */
        Last = 0xFFFF,
    }

    public enum ButtonState : byte
    {
        Released,
        Pressed,
    }

    /* Only available in 2.0.4 or higher */
    public enum MouseWheelDirection : uint
    {
        Normal,
        Flipped,
    }

    /* Fields shared by every event */
    [StructLayout(LayoutKind.Sequential)]
    public struct InternalBaseEvent
    {
        public EventType type;
        public UInt32 timestamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalDisplayEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 display;
        public DisplayEventType displayEventType;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int32 data1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalWindowEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public WindowEventType windowEventType;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int32 data1;
        public Int32 data2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalKeyboardEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public ButtonState state;
        public byte repeat; /* non-zero if this is a repeat */
        private byte padding2;
        private byte padding3;
        public Keysym keysym;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct InternalTextEditingEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        /* Default size is according to SDL2 default. */
        public fixed byte text[32];
        public Int32 start;
        public Int32 length;

        public unsafe string Text {
            get {
                fixed (byte *ptr = text) return Util.ToString((IntPtr) ptr);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct InternalTextInputEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        /* Default size is according to SDL2 default. */
        public fixed byte text[32];

        public unsafe string Text {
            get {
                fixed (byte *ptr = text) return Util.ToString((IntPtr) ptr);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalMouseMotionEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public UInt32 which;
        public MouseButton state; /* bitmask of buttons */
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int32 x;
        public Int32 y;
        public Int32 xrel;
        public Int32 yrel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalMouseButtonEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public UInt32 which;
        public byte button; /* button id */
        public ButtonState state; /* SDL_PRESSED or SDL_RELEASED */
        public byte clicks; /* 1 for single-click, 2 for double-click, etc. */
        private byte padding1;
        public Int32 x;
        public Int32 y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalMouseWheelEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public UInt32 which;
        public Int32 x; /* amount scrolled horizontally */
        public Int32 y; /* amount scrolled vertically */
        public UInt32 direction; /* Set to one of the SDL_MOUSEWHEEL_* defines */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalJoyAxisEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int16 axisValue;
        public UInt16 padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalJoyBallEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte ball;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int16 xrel;
        public Int16 yrel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalJoyHatEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte hat;
        public byte hatValue;
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalJoyButtonEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte button;
        public ButtonState state; /* SDL_PRESSED or SDL_RELEASED */
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalJoyDeviceEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalControllerAxisEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int16 axisValue;
        private UInt16 padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalControllerButtonEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte button;
        public ButtonState state;
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalControllerDeviceEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which; /* joystick id for ADDED, else instance id */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalAudioDeviceEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public UInt32 which;
        public byte iscapture;
        private byte padding1;
        private byte padding2;
        private byte padding3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalTouchFingerEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public Int64 touchId;
        public Int64 fingerId;
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalMultiGestureEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public Int64 touchId;
        public float dTheta;
        public float dDist;
        public float x;
        public float y;
        public UInt16 numFingers;
        public UInt16 padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalDollarGestureEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public Int64 touchId;
        public Int64 gestureId;
        public UInt32 numFingers;
        public float error;
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalDropEvent
    {
        public EventType type;
        public UInt32 timestamp;

        /* char* filename, to be freed. */
        public IntPtr file;
        public UInt32 windowID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct InternalSensorEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public Int32 which;
        public fixed float data[6];
    }

    /* The "quit requested" event */
    [StructLayout(LayoutKind.Sequential)]
    public struct InternalQuitEvent
    {
        public EventType type;
        public UInt32 timestamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalUserEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public Int32 code;
        public IntPtr data1; /* user-defined */
        public IntPtr data2; /* user-defined */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InternalSysWMEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public IntPtr msg; /* SDL_SysWMmsg*, system-dependent*/
    }

    // C# doesn't do unions, so we do this ugly thing. */
    [StructLayout(LayoutKind.Explicit)]
    public struct GenericEvent
    {
        [FieldOffset(0)]
        public EventType type;
        [FieldOffset(0)]
        public InternalBaseEvent any;
        [FieldOffset(0)]
        public InternalDisplayEvent display;
        [FieldOffset(0)]
        public InternalWindowEvent window;
        [FieldOffset(0)]
        public InternalKeyboardEvent key;
        [FieldOffset(0)]
        public InternalTextEditingEvent edit;
        [FieldOffset(0)]
        public InternalTextInputEvent text;
        [FieldOffset(0)]
        public InternalMouseMotionEvent motion;
        [FieldOffset(0)]
        public InternalMouseButtonEvent button;
        [FieldOffset(0)]
        public InternalMouseWheelEvent wheel;
        [FieldOffset(0)]
        public InternalJoyAxisEvent jaxis;
        [FieldOffset(0)]
        public InternalJoyBallEvent jball;
        [FieldOffset(0)]
        public InternalJoyHatEvent jhat;
        [FieldOffset(0)]
        public InternalJoyButtonEvent jbutton;
        [FieldOffset(0)]
        public InternalJoyDeviceEvent jdevice;
        [FieldOffset(0)]
        public InternalControllerAxisEvent caxis;
        [FieldOffset(0)]
        public InternalControllerButtonEvent cbutton;
        [FieldOffset(0)]
        public InternalControllerDeviceEvent cdevice;
        [FieldOffset(0)]
        public InternalAudioDeviceEvent adevice;
        [FieldOffset(0)]
        public InternalSensorEvent sensor;
        [FieldOffset(0)]
        public InternalQuitEvent quit;
        [FieldOffset(0)]
        public InternalUserEvent user;
        [FieldOffset(0)]
        public InternalSysWMEvent syswm;
        [FieldOffset(0)]
        public InternalTouchFingerEvent tfinger;
        [FieldOffset(0)]
        public InternalMultiGestureEvent mgesture;
        [FieldOffset(0)]
        public InternalDollarGestureEvent dgesture;
        [FieldOffset(0)]
        public InternalDropEvent drop;
    }

    public enum EventAction
    {
        Add,
        Peek,
        Get,
    }

    public class EventTypeException : Exception
    {
        public EventTypeException(EventType type, string field)
        : base("Can't get '" + field + "' for event type: " + type) {}
    }

    public class Event
    {
        public const string SDLLib = SDLContext.SDLLib;

        private GenericEvent _GenericEvent;
        protected GenericEvent GenericEvent
        {
            get => _GenericEvent;
            private set => _GenericEvent = value;
        }

        public EventType Type { get => GenericEvent.type; }
        public UInt32 Timestamp { get => GenericEvent.any.timestamp; }

        public bool Handled { get; set; } = false;

        protected internal Event(GenericEvent genericEvent) => GenericEvent = genericEvent;

        private static void EnsureSafe(int status)
        {
            if (status < 0) throw new SDLException();
        }

        public delegate void EventDelegate<T>(T Event) where T : Event;

        public static event EventDelegate<QuitEvent> OnQuit;
        public static event EventDelegate<MouseButtonEvent> OnMouseButton;
        public static event EventDelegate<MouseButtonEvent> OnMouseButtonDown;
        public static event EventDelegate<MouseButtonEvent> OnMouseButtonUp;
        public static event EventDelegate<MouseWheelEvent> OnMouseWheel;
        public static event EventDelegate<MouseMotionEvent> OnMouseMotion;
        public static event EventDelegate<TextInputEvent> OnTextInput;
        public static event EventDelegate<TextEditingEvent> OnTextEditing;
        public static event EventDelegate<KeyboardEvent> OnKeyDown;
        public static event EventDelegate<KeyboardEvent> OnKeyUp;
        public static event EventDelegate<WindowEvent> OnWindowResize;
        public static event EventDelegate<WindowEvent> OnWindowSizeChange;
        public static event EventDelegate<WindowEvent> OnWindowMinimize;
        public static event EventDelegate<WindowEvent> OnWindowMaximize;
        public static event EventDelegate<WindowEvent> OnWindowRestore;

        public static void Poll()
        {
            GenericEvent GenericEvent;
            if (SDL_PollEvent(out GenericEvent) == 0) return;

            if (GenericEvent.type == EventType.Quit)
                OnQuit?.Invoke(new QuitEvent(GenericEvent));
            else if (GenericEvent.type == EventType.MouseButtonDown)
            {
                OnMouseButtonDown?.Invoke(new MouseButtonEvent(GenericEvent));
                OnMouseButton?.Invoke(new MouseButtonEvent(GenericEvent));
            }
            else if (GenericEvent.type == EventType.MouseButtonUp)
            {
                OnMouseButtonUp?.Invoke(new MouseButtonEvent(GenericEvent));
                OnMouseButton?.Invoke(new MouseButtonEvent(GenericEvent));
            }
            else if (GenericEvent.type == EventType.MouseWheel)
                OnMouseWheel?.Invoke(new MouseWheelEvent(GenericEvent));
            else if (GenericEvent.type == EventType.MouseMotion)
                OnMouseMotion?.Invoke(new MouseMotionEvent(GenericEvent));
            else if (GenericEvent.type == EventType.TextInput)
                OnTextInput?.Invoke(new TextInputEvent(GenericEvent));
            else if (GenericEvent.type == EventType.TextEditing)
                OnTextEditing?.Invoke(new TextEditingEvent(GenericEvent));
            else if (GenericEvent.type == EventType.KeyDown)
                OnKeyDown?.Invoke(new KeyboardEvent(GenericEvent));
            else if (GenericEvent.type == EventType.KeyUp)
                OnKeyUp?.Invoke(new KeyboardEvent(GenericEvent));
            else if (GenericEvent.type == EventType.Window)
            {
                var Event = new WindowEvent(GenericEvent);
                if (Event.WindowEventType == WindowEventType.Resized)
                    OnWindowResize?.Invoke(Event);
                else if (Event.WindowEventType == WindowEventType.SizeChanged)
                    OnWindowSizeChange?.Invoke(Event);
                else if (Event.WindowEventType == WindowEventType.Minimized)
                    OnWindowMinimize?.Invoke(Event);
                else if (Event.WindowEventType == WindowEventType.Maximized)
                    OnWindowMaximize?.Invoke(Event);
                else if (Event.WindowEventType == WindowEventType.Restored)
                    OnWindowRestore?.Invoke(Event);
            }
        }

        protected static MouseButton ToButton(byte X) =>
            (MouseButton) (1 << ((int) X - 1));

        [DllImport(SDLLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_PollEvent(out GenericEvent GenericEvent);
    }

    public class QuitEvent : Event
    {
        internal QuitEvent(GenericEvent genericEvent) : base(genericEvent) {}
    }

    public class MouseButtonEvent : Event
    {
        internal MouseButtonEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalMouseButtonEvent Event { get => GenericEvent.button; }

        public MouseButton Button { get => ToButton(Event.button); }
        public byte Clicks { get => Event.clicks; }
        public ButtonState State { get => Event.state; }
        public Point Location { get => new Point(Event.x, Event.y); }
    }

    public class MouseWheelEvent : Event
    {
        internal MouseWheelEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalMouseWheelEvent Event { get => GenericEvent.wheel; }

        public Point Scrolled { get => new Point(Event.x, Event.y); }
    }

    public class MouseMotionEvent : Event
    {
        internal MouseMotionEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalMouseMotionEvent Event { get => GenericEvent.motion; }

        public MouseButton Button { get => Event.state; }
        public Point Location { get => new Point(Event.x, Event.y); }
        public Point Rel { get => new Point(Event.xrel, Event.yrel); }
    }

    public class TextInputEvent : Event
    {
        internal TextInputEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalTextInputEvent Event { get => GenericEvent.text; }

        public string Text { get => Event.Text; }
    }

    public class TextEditingEvent : Event
    {
        internal TextEditingEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalTextEditingEvent Event { get => GenericEvent.edit; }

        public string Text { get => Event.Text; }
    }

    public class KeyboardEvent : Event
    {
        internal KeyboardEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalKeyboardEvent Event { get => GenericEvent.key; }

        public bool Repeat { get => Event.repeat != 0; }
        public ButtonState State { get => Event.state; }
        public ScanCode ScanCode { get => Event.keysym.scancode; }
        public KeyCode KeyCode { get => Event.keysym.sym; }
        public KeyMod KeyMod { get => Event.keysym.mod; }

        public bool Shift
        {
            get => KeyMod.HasFlag(KeyMod.LShift)
                || KeyMod.HasFlag(KeyMod.RShift);
        }

        public bool Ctrl
        {
            get => KeyMod.HasFlag(KeyMod.LCtrl)
                || KeyMod.HasFlag(KeyMod.RCtrl);
        }

        public bool Alt
        {
            get => KeyMod.HasFlag(KeyMod.LAlt)
                || KeyMod.HasFlag(KeyMod.RAlt);
        }
    }

    public class WindowEvent : Event
    {
        internal WindowEvent(GenericEvent genericEvent) : base(genericEvent) {}
        private InternalWindowEvent Event { get => GenericEvent.window; }

        public WindowEventType WindowEventType { get => Event.windowEventType; }
        public Window Window { get => new Window(Event.windowID, false); }
    }
}
