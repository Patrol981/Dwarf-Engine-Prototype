using Voxelized.Engine.ECS;
using Voxelized.Engine.DataStructures;

namespace Voxelized.Engine.Loaders;

public abstract class MeshLoader {
  public virtual MasterMesh Load(string path) {
    return null!;
  }
}