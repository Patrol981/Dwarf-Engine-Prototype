using Dwarf.Engine.Cameras;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using Dwarf.Engine.DataStructures;

using OpenTK.Mathematics;

namespace Dwarf.Engine.Raycasting;

public class RaycastResult {
  public Vector3 Origin { get; set; }
  public Vector3 Direction { get; set; }

  public RaycastResult(Vector3 origin, Vector3 direction) {
    Origin = origin;
    Direction = direction;
  }
}

public class Raycaster {
  private Vector3 _ray = Vector3.Zero;
  private Vector3 _worldPoint = Vector3.Zero;

  private Matrix4 _projection;
  private Matrix4 _view;
  private Entity _target;

  private const float RayRange = 600;
  private const float RecursionCount = 200;

  private Camera? _camera;

  public Raycaster(Camera camera, Matrix4 projection) {
    _camera = camera;
    _projection = projection;
    _view = _camera.GetViewMatrix();
  }

  public void Update() {
    _view = _camera!.GetViewMatrix();
    _projection = _camera!.GetProjectionMatrix();
    _ray = CalculateMouseRay();
    if (IntersectionInRange(0, RayRange, _ray)) {
      _worldPoint = BinarySearch(0, 0, RayRange, _ray);
    } else {
      _worldPoint = new Vector3(0,0,0);
    }
  }

  private Vector3 CalculateMouseRay() {
    var cursor = WindowGlobalState.GetMouseState().Position;
    var normalizedCoords = GetNormalizedDeviceCoords(cursor.X, cursor.Y);
    var clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, -1.0f, 1.0f);
    var eyeCoords = ToEyeCoords(clipCoords);
    var worldRay = ToWorldCoords(eyeCoords);
    return worldRay;
  }

  private Vector2 GetNormalizedDeviceCoords(float mouseX, float mouseY) {
    var window = WindowGlobalState.GetWindow();
    float x = (2.0f * mouseX) / window.ClientSize.X - 1f;
    float y = (2.0f * mouseY) / window.ClientSize.Y - 1f;
    return new Vector2(x, -y);
  }

  private Vector4 ToEyeCoords(Vector4 clipCoords) {
    var invertedProjection = Matrix4.Invert(_projection);
    var eyeCoords = Vector4.TransformRow(clipCoords, invertedProjection);
    return new Vector4(eyeCoords.X, eyeCoords.Y, -1f, 0f);
  }

  private Vector3 ToWorldCoords(Vector4 eyeCoords) {
    var invertedView = Matrix4.Invert(_view);
    var rayWorld = Vector4.TransformRow(eyeCoords, invertedView);
    var mouseRay = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z);
    mouseRay.Normalize();
    return mouseRay;
  }

  private Vector3 GetPointOnRay(Vector3 ray, float distance) {
    var cameraPosition = CameraGlobalState.GetCameraEntity().GetComponent<Transform>().Position;
    var start = new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
    var scaledRay = new Vector3(ray.X * distance, ray.Y * distance, ray.Z * distance);
    return Vector3.Add(start, scaledRay);
  }

  private Vector3 BinarySearch(int count, float start, float finish, Vector3 ray) {
    float half = start + ((finish - start) / 2f);
    if (count >= RecursionCount) {
      Vector3 endPoint = GetPointOnRay(ray, half);
      var entity = GetEntity();
      // Terrain terrain = GetTerrain(endPoint.getX(), endPoint.getZ());
      if (entity != null) {
        return endPoint;
      } else {
        return new Vector3(0,0,0);
      }
    }
    if (IntersectionInRange(start, half, ray)) {
      return BinarySearch(count + 1, start, half, ray);
    } else {
      return BinarySearch(count + 1, half, finish, ray);
    }
  }

  private bool IntersectionInRange(float start, float end, Vector3 ray) {
    Vector3 startPoint = GetPointOnRay(ray, start);
    Vector3 endPoint = GetPointOnRay(ray, end);
    if (!IsUnderground(startPoint) && IsUnderground(endPoint)) {
      return true;
    } else {
      return false;
    }
  }

  private bool IsUnderground(Vector3 testPoint) {
    // var startPoint = GetEntity().GetComponent<Transform>().Position;
    var entity = GetEntity();
    float height = 0;
    if(entity != null) {
      var terrain = (TerrainMesh)entity.GetComponent<MasterMesh>().Meshes[0];
      height = terrain.GetHeightOfTerrain(testPoint.X, testPoint.Z, entity);
    }
    if(testPoint.Y < height) {
      return true;
    } else {
      return false;
    }
  }

  private Entity GetEntity() {
    return EntityGlobalState.GetEntities()[0];
  }

  public Vector3 Ray {
    get { return _ray; }
  }

  public Vector3 WorldPoint {
    get { return _worldPoint; }
  }
}

