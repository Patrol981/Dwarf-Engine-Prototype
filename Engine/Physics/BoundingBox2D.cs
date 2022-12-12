using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Physics;
public class BoundingBox2D : Component {
  public readonly float[] Vertices = {
    0.5f,  0.5f, 0.0f,  1.0f, 1.0f, // top right
    0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f,  0.0f, 1.0f  // top left 
  };

  public readonly ushort[] Indices = {
    0, 1, 3, // first triangle
    1, 2, 3  // second triangle
  };

  public static string VertexCode = @"
      #version 330 core
      layout(location = 0) in vec3 aPosition;
      uniform mat4 uModel;
      uniform mat4 uView;
      uniform mat4 uProjection;
  
		  void main(void)
		  {
			  gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
		  }
  ";

  public static string FragmentCode = @"
      #version 330 core
      out vec4 vFragColor;
      uniform vec3 uMaterial;

		  void main()
		  {
        vFragColor = vec4(uMaterial,1);
		  }
  ";

  private Shader? _shader;
  private int _vao, _ebo, _vbo;
  private float _minX, _minY, _minZ, _maxX, _maxY, _maxZ;
  private Vector3 _size = Vector3.Zero;
  private Vector3 _center = Vector3.Zero;
  private Matrix4 _transform = Matrix4.Identity;
  private Matrix4 _model = Matrix4.Identity;
  private Matrix4 _worldModel = Matrix4.Identity;

  public void Setup(Sprite sprite) {
    _shader = new Shader(VertexCode, FragmentCode, false);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sizeof(float) * Vertices.Length,
      Vertices,
      BufferUsageHint.StaticDraw
    );

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(
      BufferTarget.ElementArrayBuffer,
      sizeof(ushort) * Indices.Length,
      Indices,
      BufferUsageHint.StaticDraw
    );

    CalculateBox(sprite);
  }

  public void CalculateBox(Sprite sprite) {
    _minX = _maxX = sprite.Vertices[0];
    _minY = _maxY = sprite.Vertices[1];
    _minZ = _maxZ = sprite.Vertices[2];

    for (int n = 0; n < Vertices.Length; n += 3) {
      if (sprite.Vertices[n] < _minX) _minX = sprite.Vertices[n];
      if (sprite.Vertices[n] > _maxX) _maxX = sprite.Vertices[n];

      if (sprite.Vertices[n + 1] < _minY) _minY = sprite.Vertices[n + 1];
      if (sprite.Vertices[n + 1] > _maxY) _maxY = sprite.Vertices[n + 1];

      //if (sprite.Vertices[n + 2] < _minZ) _minZ = sprite.Vertices[n + 2];
      //if (sprite.Vertices[n + 2] < _maxZ) _maxZ = sprite.Vertices[n + 2];
    }

    _size = new Vector3(_maxX - _minX, _maxY - _minY, 0);
    _center = new Vector3((_minX + _maxX) / 2, (_minY + _maxY) / 2, 0);

    _transform = Matrix4.CreateTranslation(_center) * Matrix4.CreateScale(_size);
  }

  public void Draw(Camera camera) {
    if (_shader == null) return;

    _shader.Use();
    _shader.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);

    _shader.SetMatrix4("uProjection", camera.GetProjectionMatrix());
    _shader.SetMatrix4("uView", camera.GetViewMatrix());

    var modelPos = Owner!.GetComponent<Transform>().Position;
    var scale = Owner.GetComponent<Transform>().Scale;

    _model = _transform;
    _worldModel = _model * Matrix4.CreateTranslation(modelPos) * Matrix4.CreateScale(scale);
    _shader.SetMatrix4("uModel", _worldModel);
    _shader.SetVector3("uMaterial", new Vector3(127, 255, 0));
    GL.BindVertexArray(_vao);

    GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedShort, 0);
    GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedShort, sizeof(ushort) * 4);
    GL.DrawElements(PrimitiveType.Lines, 8, DrawElementsType.UnsignedShort, sizeof(ushort) * 8);

    GL.BindVertexArray(0);
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
}
