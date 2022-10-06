using OpenTK.Mathematics;

namespace Dwarf.Engine.ECS;

public class Transform : Component {
  public Vector3 Position;
  public Vector3 Rotation;

  public Transform() {
    Position = new Vector3(0, 0, 0);
    Rotation = new Vector3(0, 0, 0);
  }

  public Transform(Vector3 position) {
    Position = position;
    Rotation = new Vector3(0, 0, 0);
  }

  public Transform(Vector3 position, Vector3 rotation) {
    Position = position;
    Rotation = rotation;
  }

  public void IncreasePosition(Vector3 position) {
    Position.X += position.X;
    Position.Y += position.Y;
    Position.Z += position.Z;
  }

  public void IncreaseRotation(Vector3 rotation) {
    Rotation.X += rotation.X;
    Rotation.Y += rotation.Y;
    Rotation.Z += rotation.Z;
  }

}