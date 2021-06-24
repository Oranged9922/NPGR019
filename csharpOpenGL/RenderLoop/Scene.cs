using csharpOpenGL.Rendering.Display;
using GLFW;
using System;
using static csharpOpenGL.OpenGL.GL;
using csharpOpenGL.Rendering.Shaders;
using csharpOpenGL.Rendering.Cameras;
using System.Numerics;

namespace csharpOpenGL.RenderLoop
{
    abstract class Scene
    {

        protected int InitialWindowWidth { get; set; }
        protected int InitialWindowHeight { get; set; }
        protected string InitialWindowTitle { get; set; }
        public Scene(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle)
        {
            InitialWindowWidth = initialWindowWidth;
            InitialWindowHeight = initialWindowHeight;
            InitialWindowTitle = initialWindowTitle;
        }

        public void Run()
        {
            Initialize();
            DisplayManager.CreateWindow(InitialWindowWidth, InitialWindowHeight, InitialWindowTitle);
            LoadContent();

            while (!Glfw.WindowShouldClose(DisplayManager.Window))
            {

                SceneTime.DeltaTime = (float)Glfw.Time - SceneTime.TotalElapsedSeconds;
                SceneTime.TotalElapsedSeconds = (float)Glfw.Time;

                Update();
                Glfw.PollEvents();
                Render();
            }
        }

        protected abstract void Initialize();
        protected abstract void LoadContent();
        protected abstract void Update();
        protected abstract void Render();

    }

    class TestScene : Scene
    {

        uint vao;
        uint vbo;
        Shader shader;
        Camera2D cam;
        public TestScene(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle) : base(initialWindowWidth, initialWindowHeight, initialWindowTitle)
        {
        }

        protected override void Initialize()
        {
        
        }

        protected unsafe override void LoadContent()
        {
            string vertexShader = @"#version 460 core
                                    layout (location = 0) in vec2 aPosition;
                                    layout (location = 1) in vec3 aColor;

                                    out vec4 vertexColor;
                                    uniform mat4 projection;
                                    uniform mat4 model;
    
                                    void main() 
                                    {
                                        vertexColor = vec4(aColor.rgb, 1.0);
                                        gl_Position = projection * model * vec4(aPosition.xy, 0, 1.0);
                                    }";

            string fragmentShader = @"#version 460 core
                                    out vec4 FragColor;
                                    in vec4 vertexColor;

                                    void main() 
                                    {
                                        FragColor = vertexColor;
                                    }";

            shader = new Shader(vertexShader, fragmentShader);
            shader.Load();

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            float[] vertices =
            {
                -0.5f, 0.5f, 1f, 0f, 0f, // top left
                0.5f, 0.5f, 0f, 1f, 0f,// top right
                -0.5f, -0.5f, 0f, 0f, 1f, // bottom left

                0.5f, 0.5f, 0f, 1f, 0f,// top right
                0.5f, -0.5f, 0f, 1f, 1f, // bottom right
                -0.5f, -0.5f, 0f, 0f, 1f, // bottom left
            };
            // pointer to array
            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }

            glVertexAttribPointer(0, 2, GL_FLOAT, false, 5 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glVertexAttribPointer(1, 3, GL_FLOAT, false, 5 * sizeof(float), (void*)(2 * sizeof(float)));
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            cam = new(DisplayManager.WindowSize/2f, 2.5f);
        }

        protected override void Render()
        {
            glClearColor(0,0,0,0);
            glClear(GL_COLOR_BUFFER_BIT);

            Vector2 position = new(400, 300);
            Vector2 scale = new(150, 100);
            float rotation = MathF.Sin(SceneTime.TotalElapsedSeconds)*MathF.PI*2f;

            Matrix4x4 trans = Matrix4x4.CreateTranslation(position.X, position.Y, 0);
            Matrix4x4 sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation);

            shader.SetMatrix4x4("model", sca * rot * trans);

            shader.Use();
            shader.SetMatrix4x4("projection", cam.GetProjectionMatrix());

            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 6);
            glBindVertexArray(0);
            Glfw.SwapBuffers(DisplayManager.Window);
        }

        protected override void Update()
        {
        
        }
    }
}
