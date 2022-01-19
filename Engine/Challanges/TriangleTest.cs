using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Shaders;

namespace Voxelized.Challanges;

public class TriangleTest {
  private int _vbo, _vao;
  private readonly Shader _shader;
  private readonly float[] _vertices = {
    -0.5f, -0.5f, 0.0f, // Bottom-left vertex
    0.5f, -0.5f, 0.0f, // Bottom-right vertex
    0.0f,  0.5f, 0.0f  // Top vertex
  };

  public TriangleTest(Shader shader) {
    _shader = shader;

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    _shader.Use();
  }

  public void Update() {
    _shader.Use();

    GL.BindVertexArray(_vao);
    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
  }
}