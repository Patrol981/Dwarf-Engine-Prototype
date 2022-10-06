

using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Scenes;
public class DebugScene : Scene {
  public DebugScene(): base() {
    var window = WindowGlobalState.GetWindow();

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0, 5, 0)));
    camera.AddComponent(new FreeCamera(window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCameraEntity(camera);
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());
  }

  public override void RenderScene() {
    throw new NotImplementedException();
  }
}

