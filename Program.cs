using OpenTK.Windowing.GraphicsLibraryFramework;
using Dwarf.Engine;

class Program {
  unsafe static void Main() {
    var engine = new EngineClass();
    engine.Run();

    GLFW.Terminate();
  }
}

