using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Core.Native;
using Dwarf.Engine.Enums;
using Dwarf.Engine.Physics;

namespace Dwarf.Engine.DataStructures;

public class MeshRenderer : Component {

  private Shader? _shader;
  private Matrix4 _model;
  private Matrix4 _worldModel;
  // private int _vbo, _vao, _ebo;
  private List<int> _vbo = new(), _vao = new(), _ebo = new();

  public MeshRenderer() {
  }

  public Matrix4 Model {
    get { return _model; }
  }

  public Shader Shader {
    get { return _shader ?? null!; }
  }

  public Matrix4 WorldModel {
    get { return _worldModel; }
  }

  public void Update() {
    MasterMesh masterMesh = Owner!.GetComponent<MasterMesh>();
    if (masterMesh == null) {
      return;
    }

    for (int i = 0; i < masterMesh.Meshes.Count; i++) {
      SetupGLCalls(masterMesh, i);
    }
  }

  public MeshRenderer Init(string vs, string fs, bool isFile = true) {
    MasterMesh masterMesh = Owner!.GetComponent<MasterMesh>();
    if (masterMesh == null) {
      return null!;
    }

    _model = Matrix4.Identity;
    _worldModel = Matrix4.Identity;
    _shader = new Shader(vs, fs, isFile);

    _vao = new List<int>();
    _vbo = new List<int>();
    _ebo = new List<int>();

    for (int i = 0; i < masterMesh.Meshes.Count; i++) {
      _vao.Add(i);
      _vbo.Add(i);
      _ebo.Add(i);

      SetupGLCalls(masterMesh, i);

      _shader.SetInt("texture0", 0);
    }
    return this;
  }

  private void SetupGLCalls(MasterMesh masterMesh, int i) {
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

    if (masterMesh.Meshes[i].Texture != null) {
      masterMesh.Meshes[i].Texture.Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));
    }

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

    // joints
    GL.EnableVertexAttribArray(5);
    GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Int, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("JointIds"));

    // weights
    GL.EnableVertexAttribArray(6);
    GL.VertexAttribPointer(6, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Weights"));
  }

  public void Render(Camera camera) {
    if (Owner!.GetComponent<MasterMesh>() == null) {
      return;
    }

    var masterMesh = Owner!.GetComponent<MasterMesh>();

    _shader!.Use();
    _shader.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);

    Material material = Owner!.GetComponent<Material>();
    _shader.SetVector3("uDiffuse", material?.GetColor() ?? new Vector3(1, 1f, 1f));

    Matrix4 projection = camera.GetProjectionMatrix();
    Matrix4 view = camera.GetViewMatrix();
    _shader.SetMatrix4("uProjection", projection);
    _shader.SetMatrix4("uView", view);

    var modelPos = Owner.GetComponent<Transform>().Position;
    var angleX = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.X);
    var angleY = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y);
    var angleZ = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z);

    _model = Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY) * Matrix4.CreateRotationZ(angleZ);

    _worldModel = _model * Matrix4.CreateTranslation(modelPos);

    _shader.SetMatrix4("uModel", _worldModel);

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

      if(masterMesh.Meshes[i].Texture != null) {
        masterMesh.Meshes[i].Texture.Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));
      }
      _shader.Use();

      GL.BindVertexArray(_vao[i]);
      switch (masterMesh.MeshRenderType) {
        case MeshRenderType.Mesh:
          HandleMesh(masterMesh, i);
          break;

        case MeshRenderType.Line:
          HandleLine(masterMesh, i);
          break;

        default:
          break;
      }
      GL.BindVertexArray(0);
      GL.ActiveTexture(TextureUnit.Texture0);

      // Render Bounding Boxes
      HandleBoundingBox(camera);
    }
    
  }

  private void HandleMesh(MasterMesh masterMesh, int index) {
    if (!Owner!.GetComponent<MasterMesh>().Render) return;

    if (masterMesh.Meshes[index].Indices != null && masterMesh.Meshes[index].Indices.Count > 0) {
      GL.DrawElements(PrimitiveType.Triangles, masterMesh.Meshes[index].Indices.Count, DrawElementsType.UnsignedInt, 0);
    } else {
      GL.DrawArrays(PrimitiveType.Triangles, 0, masterMesh.Meshes[index].VertexArray.Count);
    }

    /*
    if (masterMesh.Meshes[index].Texture != null) {
      GL.BindTexture(TextureTarget.Texture2D, 0);
      GL.ActiveTexture(TextureUnit.Texture0);
      masterMesh.Meshes[index].Texture.Use(TextureUnit.Texture0);
    }

    GL.BindVertexArray(0);
    */
  }

  private void HandleLine(MasterMesh masterMesh, int index) {
    if (!Owner!.GetComponent<MasterMesh>().Render) return;

    GL.DrawArrays(PrimitiveType.Lines, 0, masterMesh.Meshes[index].VertexArray.Count);
  }

  private void HandleBoundingBox(Camera camera) {
    var boundingBox = Owner!.GetComponent<BoundingBox>();
    if (boundingBox == null) return;
    if (!boundingBox.Render) return;

    boundingBox.Draw(camera);
  }
}