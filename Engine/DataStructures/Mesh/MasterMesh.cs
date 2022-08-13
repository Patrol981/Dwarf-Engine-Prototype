using System.Runtime.CompilerServices;

using Dwarf.Engine.ECS;
using Dwarf.Engine.Enums;

namespace Dwarf.Engine.DataStructures;
public class MasterMesh : Component {
  private List<Mesh> _meshes;

  private List<Vertex> _vertexArray = new();
  private List<int> _indices = new();
  private List<int> _uvs = new();

  private int _bufferDataCount = 0;
  private int _packageStrideSize = 0;
  private int _drawCount = 0;
  private int _indicesCount = 0;
  private MeshRenderType _meshRenderType;

  public Textures.Texture Texture;

  public MasterMesh() {
    _meshes = new List<Mesh>();
    _meshRenderType = MeshRenderType.Standard;
  }

  public MasterMesh(List<Mesh> meshes, MeshRenderType meshRenderType) {
    _meshes = meshes;
    _meshRenderType = meshRenderType;
  }

  [Obsolete]
  public void RecalculateMeshes() {
    _vertexArray = new List<Vertex>();

    for(int i = 0; i < _meshes.Count; i++) {
      _vertexArray.AddRange(_meshes[i].VertexArray);
      if (_meshes[i].Indices != null) {
        _indices.AddRange(_meshes[i].Indices);
      }
    }

    Console.WriteLine(_meshRenderType);
    if( _meshRenderType == MeshRenderType.FbxModel ) {
      Console.WriteLine(_vertexArray);
    }
    

    CalculateVertexArray(_meshRenderType);
  }

  [Obsolete]
  public void CalculateVertexArray(MeshRenderType meshRenderType) {
    // GL.VertexArrayVertexBuffer(_vertexArray, inPos, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
    switch (meshRenderType) {
      case MeshRenderType.Standard: {
          _bufferDataCount = _vertexArray.Count * Unsafe.SizeOf<Vertex>();
          _packageStrideSize = 5 * Unsafe.SizeOf<Vertex>();
          _drawCount = 36;
          break;
        }

      case MeshRenderType.WavefrontObjFile: {
          _bufferDataCount = _vertexArray.Count * Unsafe.SizeOf<Vertex>();
          _packageStrideSize = 9 * Unsafe.SizeOf<Vertex>();
          _drawCount = _vertexArray.Count;
          break;
        }

      case MeshRenderType.FbxModel: {
          // _bufferDataCount = _vertexArray.Count * sizeof(float);
          _bufferDataCount = _vertexArray.Count * Unsafe.SizeOf<Vertex>();
          _packageStrideSize = Unsafe.SizeOf<Vertex>();
          _drawCount = _vertexArray.Count;
          _indicesCount = _indices.Count;
          break;
        }

      case MeshRenderType.Terrain: {
          _bufferDataCount = _vertexArray.Count * Unsafe.SizeOf<Vertex>();
          _packageStrideSize = 6 * Unsafe.SizeOf<Vertex>();
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

  public List<Vertex> VertexArray {
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

  public int IndicesCount {
    get { return _indicesCount; }
  }

}
