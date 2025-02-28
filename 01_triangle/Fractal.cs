using Silk.NET.OpenGL;

namespace _01_triangle;

public class Fractal : IVariationImplementation
{
    public string VertexShaderSource => @"
#version 440 core
layout (location = 0) in vec2 aPos;
out vec2 texCoord;
void main() {
    gl_Position = vec4(aPos, 0.0, 1.0);
    texCoord = aPos;
}
";

    public string FragmentShaderSource => @"
#version 440 core
vec4 randomColor(int seed) {
    float r = float((seed * 315 + 714) % 255) / 255.0;
    float g = float((seed * 861 + 346) % 255) / 255.0;
    float b = float((seed * 123 + 1068) % 255) / 255.0;
    return vec4(r, g, b, 1.0);
}
in vec2 texCoord;
out vec4 fragColor;
uniform vec2 seed = vec2(-0.8, 0.156);
const int maxIter = 100;
void main() {
    int i = 0;
    vec2 z = vec2(3.0 * texCoord.x, 2.0 * texCoord.y);
    for(; i < maxIter; ++i) {
        float x = (z.x * z.x - z.y * z.y) + seed.x;
        float y = (z.y * z.x + z.x * z.y) + seed.y;
        if((x * x + y * y) > 4.0) {
            break;
        }
        z = vec2(x, y);
    }
    fragColor = randomColor(i);
}
";

    public Geometry CreateGeometry(GL gl)
    {
        Geometry geometry = new()
        {
            VAO = gl.GenVertexArray(),
            VBO = gl.GenBuffer()
        };

        // Two triangles forming a full-screen quad (6 vertices)
        float[] vertices =
        [
                -1.0f, -1.0f,
                -1.0f,  1.0f,
                 1.0f,  1.0f,

                 1.0f,  1.0f,
                 1.0f, -1.0f,
                -1.0f, -1.0f,
        ];

        gl.BindVertexArray(geometry.VAO);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, geometry.VBO);

        unsafe
        {
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint)(vertices.Length * sizeof(float)),
                    v,
                    BufferUsageARB.StaticDraw);
            }

            // Set vertex attribute 0 to read vec2 data (2 floats per vertex)
            gl.VertexAttribPointer(0, 2, GLEnum.Float, false, (uint)(2 * sizeof(float)), (void*)0);
            gl.EnableVertexAttribArray(0);

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            gl.BindVertexArray(0);
        }

        geometry.Size = vertices.Length / 2;
        return geometry;
    }
}