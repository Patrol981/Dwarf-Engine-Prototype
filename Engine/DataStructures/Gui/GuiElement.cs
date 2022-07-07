namespace Voxelized.Engine.DataStructures;

public abstract class GuiElement {
  private string _name;

  public void SetName(string name) {
    _name = name;
  }

  public string GetName() {
    return _name;
  }
}