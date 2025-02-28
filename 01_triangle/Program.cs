using Silk.NET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;

namespace _01_triangle;
public class Program 
{
    enum Variation
    {
        SimpleTriangle,
        ColoredTriangle,
        Fractal
    }

    static readonly Dictionary<Variation, IVariationImplementation> Implementations = new()
    {
        { Variation.SimpleTriangle, new SimpleTriangle() },
        { Variation.ColoredTriangle, new ColoredTriangle() },
        { Variation.Fractal, new Fractal() }
    };
    static GL? gl;
    static uint shaderProgram;
    static Geometry geometry;
    static int seedLocation = -1;
    static double timeElapsed;
    public static void Main()
    {


        // Change Variation to test different implementations
        //----------------------------------------
/*-->*/ var variation = Variation.ColoredTriangle;
        //----------------------------------------


        var implementation = Implementations[variation];

        // Init window (Equivalent to glfwInit)
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1920, 1080);
        options.Title = "01 - Triangle";
        options.API = new GraphicsAPI()
        {
            API = ContextAPI.OpenGL,
            Profile = ContextProfile.Core,
            Version = new APIVersion(4, 4),
            Flags = ContextFlags.Debug
        };

        var window = Window.Create(options);
        window.Initialize();
        gl = GL.GetApi(window);

        shaderProgram = CreateShaderProgram(gl, implementation.VertexShaderSource, implementation.FragmentShaderSource);
        if (variation == Variation.Fractal)
        {
            seedLocation = gl.GetUniformLocation(shaderProgram, "seed");
            timeElapsed = 0;
        }
        geometry = implementation.CreateGeometry(gl);

        window.Render += OnRender;

        window.Run();
    }

    private static void OnRender(double obj)
    {

        gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit);

        gl.UseProgram(shaderProgram);
        if (seedLocation != -1)
        {
            timeElapsed += obj;
            Utils.UpdateSeed(gl, timeElapsed, seedLocation);
        }
        geometry.BindVertexArrays(gl);
        gl.DrawArrays(GLEnum.Triangles, 0, (uint)geometry.Size);
    }

    private static uint CreateShaderProgram(GL gl, string vertexShader, string fragmentShader)
    {
        uint program = gl.CreateProgram();
        uint vs = CompileShader(gl, ShaderType.VertexShader, vertexShader);
        uint fs = CompileShader(gl, ShaderType.FragmentShader, fragmentShader);

        gl.AttachShader(program, vs);
        gl.AttachShader(program, fs);
        gl.LinkProgram(program);
        gl.ValidateProgram(program);

        gl.DeleteShader(vs);
        gl.DeleteShader(fs);

        return program;
    }

    private static uint CompileShader(GL gl, ShaderType type, string source)
    {
        uint id = gl.CreateShader(type);
        gl.ShaderSource(id, source);
        gl.CompileShader(id);

        // Check for errors
        gl.GetShader(id, ShaderParameterName.CompileStatus, out _);

        //Checking the shader for compilation errors.
        string infoLog = gl.GetShaderInfoLog(id);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling vertex shader {infoLog}");
        }

        return id;
    }
}


public record struct Geometry(uint VAO, uint VBO, int Size) 
{ 
    public readonly void BindVertexArrays(GL gl) => gl.BindVertexArray(VAO); 
}


/// In C#, we unfortunately cannot have float[], because they are pointers to memory, meaning the code
/// public struct Vertex(float[] Position, float[] Color); would not work, because Vertex object would store only two pointers to memory. <summary>
/// We also have to explicitly define the layout of the struct in memory, so that the C# struct matches the layout we need.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vertex(float x, float y, float z, float r, float g, float b)
{
    public float X = x, Y = y, Z = z;
    public float R = r, G = g, B = b;
}

public static class Utils
{
    public static double FloatMod(double a, double b)
    {
        double d = a / b;
        return (d - Math.Round(d)) * b;
    }

    /// <summary>
    /// Updates the uniform seed in the shader.
    /// </summary>
    /// <param name="gl">The GL instance.</param>
    /// <param name="time">Time value (can be elapsed time).</param>
    /// <param name="seedLocation">Location of the 'seed' uniform.</param>
    public static void UpdateSeed(GL gl, double time, int seedLocation)
    {
        float v1 = (float)(FloatMod(time / 3.346, 2.00) - 1.0);
        float v2 = (float)(FloatMod(time / 7.2135, 2.00) - 1.0);
        gl.Uniform2(seedLocation, v1, v2);
    }
}