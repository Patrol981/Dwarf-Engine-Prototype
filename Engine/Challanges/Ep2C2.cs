using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Mathematics;

using Voxelized.DataStructures;
using Voxelized.Shaders;

namespace Voxelized.Challanges;

public class Ep2C2 {
  private readonly float[] _verts = new[] {
     0.5f,  0.5f, 0.0f,  // top right
     0.5f, -0.5f, 0.0f,  // bottom right
    -0.5f, -0.5f, 0.0f,  // bottom left
    -0.5f,  0.5f, 0.0f   // top left
  };

  private readonly uint[] _indices = {
    // note that we start from 0!
    0, 1, 3,   // first triangle
    1, 2, 3    // second triangle
  };

  private int _vbo, _vao, _ebo;
  private readonly Shader _shader;

  public unsafe Ep2C2(Shader shader) {
    _shader = shader;

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, _verts.Length * sizeof(float), _verts, BufferUsageHint.StaticDraw);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    _shader.Use();
  }

  public void Update() {
    _shader.Use();

    GL.BindVertexArray(_vao);
    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
  }
}