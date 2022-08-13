using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Core.Native;

namespace Dwarf.Engine.DataStructures;

// TODO:
// separate position shader and texture
class MeshRenderer : Component {

  private Shader? _shader;
  private Matrix4 _model;
  // private int _vbo, _vao, _ebo;
  private List<int> _vbo = new(), _vao = new(), _ebo = new();
  private List<Textures.Texture> _textures = new();

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

    for (int i = 0; i < masterMesh.Meshes.Count; i++) {
      _vao.Add(i);
      _vbo.Add(i);
      _ebo.Add(i);

      _vao[i] = GL.GenVertexArray();
      GL.BindVertexArray(_vao[i]);

      _vbo[i] = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo[i]);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        masterMesh.Meshes[i].VertexArray.Count * Unsafe.SizeOf<Vertex>(),
        masterMesh.Meshes[i].VertexArray.ToArray(),
        BufferUsageHint.StaticDraw
      );

      if (masterMesh.Meshes[i].Indices != null && masterMesh.Meshes[i].Indices.Count > 0) {
        _ebo[i] = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo[i]);

        GL.BufferData(
          BufferTarget.ElementArrayBuffer,
          masterMesh.Meshes[i].Indices.Count * sizeof(uint),
          masterMesh.Meshes[i].Indices.ToArray(),
          BufferUsageHint.StaticDraw
        );
      }

      _textures.Add(Textures.Texture.LoadFromFile($"Resources/Yuna/{i}.png"));
      //_textures[i].Use(Textures.ETextureUnit.Texture0);
      _textures[i].Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));

      // var test = (TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture{i}");

      // position
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);

      // texture
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("TextureCoords"));

      // normal
      GL.EnableVertexAttribArray(2);
      GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Normal"));

      // tangent
      GL.EnableVertexAttribArray(3);
      GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Tangent"));

      // bitangent
      GL.EnableVertexAttribArray(4);
      GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Bitangent"));

      _shader.SetInt("texture0", 0);

      Console.WriteLine($"Mesh: {masterMesh.Meshes[i].Name}");
      Console.WriteLine($"index: {i}");
    }
    return this;
  }

  public void Render(Camera camera) {
    if (Owner!.GetComponent<MasterMesh>() == null) {
      return;
    }

    var masterMesh = Owner!.GetComponent<MasterMesh>();

    _shader!.Use();
    _shader.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);

    Material material = Owner!.GetComponent<Material>();
    _shader.SetVector3("uDiffuse", material?.GetColor() ?? new Vector3(1, 0.5f, 0.3f));

    Matrix4 projection = camera.GetProjectionMatrix();
    Matrix4 view = camera.GetViewMatrix();
    _shader.SetMatrix4("uProjection", projection);
    _shader.SetMatrix4("uView", view);

    Matrix4 worldModel = _model * Matrix4.CreateTranslation(Owner.GetComponent<Transform>().Position);
    //var qY = Quaternion.FromAxisAngle(Owner.GetComponent<Transform>().Position, Owner.GetComponent<Transform>().Rotation.Y);


    // var t = Matrix4.CreateTranslation(camera.Owner!.GetComponent<Transform>().Position);
    //var rotY = Matrix4.Identity * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y));
    //worldModel *= Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.X));
    //worldModel *= Matrix4.Transpose(Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y)));
    //worldModel *= Matrix4.Identity * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z));
    //worldModel *= (rotY);
    _shader.SetMatrix4("uModel", worldModel);

    for(int i = 0; i < masterMesh.Meshes.Count; i++) {

      int diffuse = 1, specular = 1, normal = 1, height = 1;
      if(masterMesh.Meshes[i].TextureArray != null) {
        for (int j = 0; j < masterMesh.Meshes[i].TextureArray.Count; j++) {
          string number = "";
          string name = masterMesh.Meshes[i].TextureArray[j].Type;

          if (name == Assimp.TextureType.Diffuse.ToString()) {
            diffuse++;
            number = diffuse.ToString();
          } else if (name == Assimp.TextureType.Specular.ToString()) {
            specular++;
            number = specular.ToString();
          } else if (name == Assimp.TextureType.Normals.ToString()) {
            normal++;
            number = normal.ToString();
          } else if (name == Assimp.TextureType.Height.ToString()) {
            height++;
            number = height.ToString();
          }

          GL.Uniform1(GL.GetUniformLocation(_shader.GetHandle(), $"{name}{number}"), j);
          GL.BindTexture(TextureTarget.Texture2D, masterMesh.Meshes[i].TextureArray[j].Id);
        }
      }

      _textures[i].Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));
      _shader.Use();

      GL.BindVertexArray(_vao[i]);
      if (masterMesh.Meshes[i].Indices != null && masterMesh.Meshes[i].Indices.Count > 0) {
        GL.DrawElements(PrimitiveType.Triangles, masterMesh.Meshes[i].Indices.Count, DrawElementsType.UnsignedInt, 0);
      } else {
        GL.DrawArrays(PrimitiveType.Triangles, 0, masterMesh.Meshes[i].VertexArray.Count);
      }

      GL.BindVertexArray(0);
      GL.ActiveTexture(TextureUnit.Texture0);
    }
    


  }
}