using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Globals;
using Voxelized.Shaders;
using Voxelized.Cameras;
using Voxelized.Callbacks;

using Voxelized.Challanges;

namespace Voxelized.Windowing;

public class Window : GameWindow {
  private Shader? _shader;
  private FreeCamera? _freeCam;

  private double _time;
  private Matrix4 _viewMatrix, _projectionMatrix;
  private CubeTest? cubeTest;
  private CubesTest? cubesTest;
  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings) {}

  protected unsafe override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    GLFW.PollEvents();
  }

  protected override unsafe void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);

    if(!IsFocused) {
      return;
    }

    WindowGlobalState.SetTime(args.Time);
    _time += 4.0f * args.Time;

    GL.ClearColor(250.0f / 255.0f, 119.0f / 255.0f, 110.0f / 255.0f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

    // _shader!.SetMatrix4("model", model);
    _shader!.SetMatrix4("view", _freeCam!.GetViewMatrix());
    _shader!.SetMatrix4("projection", _freeCam!.GetProjectionMatrix());

    _freeCam.HandleMovement();

    // cubeTest!.Update();
    cubesTest!.Update();

    GLFW.SwapBuffers(this.WindowPtr);
  }

  protected override void OnResize(ResizeEventArgs e) {
    base.OnResize(e);
    _freeCam!.AspectRatio = Size.X / (float)Size.Y;
    GL.Viewport(0, 0, WindowSettings.GetNativeWindowSettings().Size.X, WindowSettings.GetNativeWindowSettings().Size.Y);
  }

  protected override void OnLoad() {
    base.OnLoad();

    GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    GL.Enable(EnableCap.DepthTest);

    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    _viewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
    _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView
      (MathHelper.DegreesToRadians(45.0f), this.Size.X / (float) this.Size.Y, 0.1f, 100.0f);

    // cubeTest = new CubeTest(_shader);
    cubesTest = new CubesTest(_shader);

    _freeCam = new FreeCamera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
    CameraGlobalState.SetCamera(_freeCam);

    // KeyDown += new Action<KeyboardKeyEventArgs>(KeyCallback.OnPressed);
    WindowGlobalState.SetMouseState(MouseState);
    WindowGlobalState.SetKeyboardState(KeyboardState);

    CursorGrabbed = true;
  }

  protected override void OnUnload() {
    base.OnUnload();
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    // GL.DeleteBuffer(_vbo);
    _shader!.Dispose();
  }

  protected override void OnMouseMove(MouseMoveEventArgs e) {
    base.OnMouseMove(e);
  }
}