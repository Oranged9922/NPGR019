using Silk.NET.OpenGL;

namespace _01_triangle
{
    internal class SimpleTriangle : IVariationImplementation
    {
        public string VertexShaderSource => @"
#version 440 core

layout (location = 0) in vec3 aPos;

void main() {
	gl_Position = vec4(aPos, 1.0);
}
";

        public string FragmentShaderSource => @"
#version 440 core

out vec4 fragColor;

void main() {
	fragColor = vec4(1.0f, 0.0f, 0.0f, 1.0f);
}
";

        public Geometry CreateGeometry(GL gl)
        {
            var geometry = new Geometry();

            float[] vertices =
            [
                -0.5f, -0.5f, 0.0f, // left
                0.5f, -0.5f, 0.0f, // right
                0.0f, 0.5f, 0.0f // top
            ];

            geometry.VAO = gl.GenVertexArray();
            geometry.VBO = gl.GenBuffer();

            gl.BindVertexArray(geometry.VAO);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, geometry.VBO);

            // In C#, we have to use unsafe code to get the address of the first element of the array
            unsafe
            {
                fixed (void* v = &vertices[0])
                {
                    gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
                }
            }

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            geometry.Size = vertices.Length / 3;
            return geometry;
        }
    }
}