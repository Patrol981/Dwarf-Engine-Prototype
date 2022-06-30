using System.Globalization;
using System.Text;
using OpenTK.Mathematics;

using Voxelized.DataStructures;
using Voxelized.Engine.Enums;

namespace Voxelized.Loaders;
public class SimpleObjLoader : MeshLoader {
  private List<Vector3>? _vertices, _normals;
  private List<int>? _vertIndices, _normalIndices;

  public override Mesh Load(string path) {
    _vertices = new();
    _normals = new();
    _vertIndices = new();
    _normalIndices = new();

    StreamReader reader = new StreamReader($"{path}.obj", Encoding.UTF8);
    string line;
    while ((line = reader.ReadLine()!) != null) {
      string[] parts = line.Split(" ");
      switch (parts[0]) {
        case "v":
          HandleVertices(parts);
          break;
        case "vt":
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

    List<Vector3> finalVertex = new();
    int len = _vertIndices.Count;

    for(int i=0; i< len; i++) {
      finalVertex.Add(_vertices[_vertIndices[i] - 1]);
      finalVertex.Add(_normals[_normalIndices[i] - 1]);
    }

    Mesh mesh = new Mesh(
      MeshRenderType.WavefrontObjFile,
      _vertices,
      _normals,
      finalVertex
    );

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
      _normalIndices!.Add(Convert.ToInt32(vertex[2]));
    }
  }
}
