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

  private bool _render = true;

  public MasterMesh() {
    _meshes = new List<Mesh>();
    _meshRenderType = MeshRenderType.Mesh;
  }

  public MasterMesh(List<Mesh> meshes, MeshRenderType renderType = MeshRenderType.Mesh) {
    _meshes = meshes;
    _meshRenderType = renderType;
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

  public bool Render {
    get { return _render; }
    set { _render = value; }
  }

}
