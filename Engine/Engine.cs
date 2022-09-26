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

  public EngineClass(Windowing.Window window = null!, Scene scene = null!) {
    if(window == null) {
      _window = new Dwarf.Engine.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings());
      WindowGlobalState.SetWindow(_window);
    } else {
      _window = window;
    }

    _window.BindUpdateCallback(OnUpdate);
    _window.BindRenderCallback(OnRender);
    _window.BindResizeCallback(OnResize);
    _window.BindDrawGUICallback(OnDrawGUI);

    if(scene == null) {
      Scene = new DebugScene();
    } else {
      Scene = scene;
    }

    //_physics = new Physics.Physics();

    _fps = new FPS();

    _skybox = new Skybox(_window.Size.X / (float)_window.Size.Y);

    _raycaster = new((Camera)CameraGlobalState.GetCamera());

  }

  public void Run() {
    _window.Run();
  }

  protected void OnUpdate() {
    _fps.Update();

    for(int i=0; i<Scene.Entities.Count; i++) {
      if(Scene.Entities[i].GetComponent<TransformController>() != null) {
        Scene.Entities[i].GetComponent<TransformController>().HandleMovement();
      }
    }

    _onUpdate?.Invoke();
  }

  protected void OnRender() {
    var camera = (ICamera)CameraGlobalState.GetCamera();
    camera.HandleMovement();

    _skybox.Update((Camera)camera);
    
    for (int i = 0; i < Scene.Entities.Count; i++) {
      Scene.Entities[i].GetComponent<MeshRenderer>().Render((Camera)camera);
      if (Scene.Entities[i].GetComponent<BoundingBox>() != null) {
        Scene.Entities[i].GetComponent<BoundingBox>().Draw((Camera)camera);
      }
    }

    _onRender?.Invoke();
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
}
