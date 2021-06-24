using GLFW;
using System.Numerics;
using static csharpOpenGL.OpenGL.GL;

namespace csharpOpenGL.Rendering.Shaders
{
    class Shader
    {
        string vertexCode;
        string fragmentCode;
        public uint ProgramID { get; set; }


        public Shader(string vertexCode, string fragmentCode)
        {
            this.vertexCode = vertexCode;
            this.fragmentCode = fragmentCode;
        }

        public void Load()
        {
            uint vs, fs;

            vs = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vs, vertexCode);
            glCompileShader(vs);

            int[] status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);
            if(status[0] == 0)
            {
                // failed
                string err = glGetShaderInfoLog(vs);
                System.Console.WriteLine(err);
            }
            
            fs = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fs, fragmentCode);
            glCompileShader(fs);
            status = glGetShaderiv(fs, GL_COMPILE_STATUS, 1);
            if (status[0] == 0)
            {
                // failed
                string err = glGetShaderInfoLog(fs);
                System.Console.WriteLine(err);
            }

            ProgramID = glCreateProgram();
            glAttachShader(ProgramID, vs);
            glAttachShader(ProgramID, fs);

            glLinkProgram(ProgramID);


            glDetachShader(ProgramID, vs);
            glDetachShader(ProgramID, fs);
            glDeleteBuffer(vs);
            glDeleteBuffer(fs);
        }

        public void Use()
        {
            glUseProgram(ProgramID);
        }

        public void SetMatrix4x4(string uniformName, Matrix4x4 mat)
        {
            int location = glGetUniformLocation(ProgramID, uniformName);
            glUniformMatrix4fv(location, 1, false, GetMatrix4x4Values(mat));
        }

        private float[] GetMatrix4x4Values(Matrix4x4 m)
        {
            return new float[]
            {
        m.M11, m.M12, m.M13, m.M14,
        m.M21, m.M22, m.M23, m.M24,
        m.M31, m.M32, m.M33, m.M34,
        m.M41, m.M42, m.M43, m.M44
            };
        }
    }
}
