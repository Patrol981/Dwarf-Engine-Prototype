using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;


using Voxelized.Globals;
using Voxelized.GUI;

namespace Voxelized.Windowing;

public delegate void onEventCallback();

public class Window : GameWindow {
  private double _time;
  private onEventCallback? _onUpdate;
  private onEventCallback? _onRender;
  private onEventCallback? _onDrawGUI;
  private onEventCallback? _onResize;

  private GUIController _controller;

  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings) {}


  public void BindUpdateCallback(onEventCallback callback) {
    _onUpdate = callback;
  }
  public void BindRenderCallback(onEventCallback callback) {
    _onRender = callback;
  }
  public void BindDrawGUICallback(onEventCallback callback) {
    _onDrawGUI = callback;
  }
  public void BindResizeCallback(onEventCallback callback) {
    _onResize = callback;
  }

  public void Clear() {
    GL.ClearColor(250.0f / 255.0f, 119.0f / 255.0f, 110.0f / 255.0f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
  }

  public void Clear(Vector3 color) {
    GL.ClearColor(color.X, color.Y, color.Z, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
  }

  protected unsafe override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    _onUpdate?.Invoke();

    GLFW.PollEvents();
  }

  protected override unsafe void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);

    if(!IsFocused) {
      return;
    }

    switch(WindowGlobalState.GetCursorVisible()) {
      case true:
        CursorVisible = WindowGlobalState.GetCursorVisible();
        break;
      case false:
        // CursorVisible = WindowGlobalState.GetCursorVisible();
        CursorGrabbed = WindowGlobalState.GetCursorGrabbed();
        break;
    }

    WindowGlobalState.SetTime(args.Time);
    _time += 4.0f * args.Time;

    Clear();
    //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    _onRender?.Invoke();

    _controller.Update(this, (float)args.Time);
    _onDrawGUI?.Invoke();
    // ImGuiNET.ImGui.ShowDemoWindow();
    _controller.Render();

    GLFW.SwapBuffers(this.WindowPtr);
  }

  protected override void OnResize(ResizeEventArgs e) {
    base.OnResize(e);
    _onResize?.Invoke();
    GL.Viewport(0, 0, WindowSettings.GetNativeWindowSettings().Size.X, WindowSettings.GetNativeWindowSettings().Size.Y);
    _controller.WindowResized(ClientSize.X, ClientSize.Y);
  }

  protected override void OnLoad() {
    base.OnLoad();

    GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    GL.Enable(EnableCap.DepthTest);
    WindowGlobalState.SetMouseState(MouseState);
    WindowGlobalState.SetKeyboardState(KeyboardState);
    WindowGlobalState.SetCursorVisible(false);

   // _controller = new GUIController(Size.X, Size.Y);
   _controller = new GUIController(ClientSize.X, ClientSize.Y);

    // CursorGrabbed = WindowGlobalState.GetCursorVisible();
  }

  protected override void OnUnload() {
    base.OnUnload();
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    // GL.DeleteBuffer(_vbo);
    // _shader!.Dispose();
  }

  protected override void OnMouseMove(MouseMoveEventArgs e) {
    base.OnMouseMove(e);
  }

  protected override void OnMouseWheel(MouseWheelEventArgs e) {
    base.OnMouseWheel(e);
    _controller.MouseScroll(e.Offset);
  }

  protected override void OnTextInput(TextInputEventArgs e) {
    base.OnTextInput(e);
    _controller.PressChar((char)e.Unicode);
  }
}