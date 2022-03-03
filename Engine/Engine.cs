using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using System.Linq;
using System.Text;

using Voxelized.Windowing;
using Voxelized.ECS;
using Voxelized.DataStructures;
using Voxelized.Globals;
using Voxelized.Scenes;
using Voxelized.Cameras;
using Voxelized.Shaders;
using Voxelized.Loaders;
using Voxelized.Info;

using Voxelized.Challanges;

namespace Voxelized;

public class Engine {
  private Voxelized.Windowing.Window _window;
  private Scene _scene;
  private FPS _fps;

  public Engine() {
    _window = new Voxelized.Windowing.Window(GameWindowSettings.Default, WindowSettings.GetNativeWindowSettings());

    _window.BindUpdateCallback(OnUpdate);
    _window.BindRenderCallback(OnRender);
    _window.BindResizeCallback(OnResize);
    _window.BindDrawGUICallback(OnDrawGUI);

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0,0,1)));
    camera.AddComponent(new FreeCamera(Vector3.UnitZ * 3, _window.Size.X / (float) _window.Size.Y));
    CameraGlobalState.SetCameraEntity(camera);

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
    for(int i=0; i<_scene.Entities.Count; i++) {
      _scene.Entities[i].GetComponent<MeshRenderer>().Render(camera);
    }
  }

  public void OnResize() {
    CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>().AspectRatio =
      _window.Size.X / (float) _window.Size.Y;
  }

  public void OnDrawGUI() {
    if(ImGui.BeginMainMenuBar()) {
      ImGui.Text($"FPS: {FPSState.GetFrames()}");

      if(ImGui.BeginMenu("Debug Data")) {
        var entities = EntityGlobalState.GetEntities();
        for(int i=0; i<entities.Count; i++) {
          ImGui.PushID(entities[i].GetName());
          if(ImGui.TreeNodeEx(entities[i].GetName())) {
            ImGui.Text($"Model {i} name: {entities[i].GetName()}");
            var color = entities[i].GetComponent<Material>().GetColor();
            var pos = entities[i].GetComponent<Transform>();
            var cnvVec = new System.Numerics.Vector3(color.X, color.Y, color.Z);
            var cnvPos = new System.Numerics.Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
            ImGui.DragFloat3("Material Color", ref cnvVec, 0.01f);
            entities[i].GetComponent<Material>().SetColor(new Vector3(cnvVec.X, cnvVec.Y, cnvVec.Z));
            ImGui.DragFloat3("Position", ref cnvPos, 0.01f);
            entities[i].GetComponent<Transform>().Position = new Vector3(cnvPos.X, cnvPos.Y, cnvPos.Z);
          }
        }
      }
    }
  }

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
}