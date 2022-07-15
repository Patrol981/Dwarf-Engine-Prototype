using Voxelized.Engine.ECS;
using Voxelized.Engine.Enums;

namespace Voxelized.Engine.DataStructures;
public class MasterMesh : Component {
  private List<Mesh> _meshes;

  private List<float> _vertexArray = new();
  private List<int> _indices = new();

  private int _bufferDataCount = 0;
  private int _packageStrideSize = 0;
  private int _drawCount = 0;
  private MeshRenderType _meshRenderType;

  public MasterMesh() {
    _meshes = new List<Mesh>();

    _meshRenderType = MeshRenderType.Standard;
  }

  public MasterMesh(List<Mesh> meshes, MeshRenderType meshRenderType) {
    _meshes = meshes;
    _meshRenderType = meshRenderType;

    RecalculateMeshes();
  }

  public void RecalculateMeshes() {
    for (int i = 0; i < _meshes.Count; i++) {
      _vertexArray.AddRange(_meshes[i].VertexArray);
      if (_meshes[i].Indices != null) {
        _indices.AddRange(_meshes[i].Indices);
      }
    }

    CalculateVertexArray(_meshRenderType);
  }

  public void CalculateVertexArray(MeshRenderType meshRenderType) {
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

      case MeshRenderType.FbxModel: {
          _bufferDataCount = _vertexArray.Count * sizeof(float);
          _packageStrideSize = 6 * sizeof(float);
          _drawCount = _vertexArray.Count;
          break;
        }

      case MeshRenderType.Terrain: {
          _bufferDataCount = _vertexArray.Count * sizeof(float);
          _packageStrideSize = 6 * sizeof(float);
          _drawCount = _vertexArray.Count;
          break;
        }

      default: {
          break;
        }
    }
  }

  public List<Mesh> Meshes {
    get { return _meshes; }
    set { _meshes = value; }
  }

  public List<int> Indices {
    get { return _indices; }
    set { _indices = value; }
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
    get { return _drawCount; }
  }

}
