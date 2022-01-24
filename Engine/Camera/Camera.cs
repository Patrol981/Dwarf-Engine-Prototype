using OpenTK.Mathematics;
using System;

namespace Voxelized.Cameras;
public abstract class Camera {
  // Those vectors are directions pointing outwards from the camera to define how it rotated.
  internal Vector3 _front = -Vector3.UnitZ;

  internal Vector3 _up = Vector3.UnitY;

  internal Vector3 _right = Vector3.UnitX;

  internal float _pitch;

  internal float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.
  internal float _fov = MathHelper.PiOver2;

  public Vector3 Position { get; set; }

  public float AspectRatio { internal get; set; }

  public Vector3 Front => _front;

  public Vector3 Up => _up;

  public Vector3 Right => _right;

  public Camera(Vector3 position, float aspectRatio) {
    Position = position;
    AspectRatio = aspectRatio;
  }

  public float Pitch {
    get => MathHelper.RadiansToDegrees(_pitch);
    set {
      var angle = MathHelper.Clamp(value, -89f, 89f);
      _pitch = MathHelper.DegreesToRadians(angle);
      UpdateVectors();
    }
  }

  public float Yaw {
    get => MathHelper.RadiansToDegrees(_yaw);
    set {
      _yaw = MathHelper.DegreesToRadians(value);
      UpdateVectors();
    }
  }
  public float Fov {
    get => MathHelper.RadiansToDegrees(_fov);
    set {
      var angle = MathHelper.Clamp(value, 1f, 45f);
      _fov = MathHelper.DegreesToRadians(angle);
    }
  }

  public Matrix4 GetViewMatrix() {
    return Matrix4.LookAt(Position, Position + _front, _up);
  }

  public Matrix4 GetProjectionMatrix() {
    return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
  }

  internal abstract void UpdateVectors();
}