using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Voxelized.Engine.Cameras;
using Voxelized.Engine.ECS;
using Voxelized.Engine.Shaders;

namespace Voxelized.Engine.DataStructures;

class MeshRenderer : Component {

  private Shader? _shader;
  private Matrix4 _model;
  private int _vbo, _vao, _ebo;

  public MeshRenderer() {
  }
  public MeshRenderer Init(string vs, string fs) {
    MasterMesh masterMesh = Owner!.GetComponent<MasterMesh>();
    if (masterMesh == null) {
      return null!;
    }

    // masterMesh.Meshes[i].GetVertexArray();

    _model = Matrix4.Identity;
    _shader = new Shader(vs, fs);
    // masterMesh.Meshes[i].SetupShader(vs, fs);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      masterMesh.BufferDataCount,
      masterMesh.VertexArray.ToArray(),
      BufferUsageHint.StaticDraw
    );

    if (masterMesh.Indices != null && masterMesh.Indices.Count > 0) {
      _ebo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

      GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        masterMesh.Indices.Count * sizeof(int),
        masterMesh.Indices.ToArray(),
        BufferUsageHint.StaticDraw
      );
    }



    // position
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, masterMesh.PackageStrideSize, 0);

    // normal
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, masterMesh.PackageStrideSize, 3 * sizeof(float));

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

    if (masterMesh.Meshes[0].Texture != null) {
      // GL.BindBuffer(BufferTarget.ArrayBuffer, )
      // GL.BufferData(BufferTarget.ArrayBuffer, mesh.)
      // 
      //GL.EnableVertexAttribArray(2);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

      var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(2);
      GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, masterMesh.PackageStrideSize, 6 * sizeof(float));

      // mesh.Texture = Textures.Texture.LoadFromFile("Resources/container.png");
      masterMesh.Meshes[0].Texture.Use(TextureUnit.Texture0);

      _shader.SetInt("texture0", 0);

      GL.BindVertexArray(0);
    }

    return this;
  }

  public void Render(Camera camera) {
    if (Owner!.GetComponent<MasterMesh>() == null) {
      return;
    }

    var masterMesh = Owner!.GetComponent<MasterMesh>();
    if (masterMesh.MeshRenderType == Engine.Enums.MeshRenderType.WavefrontObjFile) {
      masterMesh.Meshes[0].Texture.Use(TextureUnit.Texture0);
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
    var qY = Quaternion.FromAxisAngle(Owner.GetComponent<Transform>().Position, Owner.GetComponent<Transform>().Rotation.Y);


    // var t = Matrix4.CreateTranslation(camera.Owner!.GetComponent<Transform>().Position);
    var rotY = Matrix4.Identity * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y));
    //worldModel *= Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.X));
    //worldModel *= Matrix4.Transpose(Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y)));
    //worldModel *= Matrix4.Identity * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z));
    worldModel *= (rotY);
    _shader.SetMatrix4("uModel", worldModel);

    GL.BindVertexArray(_vao);
    //Console.WriteLine(Owner.GetComponent<Mesh>().DrawCount);
    // GL.DrawArrays(PrimitiveType.Triangles, 0, Owner.GetComponent<Mesh>().DrawCount);
    if (masterMesh.MeshRenderType == Engine.Enums.MeshRenderType.Terrain) {
      GL.DrawElements(PrimitiveType.Triangles, masterMesh.VertexArray.Count, DrawElementsType.UnsignedInt, 0);
    } else {
      GL.DrawArrays(PrimitiveType.Triangles, 0, masterMesh.DrawCount);
    }


  }
}