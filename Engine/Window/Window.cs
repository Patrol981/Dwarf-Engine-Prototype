using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Dwarf.Engine.Globals;
using Dwarf.Engine.GUI;

namespace Dwarf.Engine.Windowing;

public delegate void onEventCallback();

public class Window : GameWindow {
  private double _time;
  private onEventCallback? _onUpdate;
  private onEventCallback? _onRender;
  private onEventCallback? _onDrawGUI;
  private onEventCallback? _onResize;
  private onEventCallback? _onLoad;

  private GUIController _controller;

  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings) { }


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

  public void BindOnLoadCallback(onEventCallback callback) {
    _onLoad = callback;
  }

  public void Clear() {
    GL.ClearColor(250.0f / 255.0f, 119.0f / 255.0f, 110.0f / 255.0f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
  }

  public void Clear(Vector3 color) {
    GL.ClearColor(color.X, color.Y, color.Z, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.StencilBufferBit);
  }

  protected unsafe override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    _onUpdate?.Invoke();

    GLFW.PollEvents();
  }

  protected override unsafe void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);

    //if(!IsFocused) {
    //  return;
    //}

    switch (WindowGlobalState.GetCursorVisible()) {
      case true:
        //CursorVisible = WindowGlobalState.GetCursorVisible();
        CursorState = CursorState.Normal;
        break;
      case false:
        //CursorGrabbed = WindowGlobalState.GetCursorGrabbed();
        CursorState = CursorState.Grabbed;
        break;
    }

    WindowGlobalState.SetTime(args.Time);
    _time += 4.0f * args.Time;

    Clear();
    //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    _onRender?.Invoke();

    _controller.Update(this, (float)args.Time);
    _onDrawGUI?.Invoke();
    _controller.Render();

    GLFW.SwapBuffers(this.WindowPtr);
  }

  protected override void OnResize(ResizeEventArgs e) {
    base.OnResize(e);
    _onResize?.Invoke();
    var oldSettings = WindowSettings.GetNativeWindowSettings();
    var newSettings = new NativeWindowSettings {
      Size = new Vector2i(e.Size.X, e.Size.Y),
      Title = oldSettings.Title,
      Flags = oldSettings.Flags
    };
    WindowSettings.SetNativeWindowSettings(newSettings);
    GL.Viewport(0, 0, WindowSettings.GetNativeWindowSettings().Size.X, WindowSettings.GetNativeWindowSettings().Size.Y);
    _controller.WindowResized(ClientSize.X, ClientSize.Y);
  }

  protected override void OnLoad() {
    base.OnLoad();

    if (Debugging.Debugger.UseDebbuger) {
      GL.DebugMessageCallback(Debugging.Debugger.DebugMessageDelegate, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    // Clipping
    GL.Enable(EnableCap.DepthTest);
    // Bounding Boxes
    GL.Enable(EnableCap.PolygonOffsetFill);
    GL.PolygonOffset(1, 0);
    // Skybox
    GL.Disable(EnableCap.Blend);
    WindowGlobalState.SetMouseState(MouseState);
    WindowGlobalState.SetKeyboardState(KeyboardState);

    _controller = new GUIController(ClientSize.X, ClientSize.Y);

    _onLoad?.Invoke();
  }

  protected override void OnUnload() {
    base.OnUnload();
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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