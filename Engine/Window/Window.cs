using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Globals;
using Voxelized.Shaders;

using Voxelized.Challanges;

namespace Voxelized.Windowing;

public class Window : GameWindow {
  private int _vbo, _vao;
  private Shader? _shader;
  private TriangleTest? _triangleTest;
  private Ep2C2? ep2c2;
  private Ep2C3? ep2c3;
  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings) {}

  protected unsafe override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    GLFW.PollEvents();
  }

  protected override unsafe void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);

    GL.ClearColor(250.0f / 255.0f, 119.0f / 255.0f, 110.0f / 255.0f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit);

    // _triangleTest!.Update();
    // ep2c2!.Update();
    ep2c3!.Update();

    GLFW.SwapBuffers(this.WindowPtr);
  }

  protected override void OnResize(ResizeEventArgs e) {
    base.OnResize(e);
    GL.Viewport(0, 0, WindowSettings.GetNativeWindowSettings().Size.X, WindowSettings.GetNativeWindowSettings().Size.Y);
  }

  protected override void OnLoad() {
    base.OnLoad();

    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    // _triangleTest = new TriangleTest(_shader);
    // ep2c2 = new Ep2C2(_shader);
    ep2c3 = new Ep2C3(_shader);
  }

  protected override void OnUnload() {
    base.OnUnload();
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.DeleteBuffer(_vbo);
    _shader!.Dispose();
  }
}