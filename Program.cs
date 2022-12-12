using OpenTK.Windowing.GraphicsLibraryFramework;
using Dwarf.Engine;
using Dwarf.Engine.Globals;

class Program {
  unsafe static void Main() {
    var engine = new EngineClass();
    engine.Run();

    GLFW.Terminate();
  }
}

