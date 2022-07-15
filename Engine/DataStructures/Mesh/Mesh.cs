using OpenTK.Mathematics;

namespace Voxelized.Engine.DataStructures;

public class Mesh {
  private List<float> _vertexArray;
  private List<Vector3> _normals;
  private List<Vector3> _vertices;
  private List<int> _indices;

  private Textures.Texture _texture;

  public Mesh(
    List<Vector3> vertices,
    List<Vector3> normals,
    List<float> vertexArray,
    List<int> indices = null!,
    Textures.Texture texture = null!
  ) {
    _vertices = vertices;
    _normals = normals;
    _vertexArray = vertexArray;
    _indices = indices;

    _texture = texture;
  }

  public Mesh() {
    _vertices = new();
    _normals = new();
    _indices = new();
    _vertexArray = new();
  }

  public List<float> VertexArray {
    get { return _vertexArray; }
    set { _vertexArray = value; }
  }

  public List<Vector3> Normals {
    get { return _normals; }
    set { _normals = value; }
  }

  public List<Vector3> Vertices {
    get { return _vertices; }
    set { _vertices = value; }
  }

  public List<int> Indices {
    get { return _indices; }
    set { _indices = value; }
  }

  public Textures.Texture Texture {
    get { return _texture; }
    set { _texture = value; }
  }
}