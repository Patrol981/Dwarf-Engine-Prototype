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

  private Matrix4 _projection;
  private Matrix4 _view;
  private Entity _target;


  private Camera? _camera;

  public Raycaster(Camera camera, Matrix4 projection) {
    _camera = camera;
    _projection = projection;
    _view = _camera.GetViewMatrix();
  }

  public Raycaster(Camera camera) {
    _camera = camera;
    _projection = _camera.GetProjectionMatrix();
    //CreateProjection();
    _view = _camera.GetViewMatrix();
  }

  public Raycaster(Camera camera, Entity target) {
    _camera = camera;
    _target = target;
    _projection = _camera.GetProjectionMatrix();
    _view = _camera.GetViewMatrix();
  }

  public void Update() {
    if (_camera == null) return;

    _projection = _camera.GetProjectionMatrix();
    _view = _camera.GetViewMatrix();
    _ray = CalculateRay();
    //CalcRay();
  }

  private Vector3 CalculateRay() {
    var cursor = WindowGlobalState.GetMouseState();
    var window = WindowGlobalState.GetWindow().Size;

    //Console.WriteLine(cursor.Position);
    // Console.WriteLine(window.X);

    float x = (2.0f * cursor.Position.X) / window.X - 1.0f;
    float y = 1.0f - (2.0f * cursor.Position.Y) / window.Y;
    float z = 1.0f;
    Vector3 rayNds = new(x, y, z);

    // Console.WriteLine(rayNds);

    Vector4 rayClip = new(rayNds.X, rayNds.Y, -1.0f, 1.0f);

    // Console.WriteLine(rayClip);

    Vector4 rayEye = Matrix4.Invert(_camera.GetProjectionMatrix()) * rayClip;
    rayEye.Z = -1.0f;
    rayEye.W = 0.0f;

    // Console.WriteLine(rayEye);

    Vector3 worldRay = (Matrix4.Invert(_camera.GetViewMatrix()) * rayEye).Xyz;
    worldRay.Normalize();

    // Console.WriteLine(worldRay);
    return worldRay;
  }

  public bool OBBIntersection(
    RaycastResult raycastResult,
    Vector3 aabbMin,
    Vector3 aabbMax,
    Matrix4 modelMatrix,
    ref float distance
  ) {
    float tMin = 0.01f;
    float tMax = 1000.0f;

    // Console.WriteLine(modelMatrix);

    //Vector3 OBBPositionWorldspace = modelMatrix.ExtractTranslation();
    Vector3 OBBPositionWorldspace = new(modelMatrix[3,0], modelMatrix[3,1], modelMatrix[3,2]);

    //Console.WriteLine(OBBPositionWorldspace);

    Vector3 delta = OBBPositionWorldspace - raycastResult.Origin;
    

    // x axis intersection
    Vector3 axisX = new(modelMatrix[0,0], modelMatrix[0,1], modelMatrix[0,2]);
    float e = Vector3.Dot(axisX, delta);
    float f = Vector3.Dot(raycastResult.Direction, axisX);

    Console.WriteLine(raycastResult.Direction);
    Console.WriteLine(f);

    if(MathF.Abs(f) > 0.001f) {
      float t1 = (e + aabbMin.X) / f;
      float t2 = (e + aabbMax.X) / f;

      if (t1 > t2) {
        float w;
        w = t1;
        t1 = t2;
        t2 = w;
      }

      if(t2 < tMax) {
        tMax = t2;
      }

      if(t1 > tMin) {
        tMin = t1;
      }

      if (tMax < tMin) {
        return false;
      }
    } else {
      if (-e + aabbMin.X > 0.0f || -e + aabbMax.X < 0.0f) {
        return false;
      }
    }

    // y axis intersection

    Vector3 axisY = new(modelMatrix[1, 0], modelMatrix[1, 1], modelMatrix[1, 2]);
    e = Vector3.Dot(axisY, delta);
    f = Vector3.Dot(raycastResult.Direction, axisY);

    if (MathF.Abs(f) > 0.001f) {
      float t1 = (e + aabbMin.Y) / f;
      float t2 = (e + aabbMax.Y) / f;

      if (t1 > t2) {
        float w;
        w = t1;
        t1 = t2;
        t2 = w;
      }

      if (t2 < tMax) {
        tMax = t2;
      }

      if (t1 > tMin) {
        tMin = t1;
      }

      if (tMax < tMin) {
        return false;
      }
    } else {
      if (-e + aabbMin.Y > 0.0f || -e + aabbMax.Y < 0.0f) {
        return false;
      }
    }

    // z axis intersection

    Vector3 axisZ = new(modelMatrix[2, 0], modelMatrix[2, 1], modelMatrix[2, 2]);
    e = Vector3.Dot(axisZ, delta);
    f = Vector3.Dot(raycastResult.Direction, axisZ);

    if (MathF.Abs(f) > 0.001f) {
      float t1 = (e + aabbMin.Z) / f;
      float t2 = (e + aabbMax.Z) / f;

      if (t1 > t2) {
        float w;
        w = t1;
        t1 = t2;
        t2 = w;
      }

      if (t2 < tMax) {
        tMax = t2;
      }

      if (t1 > tMin) {
        tMin = t1;
      }

      if (tMax < tMin) {
        return false;
      }
    } else {
      if (-e + aabbMin.Z > 0.0f || -e + aabbMax.Z < 0.0f) {
        return false;
      }
    }


    Console.WriteLine(tMin);
    distance = tMin;
    return true;
  }

  public RaycastResult ScreenPosToRay(Camera camera) {
    Vector2 screen = WindowGlobalState.GetWindow().ClientSize.ToVector2();
    Vector2 mouse = WindowGlobalState.GetMouseState().Position;

    Vector4 rayStartNDC = new(
      ((float)mouse.X / (float)screen.X - 0.5f) * 2.0f,
      ((float)mouse.Y / (float)screen.Y - 0.5f) * 2.0f,
      -1.0f,
      1.0f
    );

    Vector4 rayEndNDC = new(
      ((float)mouse.X / (float)screen.X - 0.5f) * 2.0f,
      ((float)mouse.Y / (float)screen.Y - 0.5f) * 2.0f,
      0.0f,
      1.0f
    );

    Matrix4 invProj = camera.GetProjectionMatrix().Inverted();
    Matrix4 invView = camera.GetViewMatrix().Inverted();

    Vector4 rayStartCamera = invProj * rayStartNDC; rayStartCamera /= rayStartCamera.W;
    Vector4 rayStartWorld = invView * rayStartCamera; rayStartWorld /= rayStartWorld.W;
    Vector4 rayEndCamera = invProj * rayEndNDC; rayEndCamera /= rayEndCamera.W;
    Vector4 rayEndWorld = invView * rayEndCamera; rayEndWorld /= rayEndWorld.W;

    Vector3 rayDirWorld = new(rayEndWorld - rayStartWorld);
    rayDirWorld.Normalize();

    return new RaycastResult(new Vector3(rayStartWorld.X, rayStartWorld.Y, rayStartWorld.Z), rayDirWorld.Normalized());
  }
  
  public RaycastResult ScreenPointToRay(Vector2 mouseLocation) {
    Vector3 near = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 0));
    Vector3 far = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 1));

    Vector3 origin = near;
    Vector3 direction = (far - near).Normalized();
    return new RaycastResult(origin, direction);
  }

  private Vector3 UnProject(Vector3 screen) {
    int[] viewport = new int[4];
    OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, viewport);

    Vector4 pos = new Vector4();

    // Map x and y from window coordinates, map to range -1 to 1 
    pos.X = (screen.X - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
    pos.Y = 1 - (screen.Y - (float)viewport[1]) / (float)viewport[3] * 2.0f;
    pos.Z = screen.Z * 2.0f - 1.0f;
    pos.W = 1.0f;

    Vector4 pos2 = Vector4.TransformRow(pos, Matrix4.Invert(_camera.GetViewMatrix() * _camera.GetProjectionMatrix()));
    Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);

    return pos_out / pos2.W;
  }

  private void CalcRay() {
    var targetPos = new Vector4(_target.GetComponent<Transform>().Position.Xzy, 1f) * _view;

    var cursor = WindowGlobalState.GetMouseState();
    var window = WindowGlobalState.GetWindow().Size;

    float ndcX = (2.0f * cursor.X) / window.X - 1.0f;
    float ndcY = 1.0f - (2.0f * cursor.Y) / window.Y; // flip the Y axis

    float focalLength = 1.0f / MathF.Tan(MathHelper.DegreesToRadians(45.0f / 2.0f));
    Vector4 ndcRay = new(ndcX, ndcY, 1.0f, 1.0f);

    // Console.WriteLine(ndcRay);

    Vector4 rayView = Vector4.TransformColumn(Matrix4.Invert(_projection), ndcRay);

    // Console.WriteLine(rayView);

    Vector4 viewSpaceIntersect = new(rayView * targetPos);

    // Console.WriteLine(viewSpaceIntersect);

    //var pos = _target.GetComponent<Transform>().Position;
    //Vector4 viewSpaceIntersect = new(
    //  rayView.X * targetPos.Z,
    //  rayView.Y * targetPos.Z,
    //  rayView.Z * targetPos.Z,
    //  1.0f);

    Vector4 pointWorld = Matrix4.Invert(_camera.GetViewMatrix()) * viewSpaceIntersect;
    Console.WriteLine(pointWorld);
    // _target.GetComponent<Transform>().Position = new Vector3(pointWorld.X, pointWorld.Y, pointWorld.Z);
  }

  private Vector3 cCalculateRay() {
    var cursor = WindowGlobalState.GetMouseState();
    Vector2 normalized = GetNormalizedCoords(cursor.X, cursor.Y);
    Vector4 clipCoords = new Vector4(normalized.X, normalized.Y, -1f, 1f);
    Console.WriteLine(clipCoords); // <------
    Vector4 eyeCoords = GetEyeCoords(clipCoords);
    Vector3 worldRay = GetWorldPosition(eyeCoords);
    return worldRay;
  }

  private Vector3 GetWorldPosition(Vector4 eyeCoords) {
    Vector4 rayWorld = _camera.GetViewMatrix() * eyeCoords;
    Vector3 mouseRay = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z);
    return mouseRay.Normalized();
  }

  private Vector4 GetEyeCoords(Vector4 clipCoords) {
    Matrix4 invertedProjection = new Matrix4();
    Matrix4.Invert(_camera.GetProjectionMatrix(), out invertedProjection);
    Vector4 eyeCoords = invertedProjection * clipCoords;
    return new Vector4(eyeCoords.X, eyeCoords.Y, -1f, 0f);
  }

  private Vector2 GetNormalizedCoords(float mouseX, float mouseY) {
    float x = (2f * mouseX) / WindowGlobalState.GetWindow().Size.X - 1f;
    float y = (2f * mouseY) / WindowGlobalState.GetWindow().Size.Y - 1f;

    //Console.WriteLine($"{mouseX}, {mouseY}, {x}, {y}");
    return new Vector2(x, -y);
  }

  public Vector3 Ray {
    get { return _ray; }
  }

  public Vector3 GetRayPoint() {
    if (_camera == null) return new Vector3(0,0,0);

    Vector3 camPos = _camera.Owner!.GetComponent<Transform>().Position;
    Vector3 start = new Vector3(camPos.X, camPos.Y, camPos.Z);
    Vector3 scaledRay = new Vector3(_ray.X, _ray.Y, _ray.Z);
    return Vector3.Add(start, scaledRay);
  }
}

