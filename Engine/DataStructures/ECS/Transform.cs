using OpenTK.Mathematics;

namespace Voxelized.ECS;

class Transform : Component {
  public Vector3 Position;

  public Transform() {
    Position = new Vector3(0,0,0);
  }

  public Transform(Vector3 position) {
    Position = position;
  }

}