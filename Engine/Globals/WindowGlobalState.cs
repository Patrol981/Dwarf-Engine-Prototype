using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Globals;

public static class WindowGlobalState {
  private static double _time;
  private static MouseState? _mouseState;
  private static KeyboardState? _keyboardState;
  private static bool _cursorVisible = true;
  private static Cursor _cursor;
  private static CursorState _cursorState = CursorState.Normal;
  private static Vector2 _lastLockedCursorPosition = new Vector2(0,0);
  private static Windowing.Window? _window;

  public static void SetWindow(Windowing.Window window) {
    _window = window;
  }

  public static void SetTime(double time) {
    _time = time;
  }

  public static void SetMouseState(MouseState mouseState) {
    _mouseState = mouseState;
  }

  public static void SetKeyboardState(KeyboardState keyboardState) {
    _keyboardState = keyboardState;
  }

  public static void SetCursorVisible(bool value) {
    _cursorVisible = value;
  }

  public static void SetCursor(Cursor cursor) {
    _cursor = cursor;
  }

  public static void SetCursorState(CursorState cursorState) {
    _cursorState = cursorState;
  }

  public static void SetCursorLastLockedPosition(Vector2 position) {
    _lastLockedCursorPosition = position;
  }

  public static Windowing.Window GetWindow() {
    return _window;
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

  public static CursorState GetCursorState() {
    return _cursorState;
  }

  public static bool GetCursorVisible() {
    return _cursorVisible;
  }

  public static bool GetCursorGrabbed() {
    if(_cursorVisible) {
      return false;
    }
    return true;
  }

  public static Cursor GetCursor() {
    return _cursor;
  }

  public static Vector2 GetCursorLastLockedPosition() {
    return _lastLockedCursorPosition;
  }
}