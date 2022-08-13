using Dwarf.Engine.DataStructures;

namespace Dwarf.Engine.ECS;

public abstract class Component {
  public Entity? Owner = null;
}