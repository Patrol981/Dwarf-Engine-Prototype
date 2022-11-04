using System.Linq;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Info;
using Dwarf.Engine.Skyboxes;
using Dwarf.Engine.Scenes;
using Dwarf.Engine.Primitives;
using Dwarf.Engine.Controllers;
using Dwarf.Engine.Raycasting;
using Dwarf.Engine.Physics;
using Dwarf.Engine.DataStructures.Interfaces;
using Dwarf.Engine.DataStructures.Enums;
using Dwarf.Engine.Loaders;

namespace Dwarf.Engine;

public class EngineClass {
  // User API
  public delegate void EventCallback();

  public void SetUpdateCallback(EventCallback eventCallback) {
    _onUpdate = eventCallback;
  }

  public void SetRenderCallback(EventCallback eventCallback) {
    _onRender = eventCallback;
  }

  public void SetGUICallback(EventCallback eventCallback) {
    _onGUI = eventCallback;
  }

  public void SetOnLoadCallback(EventCallback eventCallback) {
    _onLoad = eventCallback;
  }
  
  public Scene Scene;

  // Engine Data
  private Windowing.Window _window;
  private FPS _fps;
  private Skybox _skybox;
  private Raycaster _raycaster;
  private Physics.Physics _physics;

  private EventCallback? _onUpdate;
  private EventCallback? _onRender;
  private EventCallback? _onGUI;
  private EventCallback? _onLoad;

  public EngineClass(Windowing.Window window = null!, Scene scene = null!) {
    if(window == null) {
      _window = new Dwarf.Engine.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings());
      WindowGlobalState.SetWindow(_window);
    } else {
      _window = window;
      WindowGlobalState.SetWindow(_window);
    }

    _window.BindUpdateCallback(OnUpdate);
    _window.BindRenderCallback(OnRender);
    _window.BindResizeCallback(OnResize);
    _window.BindDrawGUICallback(OnDrawGUI);
    _window.BindOnLoadCallback(OnLoad);

    if(scene == null) {
      Scene = new DebugScene();
    } else {
      Scene = scene;
    }

    // _physics = new Physics.Physics();

    // var physX = new PhysXClass();

    _fps = new FPS();

    _skybox = new Skybox(_window.Size.X / (float)_window.Size.Y);

    _raycaster = new((Camera)CameraGlobalState.GetCamera(), CameraGlobalState.GetCamera().GetProjectionMatrix());

  }

  public void Run() {
    _window.Run();
  }

  protected void OnLoad() {
    _onLoad?.Invoke();
  }

  protected void OnUpdate() {
    _onUpdate?.Invoke();

    _fps.Update();

    for(int i=0; i<Scene.Entities.Count; i++) {
      Scene.Entities[i].GetComponent<TransformController>()?.HandleMovement();
      Scene.Entities[i].GetComponent<Rigidbody>()?.Update();
    }
  }

  protected void OnRender() {
    _onRender?.Invoke();

    var camera = (ICamera)CameraGlobalState.GetCamera();
    camera.HandleMovement();

    _skybox.Update((Camera)camera);
    
    for (int i = 0; i < Scene.Entities.Count; i++) {
      Scene.Entities[i].GetComponent<MeshRenderer>()?.Render((Camera)camera);
    }
  }

  protected void OnResize() {
    CameraGlobalState.GetCamera().AspectRatio =
      _window.Size.X / (float)_window.Size.Y;
  }

  protected void OnDrawGUI() {
    _onGUI?.Invoke();
  }

  public void AddToScene(Entity entity) {
    Scene.Entities.Add(entity);
  }

  public void LoadScene(Scene scene) {
    Scene = scene;
  }

  public List<Entity> GetEntities() {
    return Scene.Entities;
  }

  public List<Entity> GetEntitiesByType(EntityType entityType) {
    return Scene.Entities.Where(e => e.Type == entityType).ToList();
  }


  public List<T> GetEntities<T>() where T : Entity {
    List<T> returnList = new();

    for(int i= 0; i < Scene.Entities.Count; i++) {
      var target = Scene.Entities[i];
      if(target.GetType() == typeof(T)) {
        returnList.Add((T)target);
      }
    }

    return returnList;
    //return (IEnumerable<T>)Scene.Entities.Where(x => x.GetType() == typeof(T));
    /*
    return AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(assembly => assembly.GetTypes())
      .Where(type => type.IsSubclassOf(typeof(T)))
      .Select(type => Activator.CreateInstance(type) as T);
      */
    //return Scene.Entities.Where(x => )
  }

  public Entity? GetEntityById(Guid id) {
    return Scene.Entities.FirstOrDefault(e => e.EntityID == id);
  }

  public void RemoveEntity(Entity entity) {
    if (Scene.Entities.Contains(entity)) {
      Scene.Entities.Remove(entity);
    }
  }

  public Raycaster Raycaster {
    get { return _raycaster; }
  }
}
