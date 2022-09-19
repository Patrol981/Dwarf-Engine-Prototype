using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;

namespace Dwarf.Engine.Generators;
public class Chunk : Component {
  private float _size;

  public Chunk() { }

  public Chunk(float size) {
    _size = size;
  }

  /// <summary>
  /// Method suggested to use only when creating multiple chunks
  /// </summary>
  /// <returns>Mesh</returns>
  public static Mesh SetupMesh(float size) {
    List<Vector3> vertices = new();
    List<Vector3> normals = new();
    List<Vector2> textureCoords = new();

    List<int> indices = new();

    int vertexCount = 32;
    var vertexArray = new List<Vertex>();

    for (int i = 0; i < vertexCount; i++) {
      for(int j= 0; j < vertexCount; j++) {
        var vert = new Vector3(
          (float)j / ((float)vertexCount - 1) * size,
          0,
          (float)i / ((float)vertexCount - 1) * size
        );
        var normal = new Vector3(
          0,
          1,
          0
        );
        var textureCoord = new Vector2(
          (float)j / ((float)vertexCount - 1),
          (float)i / ((float)vertexCount - 1)
        );

        vertices.Add(vert);
        normals.Add(normal);
        textureCoords.Add(textureCoord);
        var vertex = new Vertex();
        vertex.TextureCoords = textureCoord;
        vertex.Position = vert;
        vertex.Normal = normal;
        vertexArray.Add(vertex);
      }
    }

    for(int gz = 0; gz < vertexCount -1; gz++) {
      for(int gx = 0; gx < vertexCount - 1; gx++) {
        int topLeft = (gz * vertexCount) + gx;
        int topRight = topLeft + 1;
        int bottomLeft = ((gz + 1) * vertexCount) + gx;
        int bottomRight = bottomLeft + 1;
        indices.Add(topLeft);
        indices.Add(bottomLeft);
        indices.Add(topRight);
        indices.Add(topRight);
        indices.Add(bottomLeft);
        indices.Add(bottomRight);
      }
    }

    var mesh = new Mesh(vertexArray, indices);
    return mesh;
  }
}
