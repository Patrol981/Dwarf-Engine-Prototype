using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Voxelized.Windowing;
using Voxelized.Globals;
using Voxelized.Callbacks;

class Program {
  unsafe static void Main() {
    using (var window = new Voxelized.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings())) {
      // GLFW.SetKeyCallback(window.WindowPtr, KeyCallback.OnKeyPressed);
      // GLFW.SetMouseButtonCallback(window.WindowPtr, MouseCallback.OnMousePressed);
      window.Run();
    }

    GLFW.Terminate();
  }
}

