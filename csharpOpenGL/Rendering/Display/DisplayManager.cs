using GLFW;
using System.Numerics;
using System.Drawing;
using static csharpOpenGL.OpenGL.GL;

namespace csharpOpenGL.Rendering.Display
{
    static class DisplayManager
    {
        public static Window Window { get; set; }
        public static Vector2 WindowSize { get; set; }

        public static void CreateWindow(int width, int height, string title)
        {
            WindowSize = new Vector2(width, height);

            Glfw.Init();
            SetHints(versionMajor : 4, versionMinor : 6);

            // Create window 

            Window = Glfw.CreateWindow(width, height, title, Monitor.None, Window.None);
            if(Window == Window.None)
            {
                // we fd up
                return;
            }

            Rectangle screen = Glfw.PrimaryMonitor.WorkArea;
            var x = (screen.Width - width) / 2;
            var y = (screen.Height - height) / 2;

            Glfw.SetWindowPosition(Window, x, y);

            Glfw.MakeContextCurrent(Window);
            Import(Glfw.GetProcAddress);

            // set viewport

            glViewport(0, 0, width, height);

            Glfw.SwapInterval(0); // vsync off

        }

        public static void CloseWindow()
        {
            Glfw.Terminate();
        }

        private static void SetHints(
            int versionMajor = 4,
            int versionMinor = 6,
            Profile openglProfile = Profile.Core,
            bool focused = true,
            bool resizable = true)
        {
            Glfw.WindowHint(Hint.ContextVersionMajor, 4);
            Glfw.WindowHint(Hint.ContextVersionMinor, 6);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Focused, true);
            Glfw.WindowHint(Hint.Resizable, true);

        }
    }

}
