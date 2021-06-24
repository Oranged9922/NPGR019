using csharpOpenGL.RenderLoop;

namespace csharpOpenGL
{
    class Program
    {
        public static void Main(string[] args)
        {

            Scene scene = new TestScene(800, 600, "Hey");
            scene.Run();
        }
    }
}