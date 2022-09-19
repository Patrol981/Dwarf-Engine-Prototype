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

  public EngineClass(Windowing.Window window = null!) {
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

    Scene = new DebugScene();

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
    if (ImGui.BeginMainMenuBar()) {
      ImGui.Text($"FPS: {FPSState.GetFrames()}");

      if (ImGui.BeginMenu("Debug Data")) {
        var entities = EntityGlobalState.GetEntities();
        for (int i = 0; i < entities.Count; i++) {
          ImGui.PushID(entities[i].GetName());
          if (ImGui.TreeNodeEx(entities[i].GetName())) {
            ImGui.Text($"Model {i} name: {entities[i].GetName()}");
            ImGui.Text($"Meshes: {entities[i].GetComponent<MasterMesh>().Meshes.Count}");
            for(int x=0; x< entities[i].GetComponent<MasterMesh>().Meshes.Count; x++) {
              ImGui.Text(entities[i].GetComponent<MasterMesh>().Meshes[x].Name);
            }
            var color = entities[i].GetComponent<Material>().GetColor();
            var pos = entities[i].GetComponent<Transform>();
            var cnvVec = new System.Numerics.Vector3(color.X, color.Y, color.Z);
            var cnvPos = new System.Numerics.Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
            var cnvRot = new System.Numerics.Vector3(pos.Rotation.X, pos.Rotation.Y, pos.Rotation.Z);
            ImGui.DragFloat3("Material Color", ref cnvVec, 0.01f);
            entities[i].GetComponent<Material>().SetColor(new Vector3(cnvVec.X, cnvVec.Y, cnvVec.Z));
            ImGui.DragFloat3("Position", ref cnvPos, 0.01f);
            entities[i].GetComponent<Transform>().Position = new Vector3(cnvPos.X, cnvPos.Y, cnvPos.Z);
            ImGui.DragFloat3("Rotation", ref cnvRot, 0.01f);
            entities[i].GetComponent<Transform>().Rotation = new Vector3(cnvRot.X, cnvRot.Y, cnvRot.Z);
            if(ImGui.Button("Switch Controller")) {
              for(int x=0; x<Scene.Entities.Count; x++) {
                var target = Scene.Entities[x];
                if(target.GetComponent<TransformController>() != null) {
                  target.RemoveComponent<TransformController>();
                  break;
                }
              }
              CameraGlobalState.GetCameraEntity().GetComponent<ThirdPersonCamera>().FollowTarget = entities[i];
              entities[i].AddComponent(new TransformController(1.5f));
              WindowGlobalState.SetCursorVisible(false);
            }
          }
        }
      }
    }

    if (ImGui.Begin("Camera Test")) {
      if (EntityGlobalState.GetEntities().Count == 0) return;
      var cameraTransform = CameraGlobalState.GetCameraEntity().GetComponent<Transform>();
      var camera = CameraGlobalState.GetCameraEntity().GetComponent<ThirdPersonCamera>();

      ImGui.Text("Camera");
      ImGui.Text($"X:{cameraTransform.Position.X} Y:{cameraTransform.Position.Y} Z:{cameraTransform.Position.Z}");
      ImGui.Text($"X:{cameraTransform.Rotation.X} Y:{cameraTransform.Rotation.Y} Z:{cameraTransform.Rotation.Z}");

      /*
      var camTarget = CameraGlobalState.GetCameraEntity().GetComponent<ThirdPersonCamera>().FollowTarget.GetComponent<Transform>();

      ImGui.Text($"Front: {camera.Front}");
      ImGui.Text($"Yaw: {camera.Yaw}");
      ImGui.Text($"Pitch: {camera.Pitch}");
      ImGui.Text($"Distance: {camera.ScrollDistance}");
      ImGui.Text($"Angle: {camera.Angle}");
      

      ImGui.Text("Target");
      ImGui.Text($"X:{camTarget.Position.X} Y:{camTarget.Position.Y} Z:{camTarget.Position.Z}");
      ImGui.Text($"X:{camTarget.Rotation.X} Y:{camTarget.Rotation.Y} Z:{camTarget.Rotation.Z}");
      */
    }

    if(ImGui.Begin("Boundings")) {
      var target = Scene.Entities[0];

      ImGui.Text($"Target: {target.GetName()}");

      ImGui.Text($"Meshes: {target.GetComponent<MasterMesh>().Meshes.Count}");

      ImGui.Text("Size");
      ImGui.Text(target.GetComponent<BoundingBox>().Size.ToString());
      ImGui.Text("Center");
      ImGui.Text(target.GetComponent<BoundingBox>().Center.ToString());
      ImGui.Text("Transfrom");
      ImGui.Text(target.GetComponent<BoundingBox>().Tranform.ToString());
      ImGui.Text("Model");
      ImGui.Text(target.GetComponent<BoundingBox>().Model.ToString());
      ImGui.Text("WorldModel");
      ImGui.Text(target.GetComponent<BoundingBox>().WorldModel.ToString());
    }

    _onGUI?.Invoke();
  }

  public void AddToScene(Entity entity) {
    Scene.Entities.Add(entity);
  }
}
