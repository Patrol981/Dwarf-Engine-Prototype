using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace Voxelized.Callbacks;

public static class KeyCallback {
  unsafe public static void OnKeyPressed(Window* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) {
    Console.WriteLine(key);
    Console.WriteLine(scanCode);

    if(key == Keys.Escape) {
      GLFW.Terminate();
    }
  }
}