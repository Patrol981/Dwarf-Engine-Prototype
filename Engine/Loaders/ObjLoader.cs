using Assimp;
using OpenTK.Mathematics;
using Dwarf.Engine.DataStructures;

namespace Dwarf.Engine.Loaders;
public class ObjLoader : MeshLoader {
  private readonly AssimpContext _assimpContext;
  private readonly ConsoleLogStream _logger;

  public ObjLoader() {
    _assimpContext = new AssimpContext();

    Assimp.LogStream.IsVerboseLoggingEnabled = true;
    _logger = new Assimp.ConsoleLogStream();
    _logger.Attach();
  }


  public override MasterMesh Load(string path, bool useTextures = true) {
    string[] pathElements = path.Split('/');
    string withoutExt = path.Substring(0, path.Length - 4);
    string filename = $"{path}/{pathElements[pathElements.Length - 1]}.obj";
    var scene = _assimpContext.ImportFile(path);

    _logger.Detach();

    List<DataStructures.Mesh> meshes = new();

    var node = scene.RootNode;
    int x = 0;
    foreach (var child in node.Children) {
      foreach (int index in child.MeshIndices) {
        List<Vector3> posList = new();
        List<Color4> colorList = new();
        List<Vector3> texList = new();
        List<Vector3> normalList = new();
        List<int> indices = new();

        var aMesh = scene.Meshes[index];
        var vertexArray = new List<Vertex>();

        foreach (Face face in aMesh.Faces) {
          for (int i = 0; i < face.IndexCount; i++) {
            int indice = face.Indices[i];

            indices.Add(indice);
            var vertex = new Vertex();

            bool hasColors = aMesh.HasVertexColors(0);
            bool hasTexCoords = aMesh.HasTextureCoords(0);

            if (hasColors) {
              Color4 vertColor = FromColor(aMesh.VertexColorChannels[0][indice]);
              //colorList.Add(vertColor);
              vertex.Colors = vertColor;
            }
            if (aMesh.HasNormals) {
              Vector3 normal = FromVector(aMesh.Normals[indice]);
              //normalList.Add(normal);
              vertex.Normal = normal;
            }
            if (hasTexCoords) {
              Vector3 uvw = FromVector(aMesh.TextureCoordinateChannels[0][indice]);
              //texList.Add(uvw);
              vertex.TextureCoords = uvw.Xy;
            }
            Vector3 pos = FromVector(aMesh.Vertices[indice]);
            //posList.Add(pos);
            vertex.Position = pos;

            vertexArray.Add(vertex);
          }
        }

        DataStructures.Mesh mesh = new(
          vertexArray,
          indices,
          useTextures == true ? Textures.Texture.FastTextureLoad($"{withoutExt}.png") : null!,
          //useTextures == true ? Textures.Texture.LoadFromFile($"{path}/{pathElements[pathElements.Length - 1]}.png") : null!,
          aMesh.Name
        );

        x++;
        meshes.Add(mesh);
      }
    }

    MasterMesh masterMesh = new(meshes);
    return masterMesh;
  }

  private Vector3 FromVector(Vector3D vec) {
    Vector3 v;
    v.X = vec.X;
    v.Y = vec.Y;
    v.Z = vec.Z;
    return v;
  }

  private Color4 FromColor(Color4D color) {
    Color4 c;
    c.R = color.R;
    c.G = color.G;
    c.B = color.B;
    c.A = color.A;
    return c;
  }
}
