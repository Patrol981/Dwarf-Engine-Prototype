using OpenTK.Mathematics;

namespace Voxelized.ECS;

class Material : Component {
  private Vector3 _color;

  public Material(Vector3 color) {
    _color = color;
  }

  public Material() {
    _color = new Vector3(0,0,0);
  }

  public Vector3 GetColor() {
    return _color;
  }

  public void SetColor(Vector3 color) {
    _color = color;
  }
}