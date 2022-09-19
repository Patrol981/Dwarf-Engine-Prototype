using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Dwarf.Engine.DataStructures;

public struct Vertex {
  public Vector3 Position;
  public Vector2 TextureCoords;
  public Vector3 Normal;
  public Color4 Colors;
  public Vector3 Tangent;
  public Vector3 Bitangent;
  public Vector3i JointIds;
  public Vector3 Weights;
  // public Joint JointHierarchy;
}