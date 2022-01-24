using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Voxelized.Globals;

namespace Voxelized.Callbacks;

public static class KeyCallback {

  public static void OnPressed() {
    Console.WriteLine("elo");
  }
  unsafe public static void OnKeyPressed(Window* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) {
    // Console.WriteLine(key);
    // Console.WriteLine(scanCode);

    if(key == Keys.Escape) {
      GLFW.Terminate();
    }

    // CameraHandle(window, key, scanCode, action, mods);
  }

  private unsafe static void CameraHandle(Window* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) {
    var camera = CameraGlobalState.GetCamera();
    if(key == Keys.W) {
      camera.Position += camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(key == Keys.S) {
      camera.Position -= camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(key == Keys.A) {
      camera.Position -= camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(key == Keys.D) {
      camera.Position += camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
  }
}