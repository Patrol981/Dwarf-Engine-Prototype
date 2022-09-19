﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;

namespace Dwarf.Engine.Generators;

[Obsolete]
internal class MeshGenerator : Component {
  private float _size;
  private int _vertexCount;
  private Shader _shader;
  private Matrix4 _model;

  private int _vbo;
  private int _vao;
  private int _ebo;

  protected float X;
  protected float Y;
  protected Mesh Mesh = new();

  List<float> _vertices = new();
  List<float> _normals = new();
  List<int> _indices = new();
  List<Vertex> _vertexArray = new();
  int _count;

  public MeshGenerator() {
  }

  public MasterMesh SetupPlane(int gridX, int gridY, float size = 100, int vertexCount = 32) {
    _size = size;
    _vertexCount = vertexCount;

    X = gridX * _size;
    Y = gridY * _size;

    List<Vector3> data = new();

    int count = _vertexCount * _vertexCount;
    float[] vertices = new float[count * 3];
    float[] normals = new float[count * 3];
    float[] textureCoords = new float[count * 2];
    int[] indices = new int[6 * (_vertexCount - 1) * (_vertexCount - 1)];
    int vertexPointer = 0;
    for (int i = 0; i < _vertexCount; i++) {
      for (int j = 0; j < _vertexCount; j++) {
        vertices[vertexPointer * 3] = (float)j / ((float)_vertexCount - 1) * _size;
        vertices[vertexPointer * 3 + 1] = 0;
        vertices[vertexPointer * 3 + 2] = (float)i / ((float)_vertexCount - 1) * _size;
        normals[vertexPointer * 3] = 0;
        normals[vertexPointer * 3 + 1] = 1;
        normals[vertexPointer * 3 + 2] = 0;
        textureCoords[vertexPointer * 2] = (float)j / ((float)_vertexCount - 1);
        textureCoords[vertexPointer * 2 + 1] = (float)i / ((float)_vertexCount - 1);
        vertexPointer++;
      }
    }

    int pointer = 0;
    for (int gz = 0; gz < _vertexCount - 1; gz++) {
      for (int gx = 0; gx < _vertexCount - 1; gx++) {
        int topLeft = (gz * _vertexCount) + gx;
        int topRight = topLeft + 1;
        int bottomLeft = ((gz + 1) * _vertexCount) + gx;
        int bottomRight = bottomLeft + 1;
        indices[pointer++] = topLeft;
        indices[pointer++] = bottomLeft;
        indices[pointer++] = topRight;
        indices[pointer++] = topRight;
        indices[pointer++] = bottomLeft;
        indices[pointer++] = bottomRight;
      }
    }

    _indices = indices.ToList();
    _normals = normals.ToList();
    _vertices = vertices.ToList();
    _count = count;

    MasterMesh masterMesh = Owner!.GetComponent<MasterMesh>();
    if (masterMesh == null) {
      masterMesh = new MasterMesh();
      Owner!.AddComponent(masterMesh);
    }

    Mesh mesh = new();

    _model = Matrix4.Identity;
    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    List<Vector3> verts = new();
    List<Vector3> norms = new();
    List<Vector3> colors = new();
    for (int i = 0; i < vertices.Length; i += 3) {
      verts.Add(new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
    }
    for (int i = 0; i < normals.Length; i += 3) {
      verts.Add(new Vector3(normals[i], normals[i + 1], normals[i + 2]));
    }
    for (int i = 0; i < count; i++) {
      colors.Add(new Vector3(1, 1, 1));
    }

    for (int i = 0; i < _vertices.Count; i += 3) {
      Vertex v = new();
      v.Position = new Vector3(vertices[i], vertices[i + 1], vertices[i+2]);
      v.Normal = new Vector3(normals[i], normals[i + 1], normals[i + 2]);
      _vertexArray.Add(v);
    }

    mesh.Vertices = verts.ToList();
    mesh.Normals = norms;
    mesh.VertexArray = _vertexArray;
    mesh.Indices = _indices;

    mesh.Texture = Textures.Texture.LoadFromFile("Resources/grass.png");

    masterMesh.Meshes.Add(mesh);
    masterMesh.MeshRenderType = Enums.MeshRenderType.Terrain;
    masterMesh.RecalculateMeshes();
    return masterMesh;
  }
}
