using Voxelized.DataStructures;
using Voxelized.ECS;

namespace Voxelized.Globals;

public static class GuiDataState {
  private static Entity _targetEntity = null!;
  private static bool _usingDebugWindow = false;

  public static void SetEntity(Entity entity) {
    _targetEntity = entity;
  }

  public static void SetUsingDebugWindow(bool value) {
    _usingDebugWindow = value;
  }

  public static Entity GetEntity() {
    if(_targetEntity == null) {
      _targetEntity = new Entity();
      _targetEntity.SetName("No Entity Specified");
    }
    return _targetEntity;
  }

  public static bool GetUsingDebugWindow() {
    return _usingDebugWindow;
  }
}