using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using OpenTK.Mathematics;
using Voxelized.DataStructures;
using Voxelized.ECS;

namespace Voxelized.Loaders;

public class ObjLoader : MeshLoader {
  private string current_material;
  public List<Vector3> Vertices, Normals, ColorPerIndex, FinalVertexArray;
  public List<int> Indices, NormalIndices;
  private Dictionary<string, Vector3> Materials;
  private Dictionary<string, int> MaterialsIndices;
  public Vector3 Position;
  public int counter;
  public float MaxX, MinX, MaxY, MinY, MaxZ, MinZ;
  public Vector3 ExtremeMin, ExtremeMax;
  public Mesh LoadObj(string path) {
    string line;

    Vertices = new();
    Normals = new();
    ColorPerIndex = new();
    FinalVertexArray = new();
    Indices = new();
    NormalIndices = new();
    Materials = new();
    MaterialsIndices = new();
    current_material = "";

    StreamReader materialreader = new StreamReader($"{path}.mtl", Encoding.UTF8);
    string current_material_name = "", material_line;
    while ((material_line = materialreader.ReadLine()) != null) {
      string[] substrings = material_line.Split(" ");
      switch (substrings[0]) {
        case "newmtl":
          current_material_name = substrings[1];
          break;
        case "Kd":
          var color = new Vector3(Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
              Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture),
              Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture));
          Materials.Add(current_material_name, color);
          MaterialsIndices.Add(current_material_name, Materials.Count - 1);
          break;
      }
    }

    StreamReader reader = new StreamReader($"{path}.obj", Encoding.UTF8);
    while ((line = reader.ReadLine()) != null) {
      string[] substrings = line.Split(" ");
      switch (substrings[0]) {
        case "v":
          var XPos = Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture);
          var YPos = Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture);
          var ZPos = Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture);
          if (XPos > MaxX)
            MaxX = XPos;
          if (XPos < MinX)
            MinX = XPos;
          if (YPos > MaxY)
            MaxY = YPos;
          if (YPos < MinY)
            MinY = YPos;
          if (ZPos > MaxZ)
            MaxZ = ZPos;
          if (ZPos < MinZ)
            MinZ = ZPos;
          Vertices.Add((XPos, YPos, ZPos));
          break;
        case "vt":
          //TextureCoordinates.Add((Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
          //Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture)));
          break;
        case "vn":
          Normals.Add((Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
              Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture),
              Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture)));
          break;
        case "f":
          SplitFLine(substrings);
          break;
        case "usemtl":
          current_material = substrings[1];
          break;
      }
    }

    ExtremeMin = (MinX, MinY, MinZ);
    ExtremeMax = (MaxX, MaxY, MaxZ);

    // Mesh mesh = new Mesh(Engine.Enums.MeshRenderType.WavefrontObjFile, Normals, ColorPerIndex, Vertices, Indices, NormalIndices, Materials, MaterialsIndices);
    Mesh mesh = new Mesh();

    return mesh;
  }

  private void SplitFLine(string[] line) {
    for (int i = 1; i < line.Length; i++) {
      string[] vertex = line[i].Split("/");
      Indices.Add(Convert.ToInt32(vertex[0]) - 1);
      NormalIndices.Add(Convert.ToInt32(vertex[2]) - 1);
      ColorPerIndex.Add(Materials[current_material]);
    }
  }
}