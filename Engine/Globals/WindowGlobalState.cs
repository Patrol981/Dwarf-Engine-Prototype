using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Voxelized.Globals;

public static class WindowGlobalState {
  private static double _time;
  private static MouseState? _mouseState;
  private static KeyboardState? _keyboardState;

  public static void SetTime(double time) {
    _time = time;
  }

  public static void SetMouseState(MouseState mouseState) {
    _mouseState = mouseState;
  }

  public static void SetKeyboardState(KeyboardState keyboardState) {
    _keyboardState = keyboardState;
  }

  public static double GetTime() {
    return _time;
  }

  public static MouseState GetMouseState() {
    if(_mouseState == null) {
      return null!;
    }
    return _mouseState;
  }

  public static KeyboardState GetKeyboardState() {
    if(_keyboardState == null) {
      return null!;
    }
    return _keyboardState;
  }
}