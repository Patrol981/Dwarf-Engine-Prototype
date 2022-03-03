using Voxelized.DataStructures;
using Voxelized.ECS;
using Voxelized.Loaders;

namespace Voxelized.Scenes;

public abstract class Scene {
  public List<Entity> Entities;

  public Scene() {
    Entities = new List<Entity>();
  }

  internal abstract void RenderScene();
}