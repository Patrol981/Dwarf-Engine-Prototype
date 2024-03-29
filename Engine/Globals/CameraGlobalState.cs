using OpenTK.Mathematics;

using Dwarf.Engine.Cameras;
using Dwarf.Engine.Enums;
using Dwarf.Engine.ECS;
using Dwarf.Engine.DataStructures;

namespace Dwarf.Engine.Globals;

public static class CameraGlobalState {
  private static Camera? _camera;
  private static Entity? _cameraEntity;
  private static float _cameraSpeed = 1.5f;
  private static float _sensitivity = 0.2f;
  private static bool _firstMove = true;
  private static WindowRenderMode _windowRednerMode = WindowRenderMode.Normal;
  private static Vector2 _lastPos;

  public static void SetCamera(Camera camera) {
    _camera = camera;
  }

  public static void SetCameraEntity(Entity cameraEntity) {
    _cameraEntity = cameraEntity;
  }

  public static void SetCameraSpeed(float cameraSpeed) {
    _cameraSpeed = cameraSpeed;
  }

  public static void SetSensitivity(float sensitivity) {
    _sensitivity = sensitivity;
  }

  public static void SetFirstMove(bool firstMove) {
    _firstMove = firstMove;
  }

  public static void SetWindowRenderMode(WindowRenderMode windowRenderMode) {
    _windowRednerMode = windowRenderMode;
  }

  public static void SetLastPosition(Vector2 lastPos) {
    _lastPos = lastPos;
  }

  public static Camera GetCamera() {
    if(_camera == null) {
      _camera = new FreeCamera(float.NaN);
    }
    return _camera;
  }

  public static Entity GetCameraEntity() {
    if(_cameraEntity == null) {
      _cameraEntity = new Entity();
    }
    return _cameraEntity;
  }

  public static float GetCameraSpeed() {
    return _cameraSpeed;
  }

  public static float GetSensitivity() {
    return _sensitivity;
  }

  public static bool GetFirstMove() {
    return _firstMove;
  }
  public static WindowRenderMode GetWindowRenderMode() {
    return _windowRednerMode;
  }
  public static Vector2 GetLastPosition() {
    return _lastPos;
  }
}