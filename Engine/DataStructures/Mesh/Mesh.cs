using OpenTK.Mathematics;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;
using Dwarf.Engine.Textures;

namespace Dwarf.Engine.DataStructures;

public class Mesh : Component {
  private List<Vertex> _vertexArray;
  private List<TextureStruct> _textureArray;

  private List<Vector3> _normals;
  private List<Vector3> _vertices;
  private List<Vector3> _uvs;
  private List<int> _indices;

  private string _name;

  private Texture _texture;

  public Mesh(List<Vertex> vertexArray) {
    _vertexArray = vertexArray;
  }

  public Mesh(
    List<Vertex> vertexArray,
    List<int> indices,
    List<TextureStruct> textures
  ) {
    _vertexArray = vertexArray;
    _textureArray = textures;
    _indices = indices;
  }

  public Mesh(
    List<Vertex> vertexArray,
    List<int> indices
  ) {
    _vertexArray = vertexArray;
    _indices = indices;
  }

  public Mesh(
    List<Vertex> vertexArray,
    List<int> indices,
    Texture texture,
    string name = null!
  ) {
    _vertexArray = vertexArray;
    _indices = indices;
    _texture = texture;
    _name = name;
  }

  public Mesh(
    List<Vector3> posList,
    List<Color4> colorList,
    List<Vector3> texList,
    List<Vector3> normalList,
    List<int> indices = null!,
    List<TextureStruct> textureList = null!,
    Texture texture = null!,
    string name = null!
  ) {
    _normals = normalList;
    _vertices = posList;
    _uvs = texList;
    _indices = indices;
    _vertexArray = new List<Vertex>();

    var count = posList.Count;

    for (int i=0; i<count; i++) {
      Vertex v = new Vertex();

      v.Position = posList[i];
      v.Normal = normalList[i];
      v.TextureCoords = new Vector2(texList[i].X, texList[i].Y);

      _vertexArray.Add(v);
    }

    _texture = texture;

    _name = name;
  }

  [Obsolete]
  public Mesh(
    List<Vector3> vertices,
    List<Vector3> normals,
    List<Vertex> vertexArray,
    List<int> indices = null!,
    //List<int> uvs = null!,
    Texture texture = null!
  ) {
    _vertices = vertices;
    _normals = normals;
    _vertexArray = vertexArray;
    _indices = indices;

    _texture = texture;

    if(texture != null) {
      // _shader = new Shader("Shaders/texture.vert", "Shaders/texture.frag");
    }
  }

  public Mesh() {
    _vertices = new();
    _normals = new();
    _indices = new();
    _vertexArray = new();
  }

  public List<Vertex> VertexArray {
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

  public List<Vector3> Uvs {
    get { return _uvs; }
    set { _uvs = value; }
  }

  public Texture Texture {
    get { return _texture; }
    set { _texture = value; }
  }

  public List<TextureStruct> TextureArray {
    get { return _textureArray; }
  }

  public string Name {
    get { return _name; }
  }
}