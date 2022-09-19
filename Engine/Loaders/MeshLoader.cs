using Dwarf.Engine.ECS;
using Dwarf.Engine.DataStructures;

namespace Dwarf.Engine.Loaders;

public abstract class MeshLoader {
  public virtual MasterMesh Load(string path, bool useTextures) {
    return null!;
  }

  public virtual async Task<MasterMesh> LoadAsync(string path) {
    return null!;
  }
}