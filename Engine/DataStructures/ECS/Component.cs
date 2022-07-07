using Voxelized.Engine.DataStructures;

namespace Voxelized.Engine.ECS;

public abstract class Component {
  public Entity? Owner = null;
}