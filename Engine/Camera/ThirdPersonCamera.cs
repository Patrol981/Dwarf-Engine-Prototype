using OpenTK.Mathematics;

namespace Dwarf.Engine.Cameras;
public class ThirdPersonCamera : Camera, ICamera {
  public ThirdPersonCamera() { }

  public ThirdPersonCamera(Vector3 position, float aspectRatio) : base(position, aspectRatio) { }

  public void HandleMovement() {

  }

  internal override void UpdateVectors() {

  }

  public float GetAspectRatio() {
    return AspectRatio;
  }

  public void SetAspectRatio(float aspectRatio) {
    AspectRatio = aspectRatio;
  }
}
