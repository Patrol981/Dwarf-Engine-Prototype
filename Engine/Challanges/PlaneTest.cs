using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Textures;
using Voxelized.Shaders;

namespace Voxelized.Challanges;

public class PlaneTest {
  private readonly Shader _shader;
  private readonly Texture _texture;

  private readonly float[] _vertices = {
    // Position         Texture coordinates
    0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
    0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
  };

  private readonly uint[] _indices = {
    0, 1, 3,
    1, 2, 3
  };

  private int _vbo, _vao, _ebo;

  public PlaneTest(Shader shader) {
    _shader = shader;

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

    _shader.Use();

    var vertexLocation = _shader.GetAttribLocation("aPosition");
    GL.EnableVertexAttribArray(vertexLocation);
    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

    var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
    GL.EnableVertexAttribArray(texCoordLocation);
    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

    _texture = Texture.LoadFromFile("Resources/container.png");
    _texture.Use(TextureUnit.Texture0);

    _shader.SetInt("texture0", 0);
  }

  public void Update() {
    _texture.Use(TextureUnit.Texture0);
    _shader.Use();

    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
  }
}