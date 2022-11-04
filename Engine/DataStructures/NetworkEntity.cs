namespace Dwarf.Engine.DataStructures;
public class NetworkEntity : Entity {
  private bool _spawned = false;

  public bool Spawned { 
    get { return _spawned; }
    set { _spawned = value; } 
  }
}
