using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Mathematics;

using Voxelized.DataStructures;

namespace Voxelized.GLBinders;

unsafe public static class VAO {
  public static void UploadVerticesAndSpecifications() {
    Vertex[] myVertexArray = new Vertex[3];

    myVertexArray[0].Position = new Vector3(-0.5f, -0.5f, 0.0f);
    myVertexArray[0].Color = new Vector4(0.9f, 0.8f, 0.2f, 1.0f);

    myVertexArray[1].Position = new Vector3(0.0f, 0.5f, 0.0f);
    myVertexArray[0].Color = new Vector4(0.2f, 0.9f, 0.8f, 1.0f);

    myVertexArray[2].Position = new Vector3(0.5f, -0.5f, 0.0f);
    myVertexArray[0].Color = new Vector4(0.9f, 0.2f, 0.8f, 1.0f);

    GL.GenVertexArray();
  }

  /*
  public static void Challange1() {
    int i;
    private Vertex[] data =  {
      new Vertex{{0.9f,  0.1f, 0.12f, 1.0f},   {-0.5f, -0.5f, 0.0f}},
      new Vertex{{0.1f,  0.9f, 0.12f, 1.0f},   {-0.5f,  0.5f, 0.0f}},
      new Vertex{{0.12f, 0.9f, 0.1f,  1.0f},   { 0.5f,  0.5f, 0.0f}},

      new Vertex{{0.9f,  0.1f, 0.12f, 1.0f},   {-0.5f, -0.5f, 0.0f}},
      new Vertex{{0.12f, 0.9f, 0.1f,  1.0f},   { 0.5f,  0.5f, 0.0f}},
      new Vertex{{0.12f, 0.1f, 0.9f,  1.0f},   { 0.5f, -0.5f, 0.0f}}
    };
  }
  */
}