using OpenTK.Mathematics;

using Voxelized.Engine.ECS;
using Voxelized.Engine.DataStructures;

namespace Voxelized.Engine.Loaders;

public class STLMeshLoader : MeshLoader {
  public override MasterMesh Load(string path) {
    List<float> meshes = new List<float>();
    int triangles = 0;
    int byteIndex = 0;
    byte[] fileBytes = File.ReadAllBytes(path);

    byte[] tmp = new byte[4];

    if(fileBytes.Length > 120) {
      tmp[0] = fileBytes[80];
      tmp[1] = fileBytes[81];
      tmp[2] = fileBytes[82];
      tmp[3] = fileBytes[83];

      triangles = System.BitConverter.ToInt32(tmp, 0);

      byteIndex = 84;
      for(int i=0; i<triangles; i++) {
        Vector3 n = FromBuff(fileBytes, byteIndex);
        byteIndex += 12;
        Vector3 v1 = FromBuff(fileBytes, byteIndex);
        byteIndex += 12;
        Vector3 v2 = FromBuff(fileBytes, byteIndex);
        byteIndex += 12;
        Vector3 v3 = FromBuff(fileBytes, byteIndex);
        byteIndex += 12;

        byteIndex += 2;

        meshes.AddRange(new float[] {
          v1.X, v1.Y, v1.Z, n.X, n.Y, n.Z,
          v2.X, v2.Y, v2.Z, n.X, n.Y, n.Z,
          v3.X, v3.Y, v3.Z, n.X, n.Y, n.Z,
        });
      }
    }

    // Mesh mesh = new Mesh(meshes.ToArray(), triangles * 3);
    Mesh mesh = new();
    MasterMesh masterMesh = new MasterMesh();
    return masterMesh;
  }

  Vector3 FromBuff(byte[] fileBytes, int byteIndex) {
    Vector3 v;
      v.X = System.BitConverter.ToSingle(
        new byte[] {
          fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3]
        }, 0
      );
      byteIndex += 4;

      v.Y = System.BitConverter.ToSingle(
        new byte[] {
          fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3]
        }, 0
      );
      byteIndex += 4;

      v.Z = System.BitConverter.ToSingle(
        new byte[] {
          fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3]
        }, 0
      );
      byteIndex += 4;

      return v;
  }
}