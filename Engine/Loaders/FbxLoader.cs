using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;

using Voxelized.Engine.DataStructures;
using Voxelized.Engine.Enums;

namespace Voxelized.Engine.Loaders;
public class FbxLoader : MeshLoader {
  private readonly AssimpContext _assimpContext;
  private readonly ConsoleLogStream _logger;

  public FbxLoader() {
    _assimpContext = new AssimpContext();

    Assimp.LogStream.IsVerboseLoggingEnabled = true;
    _logger = new Assimp.ConsoleLogStream();
    _logger.Attach();
  }


  public override MasterMesh Load(string path) {
    var scene = _assimpContext.ImportFile($"{path}.fbx");

    _logger.Detach();

    Console.WriteLine(scene.MeshCount);

    List<DataStructures.Mesh> meshes = new();

    for(int i = 0; i < scene.MeshCount; i++) {
      List<float> verts = new();
      List<OpenTK.Mathematics.Vector3> normals = new();
      List<OpenTK.Mathematics.Vector3> vec3Verts = new();
      var aMesh = scene.Meshes[i];

      for (int j = 0; j < aMesh.Vertices.Count; j++) {
        verts.AddRange(new float[] {
          aMesh.Vertices[j].X, aMesh.Vertices[j].Y, aMesh.Vertices[j].Z,
          aMesh.Normals[j].X, aMesh.Normals[j].Y, aMesh.Normals[j].Z
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

      DataStructures.Mesh mesh = new(
        vec3Verts,
        normals,
        verts
      );

      meshes.Add(mesh);
    }


    MasterMesh masterMesh = new(meshes, Enums.MeshRenderType.FbxModel);
    return masterMesh;
  }
}
