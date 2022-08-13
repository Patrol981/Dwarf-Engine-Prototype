using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Loaders;

namespace Dwarf.Engine.Scenes;

public abstract class Scene {
  public List<Entity> Entities;

  public Scene() {
    Entities = new List<Entity>();
  }

  internal abstract void RenderScene();
}