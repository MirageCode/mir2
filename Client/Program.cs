using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Client
{
    internal static class Program
    {
        public static CMain Form;

        public static bool Restart;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.ToLower() == "-tc") Settings.UseTestConfig = true;
                }
            }

            #if DEBUG
                Settings.UseTestConfig = true;
            #endif

            try
            {
                Packet.IsServer = false;
                Settings.Load();

                Form = new CMain();
                Form.Run();

                Settings.Save();
                CMain.InputKeys.Save();
            }
            catch (Exception ex)
            {
                CMain.SaveError(ex.ToString());
            }
        }
    }
}
