using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;

namespace Dwarf.Engine.Globals;

public static class GuiDataState {
  private static Entity _targetEntity = null!;
  private static bool _usingDebugWindow = false;

  public static void SetEntity(Entity entity) {
    _targetEntity = entity;
  }

  public static void SetUsingDebugWindow(bool value) {
    _usingDebugWindow = value;
  }

  public static bool GetUsingDebugWindow() {
    return _usingDebugWindow;
  }
}