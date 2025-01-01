using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;

namespace SimpleGridFly
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Allocate a console window
            AllocConsole();
            Console.WriteLine("Console window is now visible!");

            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings
            {
                Size = new Vector2i(1280, 720),
                Title = "Silkroad Map Viewer"
            };

            using var program = new GridGame(gws, nws);
            program.Run();

            // Free the console window when the app closes (optional)
            FreeConsole();
        }
    }
}