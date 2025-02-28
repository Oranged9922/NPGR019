using Silk.NET.OpenGL;

namespace _01_triangle
{
    internal interface IVariationImplementation
    {
        string VertexShaderSource { get; }
        string FragmentShaderSource { get; }

        Geometry CreateGeometry(GL gl);
    }
}