using Dwarf.Engine.DataStructures.Enums;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Loaders;
using Dwarf.Engine.Physics;

using OpenTK.Mathematics;

namespace Dwarf.Engine.DataStructures;

public enum EntityType {
  Entity,
  Terrain
}

public class Entity {
  private ComponentManager _componentManager;
  private string _name = "Entity";
  private Guid _guid = Guid.NewGuid();
  private EntityType _type = EntityType.Entity;

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

  public void RemoveComponent<T>() where T : Component {
    _componentManager.RemoveComponent<T>();
  }

  public static T CreateEmpty<T>(
    string name,
    Vector3 position
  ) where T : Entity, new() {
    T entity = new T();
    entity.Name = name;
    entity.AddComponent(new Transform(position));

    return entity;
  }

  public static void AddMeshToEmpty(
    Entity entity,
    string modelPath,
    string vertexShader,
    string fragmentShader,
    bool usingExternalShaderFile = true
  ) {
    entity.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
    entity.AddComponent(new ObjLoader().Load(modelPath));
    entity.AddComponent(new MeshRenderer());
    entity.GetComponent<MeshRenderer>()?.Init(vertexShader, fragmentShader, usingExternalShaderFile);
  }

  public static T CreateWithMesh<T>(
    string name,
    Vector3 position,
    string modelPath,
    string vertexShader,
    string fragmentShader,
    bool usingExternalShaderFile = true
  ) where T : Entity, new() {
    var entity = CreateEmpty<T>(name, position);

    entity.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
    entity.AddComponent(new GenericLoader().Load($"{WindowSettings.GetGlobalPath()}/{modelPath}"));
    entity.AddComponent(new MeshRenderer());
    entity.GetComponent<MeshRenderer>()?.Init(vertexShader, fragmentShader, usingExternalShaderFile);

    return entity;
  }

  public static T CreateSprite<T>(
    string name,
    Vector3 position,
    string texturePath,
    string vertexShader,
    string fragmentShader,
    bool usingExternalShaderFile = true
  ) where T : Entity, new() {
    var entity = CreateEmpty<T>(name, position);

    entity.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
    entity.AddComponent(new Sprite($"{WindowSettings.GetGlobalPath()}/{texturePath}"));
    entity.AddComponent(new SpriteRenderer());
    entity.GetComponent<SpriteRenderer>()?.Init(vertexShader, fragmentShader, usingExternalShaderFile);
    entity.GetComponent<Transform>().Rotation = new Vector3(0, 180, 0);

    return entity;
  }

  public static T CreateMeshWithCollision<T>(
    string name,
    Vector3 position,
    string modelPath,
    string vertexShader,
    string fragmentShader,
    bool usingExternalShaderFile,
    Entity floor,
    CollisionType collisionType
  ) where T : Entity, new() {
    var entity = CreateWithMesh<T>(name, position, modelPath, vertexShader, fragmentShader, usingExternalShaderFile);

    switch (collisionType) {
      case CollisionType.BoundingBox:
        entity.AddComponent(new BoundingBox());
        entity.GetComponent<BoundingBox>().Setup(entity.GetComponent<MasterMesh>());
        break;

      default:
        break;
    }

    if (floor == null) return entity;

    var tr = floor.GetComponent<MasterMesh>();

    entity.AddComponent(new Rigidbody());
    entity.GetComponent<Rigidbody>().Setup(tr);

    return entity;
  }

  public static T CreateSpriteWithCollision<T>(
    string name,
    Vector3 position,
    string texturePath,
    string vertexShader,
    string fragmentShader,
    bool usingExternalShaderFile = true
  ) where T : Entity, new() {
    var entity = CreateSprite<T>(name, position, texturePath, vertexShader, fragmentShader, usingExternalShaderFile);

    entity.AddComponent(new BoundingBox2D());
    entity.GetComponent<BoundingBox2D>().Setup(entity.GetComponent<Sprite>());

    return entity;
  }

  public ComponentManager GetComponentManager() {
    return _componentManager;
  }

  public string Name {
    get { return _name; }
    set { _name = value; }
  }

  public EntityType Type {
    get { return _type; }
    set { _type = value; }
  }

  public Guid EntityID {
    get { return _guid; }
    set { _guid = value; }
  }
}