using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Voxelized.Cameras;
using Voxelized.DataStructures;
using Voxelized.ECS;
using Voxelized.Shaders;

namespace Voxelized.Engine.Generators;
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
  protected Texture Texture = new();

  List<float> _vertices = new();
  List<float> _normals = new();
  List<int> _indices = new();
  List<Vector3> _vertexArray = new();
  int _count;

  public MeshGenerator() {
  }

  public Mesh SetupPlane(int gridX, int gridY, float size = 100, int vertexCount = 32) {
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


    // Mesh mesh = new Mesh(normals, null, vertices, indices, null, null, null);
    Mesh mesh = Owner!.GetComponent<Mesh>();
    if (mesh == null) {
      mesh = new Mesh();
      Owner!.AddComponent(mesh);
    }
    _model = Matrix4.Identity;
    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    List<Vector3> array = new();
    List<Vector3> verts = new();
    List<Vector3> norms = new();
    List<Vector3> colors = new();
    for (int i = 0; i < vertices.Length; i += 3) {
      verts.Add(new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
      // verts.AddRange(new float[] { vertices[i], vertices[i + 1], vertices[i + 2] });
    }
    for (int i = 0; i < normals.Length; i += 3) {
      verts.Add(new Vector3(normals[i], normals[i + 1], normals[i + 2]));
      // norms.AddRange(new float[] { normals[i], normals[i + 1], normals[i + 2] });
    }
    for (int i = 0; i < count; i++) {
      colors.Add(new Vector3(1, 1, 1));
    }

    for (int i = 0; i < _vertices.Count; i += 3) {
     _vertexArray.Add(new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
     _vertexArray.Add(new Vector3(normals[i], normals[i + 1], normals[i + 2]));
      /*
      _vertexArray.AddRange(
          new float[] {
            vertices[i],
            vertices[i + 1],
            vertices[i + 2],
            normals[i],
            normals[i + 1],
            normals[i + 2]
          }
      );
      */
      
    }

    for (int x = 0; x < count; x++) {
      //vec3 position,vec3 normals, vec3 color
      //array.Add(verts[x]);
      //array.Add(norms[x]);
      // array.Add(colors[x]);
    }
    //for (int x = 0; x < indices.Length; x++) {
    //Console.WriteLine(vertices[x]);
    //}

    mesh.Vertices = verts.ToList();
    mesh.Normals = norms;
    mesh.VertexArray = _vertexArray;
    mesh.Indices = _indices;
    mesh.MeshRenderType = Enums.MeshRenderType.Terrain;

    mesh.CalculateVertexArray(mesh.MeshRenderType);
    // mesh.VertexArray = array;
    /*
    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

    GL.BufferData(
      BufferTarget.ArrayBuffer,
      mesh.VertexArray.Count * sizeof(float) * 3,
      mesh.VertexArray.ToArray(),
      BufferUsageHint.StaticDraw
    );

    GL.BufferData(
      BufferTarget.ElementArrayBuffer,
      mesh.Indices.Count * sizeof(int),
      mesh.Indices.ToArray(),
      BufferUsageHint.StaticDraw
    );


    // pos
    GL.EnableVertexAttribArray(0);
    // GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, mesh.PackageStrideSize, 0);

    int t1 = mesh.PackageStrideSize;
    int t2 = 6 * sizeof(float);
    // normal
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));


    //GL.EnableVertexAttribArray(2);
    //GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    GL.BindVertexArray(0);
    */
    return mesh;

  }

  public void Render(Camera camera) {
    if (Owner!.GetComponent<Mesh>() == null) {
      return;
    }

    var mesh = Owner.GetComponent<Mesh>();

    _shader.Use();
    _shader.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);

    Material material = Owner!.GetComponent<Material>();
    _shader.SetVector3("uDiffuse", material?.GetColor() ?? new Vector3(1, 0.5f, 0.3f));

    Matrix4 projection = camera.GetProjectionMatrix();
    Matrix4 view = camera.GetViewMatrix();
    _shader.SetMatrix4("uProjection", projection);
    _shader.SetMatrix4("uView", view);

    Matrix4 worldModel = _model * Matrix4.CreateTranslation(Owner.GetComponent<Transform>().Position);
    _shader.SetMatrix4("uModel", worldModel);

    GL.BindVertexArray(_vao);
    // GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.VertexArray.Count);
    GL.DrawElements(PrimitiveType.Triangles, mesh.VertexArray.Count, DrawElementsType.UnsignedInt, 0);
  }
}
