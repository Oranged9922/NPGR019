using Silk.NET.OpenGL;

namespace _01_triangle;

public class ColoredTriangle : IVariationImplementation
{
    public string VertexShaderSource => @"
#version 440 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec3 vertexColor; // Variable to transfer color to the fragment shader

void main() {
	gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
	vertexColor = aColor; // Pass color to the fragment shader
}
";

    public string FragmentShaderSource => @"
#version 440 core

in vec3 vertexColor; // Color from the vertex shader

out vec4 fragColor;

void main() {
	fragColor = vec4(vertexColor, 1.0);
}
";

    public Geometry CreateGeometry(GL gl)
    {
        Geometry geometry = new()
        {
            VAO = gl.GenVertexArray(),
            VBO = gl.GenBuffer()
        };

        Vertex[] vertices =
 [
    new(-0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f),
    new( 0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f),
    new( 0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f)
];

        gl.BindVertexArray(geometry.VAO);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, geometry.VBO);
        unsafe
        {
            fixed (void* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);
            }
            // Position attribute: first 3 floats.
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)sizeof(Vertex), 0);
            gl.EnableVertexAttribArray(0);

            // Color attribute: next 3 floats.
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, (uint)sizeof(Vertex), (void*)(sizeof(float) * 3));
            gl.EnableVertexAttribArray(1);
        }
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindVertexArray(0);

        geometry.Size = vertices.Length;
        return geometry;
    }
}