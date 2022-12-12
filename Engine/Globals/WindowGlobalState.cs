using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Globals;

public static class WindowGlobalState {
  private static double s_time;
  private static MouseState? s_mouseState;
  private static KeyboardState? s_keyboardState;
  private static bool s_cursorVisible = true;
  private static Cursor s_cursor;
  private static CursorState s_cursorState = CursorState.Normal;
  private static Vector2 s_lastLockedCursorPosition = new Vector2(0, 0);
  private static Windowing.Window? s_window;

  public static void SetWindow(Windowing.Window window) {
    s_window = window;
  }

  public static void SetTime(double time) {
    s_time = time;
  }

  public static void SetMouseState(MouseState mouseState) {
    s_mouseState = mouseState;
  }

  public static void SetKeyboardState(KeyboardState keyboardState) {
    s_keyboardState = keyboardState;
  }

  public static void SetCursorVisible(bool value) {
    s_cursorVisible = value;
  }

  public static void SetCursor(Cursor cursor) {
    s_cursor = cursor;
  }

  public static void SetCursorState(CursorState cursorState) {
    s_cursorState = cursorState;
  }

  public static void SetCursorLastLockedPosition(Vector2 position) {
    s_lastLockedCursorPosition = position;
  }

  public static Windowing.Window GetWindow() {
    return s_window;
  }

  public static double GetTime() {
    return s_time;
  }

  public static MouseState GetMouseState() {
    if (s_mouseState == null) {
      return null!;
    }
    return s_mouseState;
  }

  public static KeyboardState GetKeyboardState() {
    if (s_keyboardState == null) {
      return null!;
    }
    return s_keyboardState;
  }

  public static CursorState GetCursorState() {
    return s_cursorState;
  }

  public static bool GetCursorVisible() {
    return s_cursorVisible;
  }

  public static bool GetCursorGrabbed() {
    if (s_cursorVisible) {
      return false;
    }
    return true;
  }

  public static Cursor GetCursor() {
    return s_cursor;
  }

  public static Vector2 GetCursorLastLockedPosition() {
    return s_lastLockedCursorPosition;
  }
}