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

  public override MasterMesh Load(string path) {
    var scene = _assimpContext.ImportFile($"{path}.obj");

    _logger.Detach();

    List<DataStructures.Mesh> meshes = new();

    var node = scene.RootNode;

    foreach (var child in node.Children) {
      foreach (int index in child.MeshIndices) {
        List<Vector3> posList = new();
        List<Color4> colorList = new();
        List<Vector3> texList = new();
        List<Vector3> normalList = new();
        List<int> indices = new();

        var aMesh = scene.Meshes[index];

        foreach (Face face in aMesh.Faces) {
          for (int i = 0; i < face.IndexCount; i++) {
            int indice = face.Indices[i];

            indices.Add(indice);

            bool hasColors = aMesh.HasVertexColors(0);
            bool hasTexCoords = aMesh.HasTextureCoords(0);

            if (hasColors) {
              Color4 vertColor = FromColor(aMesh.VertexColorChannels[0][indice]);
              colorList.Add(vertColor);
            }
            if (aMesh.HasNormals) {
              Vector3 normal = FromVector(aMesh.Normals[indice]);
              normalList.Add(normal);
            }
            if (hasTexCoords) {
              Vector3 uvw = FromVector(aMesh.TextureCoordinateChannels[0][indice]);
              texList.Add(uvw);
            }
            Vector3 pos = FromVector(aMesh.Vertices[indice]);
            posList.Add(pos);
          }
        }

        DataStructures.Mesh mesh = new(
            posList,
            colorList,
            texList,
            normalList,
            indices,
            null!,
            null!,
            aMesh.Name
        );

        meshes.Add(mesh);
      }
    }

    MasterMesh masterMesh = new(meshes, Enums.MeshRenderType.FbxModel);
    return masterMesh;
  }

  public MasterMesh OldLoad(string path) {
    var scene = _assimpContext.ImportFile($"{path}.obj");

    Console.WriteLine(scene.RootNode);

    _logger.Detach();

    Console.WriteLine(scene.MeshCount);

    List<DataStructures.Mesh> meshes = new();

    for (int i = 0; i < scene.MeshCount; i++) {
      List<float> verts = new();
      List<OpenTK.Mathematics.Vector3> normals = new();
      List<OpenTK.Mathematics.Vector3> vec3Verts = new();
      List<int> indices = new();
      var aMesh = scene.Meshes[i];

      indices = (aMesh.GetIndices().ToList());

      for (int j = 0; j < aMesh.Vertices.Count; j++) {
        verts.AddRange(new float[] {
          aMesh.Vertices[j].X, aMesh.Vertices[j].Y, aMesh.Vertices[j].Z,
          aMesh.Normals[j].X, aMesh.Normals[j].Y, aMesh.Normals[j].Z,
          //aMesh.VertexColorChannels[i][indices[j]].R,
          //aMesh.VertexColorChannels[i][indices[j]].G,
          //aMesh.VertexColorChannels[0][indices[j]].B,
          aMesh.TextureCoordinateChannels[i][indices[j]].X,
          aMesh.TextureCoordinateChannels[i][indices[j]].Y,
          //aMesh.TextureCoordinateChannels[i][indices[j]].Z,
        });

        vec3Verts.Add(new OpenTK.Mathematics.Vector3(
          aMesh.Vertices[j].X, aMesh.Vertices[j].Y, aMesh.Vertices[j].Z)
        );
      }

      for (int j = 0; j < aMesh.Normals.Count; j++) {
        normals.Add(new OpenTK.Mathematics.Vector3(
          aMesh.Normals[j].X, aMesh.Normals[j].Y, aMesh.Normals[j].Z)
        );
      }
      /*
      DataStructures.Mesh mesh = new(
          vec3Verts,
          normals,
          verts,
          indices
        );
      */

      //meshes.Add(mesh);
    }

    MasterMesh masterMesh = new(meshes, Enums.MeshRenderType.WavefrontObjFile);
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
