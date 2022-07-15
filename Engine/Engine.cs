using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Voxelized.Engine.Cameras;
using Voxelized.Engine.DataStructures;
using Voxelized.Engine.ECS;
using Voxelized.Engine.Globals;
using Voxelized.Engine.Info;
using Voxelized.Engine.Skyboxes;
using Voxelized.Engine.Scenes;

namespace Voxelized.Engine;

public class EngineClass {
  private Windowing.Window _window;
  private Scene _scene;
  private FPS _fps;
  private Skybox _skybox;

  public EngineClass() {
    _window = new Voxelized.Engine.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings());
    WindowGlobalState.SetWindow(_window);

    _window.BindUpdateCallback(OnUpdate);
    _window.BindRenderCallback(OnRender);
    _window.BindResizeCallback(OnResize);
    _window.BindDrawGUICallback(OnDrawGUI);

    _scene = new DebugScene();

    _fps = new FPS();

    _skybox = new Skybox(_window.Size.X / (float)_window.Size.Y);

  }

  public void Run() {
    _window.Run();
  }

  public void OnUpdate() {
    _fps.Update();
  }

  public void OnRender() {
    // var camera = CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>();
    var camera = (ICamera)CameraGlobalState.GetCamera();
    camera.HandleMovement();

    _skybox.Update((Camera)camera);
    // MouseSelect();
    for (int i = 0; i < _scene.Entities.Count; i++) {
      _scene.Entities[i].GetComponent<MeshRenderer>().Render((Camera)camera);
    }
  }

  public void OnResize() {
    CameraGlobalState.GetCamera().AspectRatio =
      _window.Size.X / (float)_window.Size.Y;
  }

  public void OnDrawGUI() {
    if (ImGui.BeginMainMenuBar()) {
      ImGui.Text($"FPS: {FPSState.GetFrames()}");

      if (ImGui.BeginMenu("Debug Data")) {
        var entities = EntityGlobalState.GetEntities();
        for (int i = 0; i < entities.Count; i++) {
          ImGui.PushID(entities[i].GetName());
          if (ImGui.TreeNodeEx(entities[i].GetName())) {
            ImGui.Text($"Model {i} name: {entities[i].GetName()}");
            ImGui.Text($"Meshes: {entities[i].GetComponent<MasterMesh>().Meshes.Count}");
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
          }
        }
      }


    }

    if (ImGui.Begin("Camera Test"))
      if (EntityGlobalState.GetEntities().Count == 0) return;
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
