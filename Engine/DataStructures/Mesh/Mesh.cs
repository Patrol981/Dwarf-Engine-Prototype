using OpenTK.Mathematics;

using Voxelized.Engine.ECS;
using Voxelized.Engine.Enums;

namespace Voxelized.Engine.DataStructures;

public class Mesh : Component {
  private List<float> _vertexArray;
  private List<Vector3> _normals;
  private List<Vector3> _vertices;
  private List<int> _indices;

  private Voxelized.Engine.Textures.Texture _texture;

  private int _bufferDataCount;
  private int _packageStrideSize;
  private int _drawCount;

  private MeshRenderType _meshRenderType;

  public Mesh(
    MeshRenderType meshRenderType,
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

    _meshRenderType = meshRenderType;

    _texture = texture;

    CalculateVertexArray(meshRenderType);
  }

  public Mesh() {
    _vertices = new();
    _normals = new();
    _indices = new();
    _vertexArray = new();

    _meshRenderType = MeshRenderType.Standard;
  }

  public void CalculateVertexArray(MeshRenderType meshRenderType) {
    // if (_vertexArray.Count > 0) return _vertexArray

    switch (meshRenderType) {
      case MeshRenderType.Standard: {
          _bufferDataCount = _vertexArray.Count * sizeof(float);
          _packageStrideSize = 6 * sizeof(float);
          _drawCount = _vertexArray.Count;
          break;
        }

      case MeshRenderType.WavefrontObjFile: {
          _bufferDataCount = _vertexArray.Count * sizeof(float);
          _packageStrideSize = 8 * sizeof(float);
          _drawCount = _vertexArray.Count;
          break;
        }

      case MeshRenderType.Terrain: {
          _bufferDataCount = _vertexArray.Count * sizeof(float);
          _packageStrideSize = 6 * sizeof(float);
          _drawCount = _vertexArray.Count;
          break;
        }
    }
  }

  public List<float> GetVertexArray() {
    // var list = CalculateVertexArray(_meshRenderType);
    return _vertexArray;
  }

  public List<float> VertexArray {
    get { return _vertexArray; }
    set { _vertexArray = value; }
  }

  public MeshRenderType MeshRenderType {
    get { return _meshRenderType; }
    set { _meshRenderType = value; }
  }

  public int BufferDataCount {
    get { return _bufferDataCount; }
  }

  public int PackageStrideSize {
    get { return _packageStrideSize; }
  }

  public int DrawCount {
    get{ return _drawCount; }
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