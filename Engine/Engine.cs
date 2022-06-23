using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Voxelized.Cameras;
using Voxelized.DataStructures;
using Voxelized.ECS;
using Voxelized.Globals;
using Voxelized.Info;
using Voxelized.Scenes;

using Voxelized.Engine.Generators;

namespace Voxelized;

public class EngineClass {
  private Voxelized.Windowing.Window _window;
  private Scene _scene;
  private FPS _fps;

  private Entity terrain;

  public EngineClass() {
    _window = new Voxelized.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings());

    _window.BindUpdateCallback(OnUpdate);
    _window.BindRenderCallback(OnRender);
    _window.BindResizeCallback(OnResize);
    _window.BindDrawGUICallback(OnDrawGUI);

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0, 0, 1)));
    camera.AddComponent(new FreeCamera(Vector3.UnitZ * 3, _window.Size.X / (float)_window.Size.Y));
    CameraGlobalState.SetCameraEntity(camera);

    terrain = new Entity();
    terrain.AddComponent(new Transform(new Vector3(0, 0, 0)));
    terrain.AddComponent(new Material(new Vector3(1f, 1f, 0.0f)));
    terrain.AddComponent(new MeshGenerator());
    terrain.GetComponent<MeshGenerator>().SetupPlane(5, 5);
    terrain.SetName("new terrain");
    //terrain.AddComponent(new MeshRenderer());
    //terrain.GetComponent<MeshRenderer>().Init();

    _scene = new DebugScene();

    _fps = new FPS();

  }

  public void Run() {
    _window.Run();
  }

  public void OnUpdate() {
    _fps.Update();
  }

  public void OnRender() {
    var camera = CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>();
    camera.HandleMovement();
    // MouseSelect();
    for (int i = 0; i < _scene.Entities.Count; i++) {
      _scene.Entities[i].GetComponent<MeshRenderer>().Render(camera);
    }

    // terrain.GetComponent<MeshRenderer>().Render(camera);
    terrain.GetComponent<MeshGenerator>().Render(camera);
  }

  public void OnResize() {
    CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>().AspectRatio =
      _window.Size.X / (float)_window.Size.Y;
  }

  public void OnDrawGUI() {
    if (ImGui.BeginMainMenuBar()) {
      ImGui.Text($"FPS: {FPSState.GetFrames()}");

      if (ImGui.BeginMenu("Debug Data")) {
        var entities = EntityGlobalState.GetEntities();
        if(ImGui.TreeNodeEx(terrain.GetName())) {
          ImGui.Text($"Terrain name: {terrain.GetName()}");
          var color = terrain.GetComponent<Material>().GetColor();
          var pos = terrain.GetComponent<Transform>();
          var cnvVec = new System.Numerics.Vector3(color.X, color.Y, color.Z);
          var cnvPos = new System.Numerics.Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
          ImGui.DragFloat3("Material Color", ref cnvVec, 0.01f);
          terrain.GetComponent<Material>().SetColor(new Vector3(cnvVec.X, cnvVec.Y, cnvVec.Z));
          ImGui.DragFloat3("Position", ref cnvPos, 0.01f);
          terrain.GetComponent<Transform>().Position = new Vector3(cnvPos.X, cnvPos.Y, cnvPos.Z);
        }
        for (int i = 0; i < entities.Count; i++) {
          ImGui.PushID(entities[i].GetName());
          if (ImGui.TreeNodeEx(entities[i].GetName())) {
            ImGui.Text($"Model {i} name: {entities[i].GetName()}");
            var color = entities[i].GetComponent<Material>().GetColor();
            var pos = entities[i].GetComponent<Transform>();
            var cnvVec = new System.Numerics.Vector3(color.X, color.Y, color.Z);
            var cnvPos = new System.Numerics.Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
            var cnvRot = new System.Numerics.Vector3(pos.Rotation.X, pos.Rotation.Y, pos.Rotation.Z);
            ImGui.DragFloat3("Material Color", ref cnvVec, 0.01f);
            entities[i].GetComponent<Material>().SetColor(new Vector3(cnvVec.X, cnvVec.Y, cnvVec.Z));
            ImGui.DragFloat3("Position", ref cnvPos, 0.01f);
            entities[i].GetComponent<Transform>().Position = new Vector3(cnvPos.X, cnvPos.Y, cnvPos.Z);
            ImGui.DragFloat3("Rotation", ref cnvPos, 0.01f);
            entities[i].GetComponent<Transform>().Rotation = new Vector3(cnvRot.X, cnvRot.Y, cnvRot.Z);
          }
        }
      }


    }

    if (ImGui.Begin("Camera Test")) {
      var targetTransform = EntityGlobalState.GetEntities()[0].GetComponent<Transform>();
      var cameraTransform = CameraGlobalState.GetCameraEntity().GetComponent<Transform>();

      ImGui.Text("Camera");
      ImGui.Text($"X:{cameraTransform.Position.X} Y:{cameraTransform.Position.Y} Z:{cameraTransform.Position.Z}");
      ImGui.Text("Target");
      ImGui.Text($"X:{targetTransform.Position.X} Y:{targetTransform.Position.Y} Z:{targetTransform.Position.Z}");
    }
  }

  /*
  private void MouseSelect() {
    if(WindowGlobalState.GetMouseState().IsButtonDown(MouseButton.Left)) {
      _window.Clear(new Vector3(0,0,0));

      for(int i=0; i<_scene.Entities.Count; i++) {
        Vector3 color = _scene.Entities[i].GetComponent<Material>().GetColor();
        int id = i+1;

        _scene.Entities[i].GetComponent<Material>().SetColor(new Vector3((float) id / 255, 0, 0));
        _scene.Entities[i].GetComponent<MeshRenderer>().GetShader().Use();
        _scene.Entities[i].GetComponent<MeshRenderer>().GetShader().SetInt("val", 0);

        _scene.Entities[i].GetComponent<MeshRenderer>().Render(CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>());

        _scene.Entities[i].GetComponent<MeshRenderer>().GetShader().SetInt("val", 1);
        _scene.Entities[i].GetComponent<Material>().SetColor(color);
      }

      byte[] pixels = new byte[4];
      var currPos = WindowGlobalState.GetMouseState();


      // GL.ReadPixels((int)(currPos.X - _window.ClientSize.X), (int)(_window.Size.Y - (currPos.Y - _window.ClientSize.Y)), 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
      GL.ReadPixels((int)(currPos.X - _window.Size.X), (int)(_window.Bounds.Max.Y - (currPos.Y - _window.Size.Y)), 1, 1, PixelFormat.Rgba,
                    PixelType.UnsignedByte, pixels);
      int index = pixels[0] - 1;

      if(index != -1) {
        Console.WriteLine($"Selected {index}");
      } else {
        Console.WriteLine(index);
      }

      _window.Clear();
    }
  }
  */
}