using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Cameras;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Dwarf.Engine.Shaders;

namespace Dwarf.Engine.Physics;
public class BoundingBox : Component {
  private Vector3 _size = Vector3.Zero;
  private Vector3 _center = Vector3.Zero;
  private Matrix4 _transform = Matrix4.Identity;

  private Matrix4 _model = Matrix4.Identity;
  private Matrix4 _worldModel = Matrix4.Identity;

  private float _minX, _minY, _minZ, _maxX, _maxY, _maxZ;

  private Shader? _shader;

  int _vbo;
  int _ibo;

  float[] _vertices = {
      -0.5f, -0.5f, -0.5f, 1.0f,
       0.5f, -0.5f, -0.5f, 1.0f,
       0.5f, 0.5f, -0.5f, 1.0f,
      -0.5f, 0.5f, -0.5f, 1.0f,
      -0.5f, -0.5f, 0.5f, 1.0f,
       0.5f, -0.5f, 0.5f, 1.0f,
       0.5f, 0.5f, 0.5f, 1.0f,
      -0.5f, 0.5f, 0.5f, 1.0f,
    };

  int[] _elements = {
      0, 1, 2, 3,
      4, 5, 6, 7,
      0, 4, 1, 5, 2, 6, 3, 7
    };

  public static bool Intersect(BoundingBox b1, BoundingBox b2) {
    var b1Pos = b1.WorldModel.ExtractTranslation();
    var b2Pos = b2.WorldModel.ExtractTranslation();

    bool x = MathF.Abs(b1Pos.X - b2Pos.X) * 2 < (b1.Size.X + b2.Size.X);
    bool y = MathF.Abs(b1Pos.Y - b2Pos.Y) * 2 < (b1.Size.Y + b2.Size.Y);
    bool z = MathF.Abs(b1Pos.Z - b2Pos.Z) * 2 < (b1.Size.Z + b2.Size.Z);

    return x && y && z;
  }

  public void Setup(MasterMesh masterMesh) {
    // if (masterMesh.VertexArray.Count == 0) return;

    _shader = new Shader("./Shaders/boundingShader.vert", "./Shaders/boundingShader.frag");

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sizeof(float) * _vertices.Length,
      _vertices,
      BufferUsageHint.StaticDraw
    );
    //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    _ibo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _ibo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sizeof(int) * _elements.Length,
      _elements,
      BufferUsageHint.StaticDraw
    );

    _minX = _maxX = masterMesh.Meshes[0].Vertices[0].X;
    _minY = _maxY = masterMesh.Meshes[0].Vertices[0].Y;
    _minZ = _maxZ = masterMesh.Meshes[0].Vertices[0].Z;

    for(int i=0; i < masterMesh.Meshes[0].Vertices.Count; i++) {
      if (masterMesh.Meshes[0].Vertices[i].X < _minX) _minX = masterMesh.Meshes[0].Vertices[i].X;
      if (masterMesh.Meshes[0].Vertices[i].X > _maxX) _maxX = masterMesh.Meshes[0].Vertices[i].X;
      if (masterMesh.Meshes[0].Vertices[i].Y < _minY) _minY = masterMesh.Meshes[0].Vertices[i].Y;
      if (masterMesh.Meshes[0].Vertices[i].Y > _maxY) _maxY = masterMesh.Meshes[0].Vertices[i].Y;
      if (masterMesh.Meshes[0].Vertices[i].Z < _minZ) _minZ = masterMesh.Meshes[0].Vertices[i].Z;
      if (masterMesh.Meshes[0].Vertices[i].Z > _maxZ) _maxZ = masterMesh.Meshes[0].Vertices[i].Z;
    }

    _size = new Vector3(_maxX - _minX, _maxY - _minY, _maxZ - _minZ);
    _center = new Vector3((_minX + _maxX) / 2, (_minY + _maxY) / 2, (_minZ + _maxZ) / 2);
    _transform = Matrix4.CreateTranslation(_center) * Matrix4.CreateScale(_size);

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.EnableVertexAttribArray(7);
    GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, 0, 0);

    GL.BindBuffer(BufferTarget.ArrayBuffer, _ibo);
    GL.EnableVertexAttribArray(8);
  }

  public void Draw(Camera camera) {
    if (_shader == null) return;

    _model = camera.GetViewMatrix() * _transform;
    var modelPos = Owner!.GetComponent<Transform>().Position;
    _worldModel = Matrix4.CreateTranslation(modelPos);

    _shader.SetMatrix4("uModel", _worldModel);
    // GL.UniformMatrix4()

    // GL.DrawElements(PrimitiveType.Triangles, _elements.Length, DrawElementsType.UnsignedInt, 0);

    //GL.DrawElements(PrimitiveType.LineLoop, 0, DrawElementsType.UnsignedInt, 0);
    //GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedInt, sizeof(uint) * 4);
    //GL.DrawElements(PrimitiveType.Lines, 8, DrawElementsType.UnsignedShort, sizeof(int) * 8);

    GL.BindVertexArray(0);
  }

  public Vector3 Size {
    get { return _size; }
  }

  public Vector3 Center {
    get { return _center; }
  }

  public Matrix4 Tranform {
    get { return _transform; }
  }

  public Matrix4 Model {
    get { return _model; }
  }

  public Matrix4 WorldModel {
    get { return _worldModel; }
  }

  public float MinX {
    get { return _minX; }
  }

  public float MinY {
    get { return _minY; }
  }

  public float MinZ {
    get { return _minZ; }
  }

  public float MaxX {
    get { return _maxX; }
  }

  public float MaxY {
    get { return _maxY; }
  }

  public float MaxZ {
    get { return _maxZ; }
  }
}
