using OpenTK.Mathematics;

namespace Dwarf.Engine.DataStructures;

public struct Joint {
  public List<Joint> Children;
  public int Id;
  public Matrix4 Tranform;
}