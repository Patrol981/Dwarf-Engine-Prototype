using OpenTK.Mathematics;

namespace Voxelized.DataStructures;

public struct Vertex {
  public Vector3 Position;
  public Vector3 Normal;
  public Vector2 TextureCoords;
  public Vector3 Tangent;
  public Vector3 Bitangent;
  int[] BoneIDs;
  float[] BoneWeights;
}