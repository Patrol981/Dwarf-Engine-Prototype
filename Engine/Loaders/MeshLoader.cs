using Voxelized.Engine.ECS;
using Voxelized.Engine.DataStructures;

namespace Voxelized.Engine.Loaders;

public abstract class MeshLoader {
  public virtual Mesh Load(string path) {
    return null!;
  }
}