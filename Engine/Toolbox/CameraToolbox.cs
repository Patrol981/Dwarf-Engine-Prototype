
using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using OpenTK.Mathematics;
using Dwarf.Engine.Windowing;

namespace Dwarf.Engine.Toolbox;
public static class CameraToolbox {
  public static void CreateFreeCamera(ref Entity camera, ref Window window) {
    camera.AddComponent(new Transform(new Vector3(0, 10, 0)));
    camera.AddComponent(new FreeCamera(window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }

  public static void CreateThirdPersonCamera(
    ref Entity camera,
    ref Window window,
    Entity target = null!
   ) {
    camera.AddComponent(new Transform(new Vector3(0, 10, 0)));
    camera.AddComponent(new ThirdPersonCamera(window.Size.X / (float)window.Size.Y, target));
    CameraGlobalState.SetCamera(camera.GetComponent<ThirdPersonCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }

  public static void CreateStaticCamera(ref Entity camera, ref Window window) {
    camera.AddComponent(new Transform(new Vector3(0, 10, 0)));
    camera.AddComponent(new StaticCamera(window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCamera(camera.GetComponent<StaticCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }

  public static void CreateOrthograpbicCamera(ref Entity camera, ref Window window) {
    camera.AddComponent(new Transform(new Vector3(0, 0, 0)));
    camera.AddComponent(new OrtographicCamera(window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCamera(camera.GetComponent<OrtographicCamera>());
    CameraGlobalState.SetCameraEntity(camera);
  }

  public static void RemoveCamera<T>(ref Entity camera) where T: Component {
    camera.RemoveComponent<T>();
  }
}

