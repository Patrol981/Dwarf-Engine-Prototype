using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Textures;
using Voxelized.Shaders;
using Voxelized.Globals;

namespace Voxelized.DataStructures;

public class Cube {
  private readonly Shader _shader;
  private readonly float[] _vertices = {
   -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
  };

  private readonly uint[] _indices = {
    0, 1, 3,
    1, 2, 3
  };

  private Texture _texture;
  private string _textureSlot;
  private Vector3 _position;

  private int _vbo, _vao, _ebo;

  public Cube(Shader shader, string textureName, string textureSlot) {
    _shader = shader;

    _textureSlot = textureSlot;

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

    var vertexLocation = _shader.GetAttribLocation("aPosition");
    GL.EnableVertexAttribArray(vertexLocation);
    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

    var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
    GL.EnableVertexAttribArray(texCoordLocation);
    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

    _texture = Texture.LoadFromFile($"Resources/{textureName}.png");
    _texture.Use(TextureUnit.Texture0);
  }

  public void Update() {
    _shader.Use();
    _shader.SetInt(_textureSlot, 0);
    _texture.Use(TextureUnit.Texture1);

    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
  }

  public void Translate() {
    this.SetPosition(new Vector3(_position.X + 0.01f, _position.Y, _position.Z));
  }

  public Vector3 GetPosition() {
    return _position;
  }

  public void SetPosition(Vector3 position) {
    _shader.Use();
    _position = position;
    var model = Matrix4.CreateTranslation(_position.X, _position.Y, _position.Z);
    _shader!.SetMatrix4("model", model);
  }
}