using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Dwarf.Engine.Windowing;
using Dwarf.Engine.Globals;
using Dwarf.Callbacks;
using Dwarf.Engine;
using Dwarf.Engine.Scenes;

class Program {
  unsafe static void Main() {
    /*
    using (var window = new Dwarf.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings())) {
      // GLFW.SetKeyCallback(window.WindowPtr, KeyCallback.OnKeyPressed);
      // GLFW.SetMouseButtonCallback(window.WindowPtr, MouseCallback.OnMousePressed);
      window.Run();
    }
    */

    var engine = new EngineClass();
    engine.Run();

    GLFW.Terminate();
  }
}

