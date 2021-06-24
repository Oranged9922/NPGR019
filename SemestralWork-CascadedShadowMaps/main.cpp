
#include <cstdio>
#include <glad/glad.h>
#include <GLFW/glfw3.h>

GLuint vertexArrayObject = 0;
GLuint shaderProgram = 0;
GLFWwindow* window;

enum class WindowSize
{
    Width = 640,
    Height = 480
};

void resizeCallback(GLFWwindow* window, int wid, int hei)
{
    glViewport(0, 0, wid, hei);
}
void errorCallback(int error, const char* descr)
{
    printf("GLFW Error %i: %s\n", error, descr);
}
bool initOpenGL() 
{
    glfwSetErrorCallback(errorCallback);

    /* Initialize the library */
    if (!glfwInit())
        return false;

    // Request OpenGL 3.3 core profile upon window creation
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);


    // create window
    window = glfwCreateWindow((int)WindowSize::Width, (int)WindowSize::Height, "Lukáš Salak - Cascaded Shadow Maps", nullptr, nullptr);
    if (window == nullptr)
    {
        printf("Failed to create the GLFW window!");
        return false;
    }

    // set created window as current for this thread
    glfwMakeContextCurrent(window);

    // check if glad is loaded properly
    if (!gladLoadGL())
    {
        printf("GLAD not loaded properly.\n");
        return false;
    }

    // window resize callback
    glfwSetFramebufferSizeCallback(window, resizeCallback);

    // set viewport
    glViewport(0, 0, (int)WindowSize::Width, (int)WindowSize::Height);
    return true;
}
void shutDown()
{
    glDeleteVertexArrays(1, &vertexArrayObject);

    glDeleteProgram(shaderProgram);
    glfwDestroyWindow(window);
    glfwTerminate();
}

void mainLoop()
{
    /* Loop until the user closes the window */
    while (!glfwWindowShouldClose(window))
    {
        /* Render here */
        glClear(GL_COLOR_BUFFER_BIT);

        /* Swap front and back buffers */
        glfwSwapBuffers(window);

        /* Poll for and process events */
        glfwPollEvents();
    }
}
int main(void)
{

    if (!initOpenGL())
    {
        printf("Failed to initialize OpenGL.\n");
        shutDown();
        return -1;
    }

    mainLoop();

    glfwTerminate();
    return 0;
}