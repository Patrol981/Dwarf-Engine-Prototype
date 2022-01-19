using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxelized.Callbacks;

public static class MouseCallback {
  unsafe public static void OnMousePressed(Window* window, MouseButton button, InputAction action, KeyModifiers mods) {
    Console.WriteLine(button);
    Console.WriteLine(action);
  }
}