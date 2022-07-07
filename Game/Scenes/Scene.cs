using Voxelized.Engine.DataStructures;
using Voxelized.Engine.ECS;
using Voxelized.Engine.Loaders;

namespace Voxelized.Engine.Scenes;

public abstract class Scene {
  public List<Entity> Entities;

  public Scene() {
    Entities = new List<Entity>();
  }

  internal abstract void RenderScene();
}