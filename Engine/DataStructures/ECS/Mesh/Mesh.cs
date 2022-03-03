namespace Voxelized.ECS;

public class Mesh : Component {
  private float[] _vertices;
  private int _vertexCount;

  public Mesh(float[] vertices, int vertexCount) {
    _vertices = vertices;
    _vertexCount = vertexCount;
  }

  public Mesh() {
    _vertexCount = 0;
    _vertices = new float[0];
  }

  public float[] GetVertecies() {
    return _vertices;
  }

  public int GetVertexCount() {
    return _vertexCount;
  }

  public void SetVerticies(float[] verticies) {
    _vertices = verticies;
  }

  public void SetVertexCount(int vertexCount) {
    _vertexCount = vertexCount;
  }
}