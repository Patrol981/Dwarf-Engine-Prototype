using Voxelized.DataStructures;

namespace Voxelized.ECS;

public abstract class Component {
  public Entity? Owner = null;
}