using System.Globalization;
using System.Text;
using OpenTK.Mathematics;

using Voxelized.Engine.DataStructures;
using Voxelized.Engine.Enums;

namespace Voxelized.Engine.Loaders;
public class SimpleObjLoader : MeshLoader {
  private List<Vector3>? _vertices, _normals;
  private List<int>? _vertIndices, _normalIndices, _textureIndices;
  private List<Vector2>? _textures;

  public override Mesh Load(string path) {
    _vertices = new();
    _normals = new();
    _vertIndices = new();
    _normalIndices = new();
    _textures = new();
    _textureIndices = new();

    StreamReader reader = new StreamReader($"{path}.obj", Encoding.UTF8);
    string line;
    while ((line = reader.ReadLine()!) != null) {
      string[] parts = line.Split(" ");
      switch (parts[0]) {
        case "v":
          HandleVertices(parts);
          break;
        case "vt":
          HandleTextures(parts);
          break;
        case "vn":
          HandleNormals(parts);
          break;
        case "f":
          HandleIndices(parts);
          break;
        case "usemtl":
          break;
        default:
          break;
      }
    }

    List<Vector3> finalVertexVec3 = new();
    List<float> finalVertexFloat = new();
    int len = _vertIndices.Count;

    for(int i=0; i< len; i++) {
      finalVertexVec3.Add(_vertices[_vertIndices[i] - 1]);
      finalVertexVec3.Add(_normals[_normalIndices[i] - 1]);
      finalVertexVec3.Add(
        new Vector3(_textures[_textureIndices[i] - 1].X,
        _textures[_textureIndices[i] - 1].Y, 
        0
      ));
    }

    for (int i=0; i < finalVertexVec3.Count; i+=3) {
      finalVertexFloat.AddRange(new float[] {
        finalVertexVec3[i].X, finalVertexVec3[i].Y, finalVertexVec3[i].Z, // position
        finalVertexVec3[i+1].X, finalVertexVec3[i+1].Y, finalVertexVec3[i+1].Z, // normal
        finalVertexVec3[i+2].X, finalVertexVec3[i+2].Y // texture
      });
    }

    Mesh mesh = new Mesh(
      MeshRenderType.WavefrontObjFile,
      _vertices,
      _normals,
      finalVertexFloat,
      null!,
      Textures.Texture.LoadFromFile($"{path}.png")
    );

    mesh.Texture.Use(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);

    return mesh;
  }

  internal void HandleVertices(string[] parts) {
    var x = Convert.ToSingle(parts[1], CultureInfo.InvariantCulture);
    var y = Convert.ToSingle(parts[2], CultureInfo.InvariantCulture);
    var z = Convert.ToSingle(parts[3], CultureInfo.InvariantCulture);
    _vertices!.Add(new Vector3(x, y, z));
  }

  internal void HandleNormals(string[] parts) {
    _normals!.Add(new Vector3(
      Convert.ToSingle(parts[1], CultureInfo.InvariantCulture),
      Convert.ToSingle(parts[2], CultureInfo.InvariantCulture),
      Convert.ToSingle(parts[3], CultureInfo.InvariantCulture)
    ));
  }

  internal void HandleIndices(string[] parts) {
    for (int i = 1; i < parts.Length; i++) {
      string[] vertex = parts[i].Split("/");
      _vertIndices!.Add(Convert.ToInt32(vertex[0]));
      _textureIndices!.Add(Convert.ToInt32(vertex[1]));
      _normalIndices!.Add(Convert.ToInt32(vertex[2]));
    }
  }

  internal void HandleTextures(string[] parts) {
    _textures!.Add(new Vector2( 
      Convert.ToSingle(parts[1], CultureInfo.InvariantCulture),
      Convert.ToSingle(parts[2], CultureInfo.InvariantCulture)
    ));
  }
}
