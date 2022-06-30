using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Voxelized.Shaders;
using Voxelized.Cameras;
using Voxelized.ECS;

namespace Voxelized.DataStructures;

class MeshRenderer : Component {

  private Shader? _shader;
  private Matrix4 _model;
  private int _vbo, _vao, _ebo;

  public MeshRenderer() {
  }
  public MeshRenderer Init() {
    Mesh mesh = Owner!.GetComponent<Mesh>();
    if (mesh == null) {
      return this;
    }

    mesh.GetVertexArray();

    _model = Matrix4.Identity;
    _shader = new Shader("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      mesh.BufferDataCount,
      mesh.GetVertexArray().ToArray(),
      BufferUsageHint.StaticDraw
    );

    if (mesh.Indices != null && mesh.Indices.Count > 0) {
      _ebo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

      GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        mesh.Indices.Count * sizeof(int),
        mesh.Indices.ToArray(),
        BufferUsageHint.StaticDraw
      );
    }

    

    // position
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, mesh.PackageStrideSize, 0);

    // normal
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, mesh.PackageStrideSize, 3 * sizeof(float));

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

    if(mesh.MeshRenderType == Engine.Enums.MeshRenderType.WavefrontObjFile) {
      //GL.EnableVertexAttribArray(2);
      //GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, mesh.PackageStrideSize, 6 * sizeof(float));
    }

    // texture
    //var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
    //GL.EnableVertexAttribArray(texCoordLocation);

    //mesh.Texture = Textures.Texture.LoadFromFile("Resources/container.png");
    //mesh.Texture.Use(TextureUnit.Texture0);

    //_shader.SetInt("texture0", 0);

    // GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

    //if (mesh.ColorPerIndex.Count > 0) {

    //}

    GL.BindVertexArray(0);

    return this;
  }

  public void Render(Camera camera) {
    if (Owner!.GetComponent<Mesh>() == null) {
      return;
    }

    Mesh mesh = Owner!.GetComponent<Mesh>();
    // mesh.Texture.Use(TextureUnit.Texture0);
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
    //Console.WriteLine(Owner.GetComponent<Mesh>().DrawCount);
    // GL.DrawArrays(PrimitiveType.Triangles, 0, Owner.GetComponent<Mesh>().DrawCount);
    if(mesh.MeshRenderType == Engine.Enums.MeshRenderType.Terrain) {
      GL.DrawElements(PrimitiveType.Triangles, mesh.VertexArray.Count, DrawElementsType.UnsignedInt, 0);
    } else {
      GL.DrawArrays(PrimitiveType.Triangles, 0, Owner.GetComponent<Mesh>().DrawCount);
    }
    
  }

  public Shader GetShader() {
    if (_shader == null) {
      return null!;
    }
    return _shader;
  }
}