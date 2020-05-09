using System;
using System.Runtime.InteropServices;
using System.Text;
using Color = System.Drawing.Color;

namespace SDL
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct InternalColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public InternalColor (Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }
    }

    internal enum Bool
    {
        False = 0,
        True = 1,
    }

    internal static class Util
    {
        public static unsafe string ToString(IntPtr s)
        {
            if (s == IntPtr.Zero) return null;

            byte* ptr = (byte*) s;
            while (*ptr != 0) ptr++;

            // TODO: Update .NET and do this:
            // return Encoding.UTF8.GetString(
            //     (byte*) s, (int) (ptr - (byte*) s));

            int len = (int) (ptr - (byte*) s);
            if (len == 0) return string.Empty;

            char* chars = stackalloc char[len];
            int strLen = System.Text.Encoding.UTF8.GetChars(
                (byte*) s, len, chars, len);

            return new string(chars, 0, strLen);
        }

        public static byte[] FromString(String s) =>
            Encoding.UTF8.GetBytes(s + '\0');
    }
}
