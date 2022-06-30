using Voxelized.ECS;
using Voxelized.DataStructures;

namespace Voxelized.Loaders;

public abstract class MeshLoader {
  public virtual Mesh Load(string path) {
    return null!;
  }
}