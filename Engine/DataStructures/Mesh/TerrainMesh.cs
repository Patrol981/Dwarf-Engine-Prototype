using Dwarf.Engine.Textures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.DMath;
using OpenTK.Mathematics;

namespace Dwarf.Engine.DataStructures;

public static class TerrainDebug {
  public static float terrainX = 0;
  public static float terrainZ = 0;
  public static float gridSquareSize = 0;
  public static float gridX = 0;
  public static float gridZ = 0;
  public static float X = 0;
  public static float Z = 0;
  public static float R = 0;
}

public class TerrainMesh : Mesh {

  private float[,] _heights;
  private float _size;
  private float _interpolation;
  private int _terrainGridIndices;

  public TerrainMesh(
    List<Vertex> vertexArray,
    List<int> indices,
    Texture texture,
    float[,] heights,
    float size,
    float interpolation,
    int terrainGridIndices
  ) : base(vertexArray, indices, texture) {
    _heights = heights;
    _size = size;
    _interpolation = interpolation;
    _terrainGridIndices = terrainGridIndices;
  }

  public float GetHeightOfTerrain(float worldX, float worldZ, Entity transform) {
    float terrainX = worldX - transform.GetComponent<Transform>().Position.X;
    float terrainZ = worldZ - transform.GetComponent<Transform>().Position.Z;

    float gridSquareSize = _size / ((float)_terrainGridIndices - 1);
    int gridX = (int)Math.Floor(terrainX / gridSquareSize);
    int gridZ = (int)Math.Floor(terrainZ / gridSquareSize);
    //int gridX = (int)(player!.GetComponent<Transform>().Position.X / _size + 1);
    //int gridZ = (int)(player!.GetComponent<Transform>().Position.Z / _size + 1);
    //int gridX = transform.GetComponent<Transform>().Position.X / 
    if (gridX+1 >= _heights.Length || gridZ+1 >= _heights.Length || gridX < 0 || gridZ < 0) {
      return 0;
    }
    float x = (terrainX % gridSquareSize) / gridSquareSize;
    float z = (terrainZ % gridSquareSize) / gridSquareSize;
    float r;
    if (x <= (1 - z)) {
      if (gridX >= _terrainGridIndices) return 0;
      if (gridZ >= _terrainGridIndices) return 0;
      if (gridX+1 >= _terrainGridIndices) return 0;
      if (gridZ+1 >= _terrainGridIndices) return 0;
      r = DMath.Maths
          .BarryCentric(new Vector3(0, _heights[gridX, gridZ], 0), new Vector3(1,
              _heights[gridX + 1, gridZ], 0), new Vector3(0,
              _heights[gridX, gridZ + 1], 1), new Vector2(x, z));
    } else {
      if (gridX >= _terrainGridIndices) return 0;
      if (gridZ >= _terrainGridIndices) return 0;
      if (gridX + 1 >= _terrainGridIndices) return 0;
      if (gridZ + 1 >= _terrainGridIndices) return 0;
      r = DMath.Maths
          .BarryCentric(new Vector3(1, _heights[gridX + 1, gridZ], 0), new Vector3(1,
              _heights[gridX + 1, gridZ + 1], 1), new Vector3(0,
              _heights[gridX, gridZ + 1], 1), new Vector2(x, z));
    }
    TerrainDebug.terrainX = terrainX;
    TerrainDebug.terrainZ = terrainZ;
    TerrainDebug.gridSquareSize = gridSquareSize;
    TerrainDebug.gridX = gridX;
    TerrainDebug.gridZ = gridZ;
    TerrainDebug.X = x;
    TerrainDebug.Z = z;
    TerrainDebug.R = r;
    return r;
  }
}

