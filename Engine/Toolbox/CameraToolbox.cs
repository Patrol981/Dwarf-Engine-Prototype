
using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Toolbox;
public static class CameraToolbox {
  public static void CreateFreeCamera(ref Entity camera, ref Windowing.Window window) {
    camera.AddComponent(new Transform(new Vector3(0, 10, 0)));
    camera.AddComponent(new FreeCamera(window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }

  public static void CreateThirdPersonCamera(
    ref Entity camera,
    ref Windowing.Window window,
    Entity target = null!
   ) {
    camera.AddComponent(new Transform(new Vector3(0, 10, 0)));
    camera.AddComponent(new ThirdPersonCamera(window.Size.X / (float)window.Size.Y, target));
    CameraGlobalState.SetCamera(camera.GetComponent<ThirdPersonCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }
}

