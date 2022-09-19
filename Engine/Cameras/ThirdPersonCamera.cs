using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Dwarf.Engine.Cameras;
public class ThirdPersonCamera : Camera, ICamera {
  private Entity _followTarget;
  private float _distanceFromTarget = 2.5f;
  private float _angleAroundTarget = 0f;
  
  public ThirdPersonCamera() {
    _followTarget = null!;
  }

  public ThirdPersonCamera(float aspectRatio, Entity followTarget) : base(aspectRatio) {
    _followTarget = followTarget;
  }

  private void CalculateZoom() {
    float zoomWheel = WindowGlobalState.GetMouseState().ScrollDelta.Y * 0.1f;
    _distanceFromTarget -= zoomWheel;
  }

  private void CalculatePitch(float deltaY) {
    float pichChange = deltaY * 0.1f;
    _pitch -= pichChange;
  }

  private void CalculateAngle(float deltaX) {
    float angleChange = deltaX * 0.3f;
    _angleAroundTarget -= angleChange;
  }

  private float CalculateHorizontalDistance() {
    return _distanceFromTarget * MathF.Cos(MathHelper.DegreesToRadians(this.Pitch));
  }

  private float CalculateVerticalDistance() {
    return _distanceFromTarget * MathF.Sin(MathHelper.DegreesToRadians(this.Pitch));
  }

  private void CalculateCameraPosition(float horizontal, float vertical) {
    float theta = _angleAroundTarget;
    float offectX = (float)(horizontal * MathF.Sin(MathHelper.DegreesToRadians(theta)));
    float offsetZ = (float)(horizontal * MathF.Cos(MathHelper.DegreesToRadians(theta)));
    Owner!.GetComponent<Transform>().Position.X = FollowTarget.GetComponent<Transform>().Position.X - offectX;
    Owner!.GetComponent<Transform>().Position.Z = FollowTarget.GetComponent<Transform>().Position.Z - offsetZ;
    Owner!.GetComponent<Transform>().Position.Y = FollowTarget.GetComponent<Transform>().Position.Y + vertical + 1.3f;

  }

  public void HandleMovement() {
    if (CameraGlobalState.GetFirstMove()) {
      CameraGlobalState.SetFirstMove(false);
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));
    } else {
      var deltaX = WindowGlobalState.GetMouseState().Position.X - CameraGlobalState.GetLastPosition().X;
      var deltaY = WindowGlobalState.GetMouseState().Position.Y - CameraGlobalState.GetLastPosition().Y;
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));

      if (WindowGlobalState.GetKeyboardState().IsKeyPressed(Keys.F)) {
        var currentCursorState = WindowGlobalState.GetCursorVisible();
        switch (currentCursorState) {
          case true:
            WindowGlobalState.SetCursorVisible(false);
            break;
          case false:
            WindowGlobalState.SetCursorVisible(true);
            break;
        }
      }

      if (WindowGlobalState.GetCursorVisible()) return;

      CalculateZoom();
      // this.Pitch -= deltaY * CameraGlobalState.GetSensitivity();
      //_angleAroundTarget -= MathHelper.ClampAngle(deltaX * CameraGlobalState.GetSensitivity());
      //CalculatePitch(deltaY);
      CalculateAngle(deltaX);

      float horizontal = CalculateHorizontalDistance();
      float vertical = CalculateVerticalDistance();

      //var targetPos = FollowTarget.GetComponent<Transform>().Position;
      //Owner.GetComponent<Transform>().Position = new(targetPos.X, targetPos.Y + 1.5f, targetPos.Z - 2);
      //this.Yaw = 1 - (_angleAroundTarget);
      //
      CalculateCameraPosition(horizontal, vertical);

      this.Yaw = 90 - _angleAroundTarget;
      
    }
  }

  internal override void UpdateVectors() {
    if (WindowGlobalState.GetCursorVisible()) {
      return;
    }
    // First, the front matrix is calculated using some basic trigonometry.

    _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
    _front.Y = MathF.Sin(_pitch);
    _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

    // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
    _front = Vector3.Normalize(_front);

    _right = (this.GetViewMatrix() * new Vector4(1, 0, 0, 0)).Xyz;
    _up = (this.GetViewMatrix() * new Vector4(0, 1, 0, 0)).Xyz;
  }

  public float GetAspectRatio() {
    return AspectRatio;
  }

  public void SetAspectRatio(float aspectRatio) {
    AspectRatio = aspectRatio;
  }

  public Entity FollowTarget {
    get { return _followTarget; }
    set { _followTarget = value; }
  }

  public float Angle {
    get { return _angleAroundTarget; }
  }

  public float ScrollDistance {
    get { return _distanceFromTarget; }
  }
}
