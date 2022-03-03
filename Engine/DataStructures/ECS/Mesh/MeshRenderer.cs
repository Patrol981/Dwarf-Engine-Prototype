using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Voxelized.Shaders;
using Voxelized.Cameras;

namespace Voxelized.ECS;

class MeshRenderer : Component {
  private Shader? _shader;
  private Matrix4 _model;
  private int _vbo, _vao;

  public MeshRenderer() {
  }

  public MeshRenderer Init() {
    Mesh mesh = Owner!.GetComponent<Mesh>();
    if(mesh == null) {
      return this;
    }

    _model = Matrix4.Identity;
    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sizeof(float) * mesh.GetVertexCount() * 6, mesh.GetVertecies(),
      BufferUsageHint.StaticDraw
    );

    // position
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

    // normal
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    return this;
  }

  public void Render(Camera camera) {
    if(Owner!.GetComponent<Mesh>() == null) {
      return;
    }
    _shader!.Use();
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
    GL.DrawArrays(PrimitiveType.Triangles, 0, Owner.GetComponent<Mesh>().GetVertexCount());
  }

  public Shader GetShader() {
    if(_shader == null) {
      return null!;
    }
    return _shader;
  }
}