using OpenTK.Mathematics;

namespace Voxelized.DataStructures;

public class Triangle {
  private Vertex[] _verticies;

  public Triangle(float[] verticies) {
    _verticies = new Vertex[3];

    _verticies[0].Position = new Vector3(verticies[0], verticies[1], verticies[2]);
    _verticies[1].Position = new Vector3(verticies[3], verticies[4], verticies[5]);
    _verticies[2].Position = new Vector3(verticies[6], verticies[7], verticies[8]);
  }

  public Vertex[] GetVertices() {
    return _verticies;
  }

  public void SetVerticies(float[] verticies) {
    _verticies[0].Position = new Vector3(verticies[0], verticies[1], verticies[2]);
    _verticies[1].Position = new Vector3(verticies[3], verticies[4], verticies[5]);
    _verticies[2].Position = new Vector3(verticies[6], verticies[7], verticies[8]);
  }
}