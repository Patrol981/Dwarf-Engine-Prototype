using Voxelized.ECS;

namespace Voxelized.DataStructures;

public class Entity {
  private ComponentManager _componentManager;
  private string _name = "Entity";

  public Entity() {
    _componentManager = new ComponentManager();
  }

  public void AddComponent(Component component) {
    component.Owner = this;
    _componentManager.AddComponent(component);
  }

  public T GetComponent<T>() where T : Component, new() {
    return _componentManager.GetComponent<T>();
  }

  public ComponentManager GetComponentManager() {
    return _componentManager;
  }

  public string GetName() {
    return _name;
  }

  public void SetName(string name) {
    _name = name;
  }
}